namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// DatabaseConnection class
    /// </summary>
    public class DatabaseConnection<N, M, R, T> : DatabaseConnectionBase<N, M, R, T>
        where N : DbConnection
        where M : DbCommand
        where R : DbDataReader
    {
        // Constructor
        public DatabaseConnection(string dbid) : base(dbid)
        {
        }

        // Constructor
        public DatabaseConnection() : base()
        {
        }
    }
} // End Partial class
