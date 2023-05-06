namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {

    public class CustomPostgresCompiler : PostgresCompiler, ICustomCompiler
    {
        public static bool QuoteIdentifier = true;

        public string Output { get; set; } = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomPostgresCompiler()
        {
            LastId = $"SELECT lastval() AS {WrapValue("Id")}";
        }

        /// <summary>
        /// Wrap a single string in a column identifier.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Wrap(string value)
        {
            if (value.ToLowerInvariant().Contains(" as "))
            {
                var (before, after) = SplitAlias(value);
                return Wrap(before) + $" {ColumnAsKeyword}" + WrapValue(after);
            }
            if (!QuoteIdentifier)
                return value;
            if (value.Contains("."))
            {
                bool open = false;
                return string.Join(".", value.Split('.').Select((x, index) =>
                {
                    if (x.StartsWith(OpeningIdentifier) && x.EndsWith(ClosingIdentifier)) // Already quoted
                    {
                        return x;
                    }
                    else if (x.StartsWith(OpeningIdentifier) && !x.EndsWith(ClosingIdentifier)) // With opening identifier but without closing identifier
                    {
                        open = true;
                        return x;
                    }
                    else if (!x.StartsWith(OpeningIdentifier) && x.EndsWith(ClosingIdentifier)) // Without opening identifier but with closing identifier
                    {
                        open = false;
                        return x;
                    } else if (open) {
                        return x;
                    }
                    return WrapValue(x);
                }));
            }
            if (value.StartsWith(OpeningIdentifier) && value.EndsWith(ClosingIdentifier)) // Already quoted
                return value;

            // If we reach here then the value does not contain an "AS" alias
            // nor dot "." expression, so wrap it as regular value.
            return WrapValue(value);
        }

        /// <summary>
        /// Override CompileValueInsertClauses() - Support RETURNING clause
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="table"></param>
        /// <param name="insertClauses"></param>
        /// <returns></returns>
        protected override SqlResult CompileValueInsertClauses(SqlResult ctx, string table, IEnumerable<InsertClause> insertClauses)
        {
            string lastId = LastId;
            if (!string.IsNullOrEmpty(Output))
                LastId = ""; // Do not use LastId if Output not empty
            try {
                var result = base.CompileValueInsertClauses(ctx, table, insertClauses);
                bool isMultiValueInsert = insertClauses.Skip(1).Any();
                if (!isMultiValueInsert && !string.IsNullOrEmpty(Output))
                    result.RawSql += $" RETURNING {WrapValue(Output)} AS {WrapValue("Id")}" ; // Add RETURNING clause
                return result;
            } finally {
                LastId = lastId; // Reset
            }
        }
    }
} // End Partial class
