namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Database connection base class
    /// </summary>
    /// <typeparam name="N">DbConnection</typeparam>
    /// <typeparam name="M">DbCommand</typeparam>
    /// <typeparam name="R">DbDataReader</typeparam>
    /// <typeparam name="T">Field data type</typeparam>
    public class DatabaseConnectionBase<N, M, R, T> : IDisposable
        where N : DbConnection
        where M : DbCommand
        where R : DbDataReader
    {
        private bool _disposed = false;

        public string ConnectionString { get; set; } = "";

        public ConfigurationSection Info { get; set; }

        public string DbId { get; set; }

        private Lazy<N> _conn; // Main connection

        private Lazy<N> _conn2; // Secondary connection

        private Lazy<QueryFactory> _queryFactory; // Query factory for main connection

        private Lazy<QueryFactory> _queryFactory2; // Query factory for secondary connection

        private DbTransaction? _trans;

        private List<M> _commands = new ();

        private List<R> _dataReaders = new ();

        // Constructor
        public DatabaseConnectionBase(string dbid)
        {
            Info = Db(dbid);
            DbId = Info["id"] ?? "";
            if (Empty(DbId))
                throw new Exception($"Database ID '{DbId}' not found.");
            _conn = new Lazy<N>(() => OpenConnection());
            _conn2 = new Lazy<N>(() => OpenConnection());
            _queryFactory = new Lazy<QueryFactory>(() => new QueryFactory {
                Connection = DbConn, // Main connection
                Compiler = GetCompiler(),
                Logger = compiled => Log(compiled.ToString()), // Log the compiled query
                QueryTimeout = Config.QueryTimeout
            });
            _queryFactory2 = new Lazy<QueryFactory>(() => new QueryFactory {
                Connection = DbConn2, // Secondary connection
                Compiler = GetCompiler(),
                Logger = compiled => Log(compiled.ToString()), // Log the compiled query
                QueryTimeout = Config.QueryTimeout
            });
        }

        // Constructor
        public DatabaseConnectionBase() : this("DB") {}

        // Overrides Object.Finalize
        ~DatabaseConnectionBase() => Dispose(false);

        // Main connection as IDbConnection
        public DbConnection DbConn => (DbConnection)_conn.Value;

        // Secondary connection as IDbConnection
        public DbConnection DbConn2 => (DbConnection)_conn2.Value;

        // Query builder compiler
        public Compiler GetCompiler(string output = "")
        {
            ICustomCompiler compiler;
            if (IsMySql)
                compiler = new CustomMySqlCompiler();
            else if (IsMsSql2012)
                compiler = new CustomSqlServerCompiler();
            else if (IsMsSql)
                compiler = new CustomSqlServerCompiler { UseLegacyPagination = true };
            else if (IsOracle)
                compiler = new CustomOracleCompiler();
            else if (IsPostgreSql)
                compiler = new CustomPostgresCompiler();
            else if (IsSqlite)
                compiler = new CustomSqliteCompiler();
            else
                throw new Exception("Query builder does not support ODBC.");
            compiler.Output = output;
            return (Compiler)compiler;
        }

        // Get query factory
        public QueryFactory GetQueryFactory(bool main) => main ? _queryFactory.Value : _queryFactory2.Value;

        // Get query builder
        public QueryBuilder GetQueryBuilder(string table = "", bool main = false)
        {
            var db = GetQueryFactory(main);
            return (table != "") ? db.Query(table) : db.Query();
        }

        // Get data type
        public T GetDataType(object dt) => (T)dt;

        // Access
        public bool IsAccess => IsOdbc && ConvertToBool(Info["msaccess"]);

        // ODBC
        public bool IsOdbc => typeof(N).ToString().Contains(".OdbcConnection");

        // Microsoft SQL Server
        public bool IsMsSql => typeof(N).ToString().Contains(".SqlConnection");

        // Microsoft SQL Server >= 2012
        public bool IsMsSql2012;

        // MySQL
        public bool IsMySql => typeof(N).ToString().Contains(".MySqlConnection");

        // PostgreSQL
        public bool IsPostgreSql => typeof(N).ToString().Contains(".NpgsqlConnection");

        // Oracle
        public bool IsOracle => typeof(N).ToString().Contains(".OracleConnection");

        // Oracle >= 12.1
        public bool IsOracle12;

        // Sqlite
        public bool IsSqlite => typeof(N).ToString().Contains(".SqliteConnection");

        /// <summary>
        /// Select limit
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="nrows">Number of rows</param>
        /// <param name="offset">Offset</param>
        /// <param name="hasOrderBy">SQL has ORDER BY clause or now</param>
        /// <returns>Task that returns data reader</returns>
        public async Task<R> SelectLimit(string sql, int nrows = -1, int offset = -1, bool hasOrderBy = false)
        {
            string offsetPart, limitPart, originalSql = sql;
            if (IsMsSql) {
                if (IsMsSql2012) { // Microsoft SQL Server >= 2012
                    if (!hasOrderBy)
                        sql += " ORDER BY @@version"; // Dummy ORDER BY clause
                    if (offset > -1)
                        sql += " OFFSET " + ConvertToString(offset) + " ROWS";
                    if (nrows > 0) {
                        if (offset < 0)
                            sql += " OFFSET 0 ROWS";
                        sql += " FETCH NEXT " + ConvertToString(nrows) + " ROWS ONLY";
                    }
                } else { // Select top
                    if (nrows > 0) {
                        if (offset > 0)
                            nrows += offset;
                        sql = Regex.Replace(sql, @"(^\s*SELECT\s+(DISTINCT)?)", @"$1 TOP " + ConvertToString(nrows) + " ", RegexOptions.IgnoreCase); // DN
                    }
                }
            } else if (IsMySql) {
                offsetPart = (offset >= 0) ? ConvertToString(offset) + "," : "";
                limitPart = (nrows < 0) ? "18446744073709551615" : ConvertToString(nrows);
                sql += " LIMIT " + offsetPart + limitPart;
            } else if (IsPostgreSql || IsSqlite) {
                offsetPart = (offset >= 0) ? " OFFSET " + ConvertToString(offset) : "";
                limitPart = (nrows >= 0) ? " LIMIT " + ConvertToString(nrows) : "";
                sql += limitPart + offsetPart;
            } else if (IsOracle) { // Select top
                if (IsOracle12) { // Oracle >= 12.1
                    if (offset > -1)
                        sql += " OFFSET " + ConvertToString(offset) + " ROWS";
                    if (nrows > 0)
                        sql += " FETCH NEXT " + ConvertToString(nrows) + " ROWS ONLY";
                } else {
                    if (nrows > 0) {
                        if (offset > 0)
                            nrows += offset;
                        sql = "SELECT * FROM (" + sql + ") WHERE ROWNUM <= " + ConvertToString(nrows);
                    }
                }
            } else if (IsAccess) { // Select top
                if (nrows > 0) {
                    if (offset > 0)
                        nrows += offset;
                    sql = Regex.Replace(sql, @"(^\s*SELECT\s+(DISTINCTROW|DISTINCT)?)", @"$1 TOP " + ConvertToString(nrows) + " ", RegexOptions.IgnoreCase); // DN
                }
            }
            if (!SelectOffset) {
                return await GetDataReaderAsync(sql) ?? await GetDataReaderAsync(originalSql); // If SQL fails due to being too complex, use original SQL.
            } else {
                return await GetDataReaderAsync(sql);
            }
        }

        // Supports select offset
        public bool SelectOffset => IsMySql || IsPostgreSql || IsMsSql2012 || IsOracle12 || IsSqlite;

        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <param name="main">Whether to use the main connection</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, bool main = false) =>
            main // Main connection
            ? DbConn.Execute(LogSql(sql), param, transaction ?? _trans, commandTimeout, commandType)
            : DbConn2.Execute(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a command (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <param name="main">Whether to use the main connection</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, bool main = false) =>
            main // Main connection
            ? await DbConn.ExecuteAsync(LogSql(sql), param, transaction ?? _trans, commandTimeout, commandType)
            : await DbConn2.ExecuteAsync(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute parameterized SQL using the secondary connection
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.Execute(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute a command asynchronously using Task using the secondary connection (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="transaction">The transaction to use for this query.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecuteNonQueryAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.ExecuteAsync(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <param name="main">Whether to use the main connection</param>
        /// <returns>The first cell selected as <see cref="object"/>.</returns>
        public object ExecuteScalar(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, bool main = false) =>
            main // Main connection
            ? DbConn.ExecuteScalar(LogSql(sql), param, transaction ?? _trans, commandTimeout, commandType)
            : DbConn2.ExecuteScalar(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Execute parameterized SQL that selects a single value (async)
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="transaction">The transaction to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <param name="main">Whether to use the main connection</param>
        /// <returns>The first cell returned, as <see cref="object"/>.</returns>
        public async Task<object> ExecuteScalarAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, bool main = false) =>
            main // Main connection
            ? await DbConn.ExecuteScalarAsync(LogSql(sql), param, transaction ?? _trans, commandTimeout, commandType)
            : await DbConn2.ExecuteScalarAsync(LogSql(sql), param, transaction, commandTimeout, commandType);

        /// <summary>
        /// Executes the query, and returns the row(s) as JSON (async)
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "firstOnly" (bool) Returns the first row only
        /// "array" (bool) Returns the result as array
        /// "convertDate" (bool) Convert date by JavaScriptDateTimeConverter
        /// </param>
        /// <returns>Tasks that returns JSON string</returns>
        public async Task<string> ExecuteJsonAsync(string sql, dynamic? options = null)
        {
            IDictionary<string, object> opts = ConvertToDictionary<object>(options);
            bool firstOnly = opts.TryGetValue("firstOnly", out object? first) && ConvertToBool(first);
            bool array = opts.TryGetValue("array", out object? ar) && ConvertToBool(ar);
            bool convertDate = opts.TryGetValue("convertDate", out object? cd) && ConvertToBool(cd);
            Newtonsoft.Json.Converters.JavaScriptDateTimeConverter? convertor = convertDate ? new () : null;
            if (firstOnly) {
                var row = await GetRowAsync(sql);
                if (convertor != null)
                    return (array) ? ConvertToJson(row?.Values, convertor) : ConvertToJson(row, convertor);
                else
                    return (array) ? ConvertToJson(row?.Values) : ConvertToJson(row);
            } else {
                var rows = await GetRowsAsync(sql);
                if (convertor != null)
                    return (array) ? ConvertToJson(rows?.Select(d => d.Values), convertor) : ConvertToJson(rows, convertor);
                else
                    return (array) ? ConvertToJson(rows?.Select(d => d.Values)) : ConvertToJson(rows);
            }
        }

        /// <summary>
        /// Executes the query, and returns the row(s) as JSON
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "firstOnly" (bool) Returns the first row only
        /// "array" (bool) Returns the result as array
        /// "convertDate" (bool) Convert date by JavaScriptDateTimeConverter
        /// </param>
        /// <returns>JSON string</returns>
        public string ExecuteJson(string sql, dynamic? options = null) => ExecuteJsonAsync(sql, options).GetAwaiter().GetResult();

        // Get data reader
        public R GetDataReader(string sql)
        {
            var cmd = GetCommand(sql); // Use main connection
            return (R)cmd.ExecuteReader();
        }

        // Get data reader
        public async Task<R> GetDataReaderAsync(string sql)
        {
            var cmd = GetCommand(sql); // Use main connection
            return (R)await cmd.ExecuteReaderAsync();
        }

        // Get a new connection
        public virtual async Task<N> OpenConnectionAsync()
        {
            var connstr = GetConnectionString(Info);
            string dbid = ConvertToString(Info["id"]);
            if (IsSqlite) {
                var relpath = Info["relpath"];
                var dbname = Info["dbname"];
                if (Empty(relpath)) {
                    connstr += AppRoot() + dbname;
                } else if (Path.IsPathRooted(relpath)) { // Physical path
                    connstr += relpath + dbname;
                } else { // Relative to wwwroot
                    connstr += ServerMapPath(relpath) + dbname;
                }
                connstr = connstr.Replace("\\", "/");
                ConnectionString = connstr;
            }
            DatabaseConnecting(ref connstr);
            var c = Activator.CreateInstance(typeof(N), new object[] { connstr }) is N n
                ? n
                : throw new Exception($"Failed to create connection for database '{dbid}'");
            await c.OpenAsync();
            string timezone = Info["timezone"] ?? Config.DbTimeZone;
            if (IsOracle) {
                if (!Empty(Info["schema"]))
                    await c.ExecuteAsync("ALTER SESSION SET CURRENT_SCHEMA = " + QuotedName(Info["schema"]!, dbid)); // Set current schema
                await c.ExecuteAsync("ALTER SESSION SET NLS_TIMESTAMP_FORMAT = 'yyyy-mm-dd hh24:mi:ss'");
                await c.ExecuteAsync("ALTER SESSION SET NLS_TIMESTAMP_TZ_FORMAT = 'yyyy-mm-dd hh24:mi:ss'");
                if (timezone != "")
                    await c.ExecuteAsync("ALTER SESSION SET TIME_ZONE = '" + timezone + "'");
                var m = Regex.Match(ConvertToString(c.ExecuteScalar("SELECT * FROM v$version WHERE banner LIKE 'Oracle%'")), @"Release (\d+\.\d+)");
                IsOracle12 = m.Success && Convert.ToDouble(m.Groups[1].Value) >= 12.1;
            } else if (IsMsSql) {
                var m = Regex.Match(ConvertToString(c.ExecuteScalar("SELECT @@version")), @"(\d+)\.(\d+)\.(\d+)(\.(\d+))");
                IsMsSql2012 = m.Success && Convert.ToInt32(m.Groups[1].Value) >= 11;
            } else if (IsPostgreSql) {
                if (!Empty(Info["schema"]))
                    await c.ExecuteAsync("SET search_path TO " + QuotedName(Info["schema"]!, dbid)); // Set current schema
                if (timezone != "")
                    await c.ExecuteAsync("SET TIME ZONE '" + timezone + "'");
            } else if (IsMySql && timezone != "") {
                await c.ExecuteAsync("SET time_zone = '" + timezone + "'");
            }
            DatabaseConnected(c);
            return c;
        }

        // Get a new connection
        public virtual N OpenConnection() => OpenConnectionAsync().GetAwaiter().GetResult();

        // Get command
        public M GetCommand(string sql)
        {
            if (Activator.CreateInstance(typeof(M), new object[] { LogSql(sql), DbConn }) is M cmd) {
                if (_trans != null)
                    cmd.Transaction = _trans;
                return cmd;
            }
            throw new Exception($"Failed to create {nameof(M)} for SQL: {sql}");
        }

        // Get a new command
        public M OpenCommand(string sql)
        {
            if (Activator.CreateInstance(typeof(M), new object[] { LogSql(sql), DbConn2 }) is M cmd) {
                _commands.Add(cmd);
                return cmd;
            }
            throw new Exception($"Failed to create {nameof(M)} for SQL: {sql}");
        }

        // Get a new data reader
        public R? OpenDataReader(string sql)
        {
            try {
                if (OpenCommand(sql) is M cmd) { // Use secondary connection
                    var r = (R)cmd.ExecuteReader();
                    _dataReaders.Add(r);
                    return r;
                }
            } catch {
                if (Config.Debug)
                    throw;
            }
            return null;
        }

        // Get a new data reader
        public async Task<R?> OpenDataReaderAsync(string sql)
        {
            try {
                if (OpenCommand(sql) is M cmd) { // Use secondary connection
                    var r = (R)await cmd.ExecuteReaderAsync();
                    _dataReaders.Add(r);
                    return r;
                }
            } catch {
                if (Config.Debug)
                    throw;
            }
            return null;
        }

        /// <summary>
        /// Get a row from data reader
        /// </summary>
        /// <param name="dr">Data reader</param>
        /// <returns>Row as dictionary</returns>
        [return: NotNullIfNotNull("dr")]
        public Dictionary<string, object>? GetRow(DbDataReader? dr) => GetDictionary(dr);

        /// <summary>
        /// Get a row by SQL
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="main">Use main connection or not</param>
        /// <returns>Row as dictionary</returns>
        public Dictionary<string, object>? GetRow(string sql, bool main = false)
        {
            var row = main ? DbConn.QueryFirstOrDefault(LogSql(sql), null, _trans) : DbConn2.QueryFirstOrDefault(LogSql(sql));
            return (row != null) ? new Dictionary<string, object>((IDictionary<string, object>)row) : null;
        }

        /// <summary>
        /// Get a row by SQL (async)
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="main">Use main connection or not</param>
        /// <returns>Task that returns row as dictionary</returns>
        public async Task<Dictionary<string, object>?> GetRowAsync(string sql, bool main = false)
        {
            var row = main ? await DbConn.QueryFirstOrDefaultAsync(LogSql(sql), null, _trans) : await DbConn2.QueryFirstOrDefaultAsync(LogSql(sql));
            return (row != null) ? new Dictionary<string, object>((IDictionary<string, object>)row) : null;
        }

        /// <summary>
        /// Get rows from data reader
        /// </summary>
        /// <param name="dr">Data reader</param>
        /// <returns>Rows as list of dictionary</returns>
        public List<Dictionary<string, object>> GetRows(DbDataReader? dr)
        {
            var rows = new List<Dictionary<string, object>>();
            while (dr != null && dr.Read() && GetRow(dr) is Dictionary<string, object> row)
                rows.Add(row);
            return rows;
        }

        /// <summary>
        /// Get rows from data reader (async)
        /// </summary>
        /// <param name="dr">Data reader</param>
        /// <returns>Task that returns rows as list of dictionary</returns>
        public async Task<List<Dictionary<string, object>>> GetRowsAsync(DbDataReader? dr)
        {
            var rows = new List<Dictionary<string, object>>();
            while (dr != null && await dr.ReadAsync() && GetRow(dr) is Dictionary<string, object> row)
                rows.Add(row);
            return rows;
        }

        /// <summary>
        /// Get rows by SQL
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="main">Use main connection or not</param>
        /// <returns>Rows as list of dictionary</returns>
        public List<Dictionary<string, object>> GetRows(string sql, bool main = false) =>
            (main ? DbConn.Query(LogSql(sql), null, _trans) : DbConn2.Query(LogSql(sql))).Select(row => (Dictionary<string, object>)ConvertToDictionary<object>(row)).ToList();

        /// <summary>
        /// Get rows by SQL (async)
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="main">Use main connection or not</param>
        /// <returns>Task that returns rows as list of dictionary</returns>
        public async Task<List<Dictionary<string, object>>> GetRowsAsync(string sql, bool main = false) =>
            (main ? await DbConn.QueryAsync(LogSql(sql), null, _trans) : await DbConn2.QueryAsync(LogSql(sql))).Select(row => (Dictionary<string, object>)ConvertToDictionary<object>(row)).ToList();

        /// <summary>
        /// Get rows by SQL
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Rows as list of array</returns>
        public List<string[]> GetArrays(string sql) =>
            DbConn2.Query(LogSql(sql)).Select(row => ((IDictionary<string, object>)row).Values.Select(v => ConvertToString(v)).ToArray()).ToList();

        /// <summary>
        /// Get rows by SQL (async)
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Task that returns rows as list of array</returns>
        public async Task<List<string[]>> GetArraysAsync(string sql) =>
            (await DbConn2.QueryAsync(LogSql(sql))).Select(row => ((IDictionary<string, object>)row).Values.Select(v => ConvertToString(v)).ToArray()).ToList();

        /// <summary>
        /// Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public IEnumerable<dynamic> Query(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.Query(LogSql(sql), param, transaction, buffered, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic QueryFirst(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.QueryFirst(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic QueryFirstOrDefault(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.QueryFirstOrDefault(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic QuerySingle(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.QuerySingle(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: the row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public dynamic QuerySingleOrDefault(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            DbConn2.QuerySingleOrDefault(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a query (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryAsync(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<dynamic> QueryFirstAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryFirstAsync(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<dynamic> QueryFirstOrDefaultAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryFirstOrDefaultAsync(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<dynamic> QuerySingleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QuerySingleAsync(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query (async)
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<dynamic> QuerySingleOrDefaultAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QuerySingleOrDefaultAsync(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <typeparam name="TRow">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="TRow"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public async Task<IEnumerable<TRow>> QueryAsync<TRow>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryAsync<TRow>(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="TRow">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<TRow> QueryFirstAsync<TRow>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryFirstAsync<TRow>(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="TRow">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<TRow> QueryFirstOrDefaultAsync<TRow>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QueryFirstOrDefaultAsync<TRow>(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="TRow">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<TRow> QuerySingleAsync<TRow>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QuerySingleAsync<TRow>(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="TRow">The type to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public async Task<TRow> QuerySingleOrDefaultAsync<TRow>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) =>
            await DbConn2.QuerySingleOrDefaultAsync<TRow>(LogSql(sql), param, transaction, commandTimeout, commandType); // Use secondary connection

        // Get count (by dataset)
        public int GetCount(string sql)
        {
            int cnt = 0;
            using var dr = OpenDataReader(sql);
            if (dr != null) {
                while (dr.Read())
                    cnt++;
            }
            return cnt;
        }

        // Get transaction
        public DbTransaction? Transaction => _trans;

        // Begin transaction
        public void BeginTrans()
        {
            try {
                _trans = IsOracle || IsSqlite
                    ? DbConn.BeginTransaction()
                    : DbConn.BeginTransaction(IsolationLevel.ReadUncommitted);
            } catch {
                if (Config.Debug)
                    throw;
            }
        }

        // Commit transaction
        public void CommitTrans()
        {
            try {
                _trans?.Commit();
            } catch {
                if (Config.Debug)
                    throw;
            }
        }

        // Rollback transaction
        public void RollbackTrans()
        {
            try {
                _trans?.Rollback();
            } catch {
                if (Config.Debug)
                    throw;
            }
        }

        // Concat // DN
        public string Concat(params string[] list)
        {
            if (IsAccess) {
                return String.Join(" & ", list);
            } else if (IsMsSql) {
                return String.Join(" + ", list);
            } else if (IsOracle || IsPostgreSql) {
                return String.Join(" || ", list);
            } else {
                return "CONCAT(" + String.Join(", ", list) + ")";
            }
        }

        // Table CSS class name
        public string TableClass { get; set; } = "table table-bordered table-sm ew-db-table";

        /// <summary>
        /// Get result in HTML table by SQL
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "fieldcaption" (bool|Dictionary)
        ///   true: Use caption and use language object, or
        ///   false: Use field names directly, or
        ///   Dictionary of fieid caption for looking up field caption by field name
        /// "horizontal" (bool) Whether HTML table is horizontal, default: false
        /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
        /// "tableclass" (string) CSS class names of the table, default: "table table-bordered ew-db-table"
        /// </param>
        /// <returns>HTML string as IHtmlContent</returns>
        public IHtmlContent ExecuteHtml(string sql, dynamic? options = null)
        {
            using var rs = OpenDataReader(sql);
            return ExecuteHtml(rs, options);
        }

        /// <summary>
        /// Get result in HTML table by SQL (async)
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "fieldcaption" (bool|Dictionary)
        ///   true: Use caption and use language object, or
        ///   false: Use field names directly, or
        ///   Dictionary of fieid caption for looking up field caption by field name
        /// "horizontal" (bool) Whether HTML table is horizontal, default: false
        /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
        /// "tableclass" (string) CSS class names of the table, default: "table table-bordered ew-db-table"
        /// </param>
        /// <returns>Task that returns HTML string as IHtmlContent</returns>
        public async Task<IHtmlContent> ExecuteHtmlAsync(string sql, dynamic? options = null)
        {
            using var rs = await OpenDataReaderAsync(sql);
            return await ExecuteHtmlAsync(rs, options);
        }

        /// <summary>
        /// Get result in HTML table (async)
        /// </summary>
        /// <param name="dr">Data reader</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "fieldcaption" (bool|Dictionary|Anonymous Type)
        ///   true: Use caption and use language object, or
        ///   false: Use field names directly, or
        ///   Dictionary of fieid caption for looking up field caption by field name
        /// "horizontal" (bool) Whether HTML table is horizontal, default: false
        /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
        /// "tableclass" (string) CSS class names of the table, default: "table table-bordered table-sm ew-db-table"
        /// </param>
        /// <returns>Task that returns IHtmlContent</returns>
        public async Task<IHtmlContent> ExecuteHtmlAsync(DbDataReader dr, dynamic? options = null)
        {
            if (dr == null || !dr.HasRows || dr.FieldCount < 1)
                return HtmlString.Empty;
            IDictionary<string, object> opts = ConvertToDictionary<object>(options);
            bool horizontal = opts.TryGetValue("horizontal", out object? horiz) && ConvertToBool(horiz);
            string classname = opts.TryGetValue("tableclass", out object? tc) && !Empty(tc) ? (string)tc : TableClass;
            bool hasTableName = opts.TryGetValue("tablename", out object? tablename) && tablename != null;
            bool useFieldCaption = opts.TryGetValue("fieldcaption", out object? caps) && caps != null;
            var captions = (useFieldCaption && caps is not bool) ? ConvertToDictionary<string>(caps) : null;
            string html = "", vhtml = "";
            int cnt = 0;
            if (await dr.ReadAsync()) { // First row
                cnt++;
                // Vertical table
                vhtml = "<table class=\"" + classname + "\"><tbody>";
                for (var i = 0; i < dr.FieldCount; i++)
                    vhtml += "<tr><td>" + GetFieldCaption(dr.GetName(i)) + "</td><td>" + ConvertToString(dr[i]) + "</td></tr>";
                vhtml += "</tbody></table>";
                // Horizontal table
                html = "<table class=\"" + classname + "\">";
                html += "<thead><tr>";
                for (var i = 0; i < dr.FieldCount; i++)
                    html += "<th>" + GetFieldCaption(dr.GetName(i)) + "</th>";
                html += "</tr></thead>";
                html += "<tbody>";
                html += "<tr>";
                for (var i = 0; i < dr.FieldCount; i++)
                    html += "<td>" + ConvertToString(dr[i]) + "</td>";
                html += "</tr>";
            }
            while (await dr.ReadAsync()) { // Other rows
                cnt++;
                html += "<tr>";
                for (var i = 0; i < dr.FieldCount; i++)
                    html += "<td>" + ConvertToString(dr[i]) + "</td>";
                html += "</tr>";
            }
            if (html != "")
                html += "</tbody></table>";
            var str = (cnt > 1 || horizontal) ? html : vhtml;
            return new HtmlString(str);

            // Local function to get field caption
            string GetFieldCaption(string key) {
                string? caption = "";
                if (useFieldCaption) {
                    if (captions != null) {
                        captions.TryGetValue(key, out caption);
                    } else if (hasTableName && !Empty(Language)) {
                        caption = tablename switch {
                            IEnumerable<string> names => names.Select(name => Language.FieldPhrase(name, key, "FldCaption")).First(caption => !Empty(caption)),
                            string str => Language.FieldPhrase(str, key, "FldCaption"),
                            _ => ""
                        };
                    }
                }
                return !Empty(caption) ? caption : key;
            }
        }

        /// <summary>
        /// Get result in HTML table (async)
        /// </summary>
        /// <param name="dr">Data reader</param>
        /// <param name="options">
        /// Options: (Dictionary or Anonymous Type)
        /// "fieldcaption" (bool|Dictionary)
        ///   true: Use caption and use language object, or
        ///   false: Use field names directly, or
        ///   Dictionary of fieid caption for looking up field caption by field name
        /// "horizontal" (bool) Whether HTML table is horizontal, default: false
        /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
        /// "tableclass" (string) CSS class names of the table, default: "table table-bordered ew-db-table"
        /// </param>
        /// <returns>HTML string as IHtmlContent</returns>
        public IHtmlContent ExecuteHtml(DbDataReader dr, dynamic? options = null) =>
            ExecuteHtmlAsync(dr, options).GetAwaiter().GetResult();

        // Close
        public void Close()
        {
            if (_conn.IsValueCreated) {
                using (_trans) {} // Dispose
                using (_conn.Value) {} // Dispose
            }
            if (_conn2.IsValueCreated) {
                using (_conn2.Value) {
                    foreach (R dr in _dataReaders) {
                        using (dr) {} // Dispose
                    }
                    foreach (M cmd in _commands) {
                        using (cmd) {} // Dispose
                    }
                }
            }
        }

        // Releases all resources used by this object
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Custom Dispose method to clean up unmanaged resources
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                Close();
            _disposed = true;
        }

        #pragma warning disable 162
        // Get connecton string from connection info
        public string GetConnectionString(ConfigurationSection Info)
        {
            string connectionString = Info["connectionstring"] ?? "";
            string uid = Info["username"] ?? "";
            string pwd = Info["password"] ?? "";
            if (Config.EncryptionEnabled) {
                uid = AesDecrypt(uid);
                pwd = AesDecrypt(pwd);
            }
            return connectionString.Replace("{uid}", uid).Replace("{pwd}", pwd);
        }
        #pragma warning restore 162

        // Database Connecting event
        public virtual void DatabaseConnecting(ref string connstr) {
            // Check Info["id"] for database ID if more than one database
        }

        // Database Connected event
        public virtual void DatabaseConnected(DbConnection conn) {
            // Example:
            //conn.Execute("Your SQL");
        }
    }
} // End Partial class
