namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// User level permission class
    /// </summary>
    public class UserLevelPermission
    {
        // Table name
        [SqlKata.Column("")]
        public string Table { set; get; } = "";

        // User level ID
        [SqlKata.Column("")]
        public int Id { set; get; }

        // Permission
        [SqlKata.Column("")]
        public int Permission { set; get; } = 0;
    }
} // End Partial class
