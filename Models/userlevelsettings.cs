namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    // Configuration
    public static partial class Config
    {
        //
        // User level settings
        //

        // User level info
        public static List<UserLevel> UserLevels = new ()
        {
            new () { Id = -2, Name = "Anonymous" }
        };

        // User level priv info
        public static List<UserLevelPermission> UserLevelPermissions = new ()
        {
            new () { Table = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}fxrate", Id = -2, Permission = 0 },
            new () { Table = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}product", Id = -2, Permission = 0 },
            new () { Table = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}order", Id = -2, Permission = 0 },
            new () { Table = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}orderlineitem", Id = -2, Permission = 0 }
        };

        // User level table info // DN
        public static List<UserLevelTablePermission> UserLevelTablePermissions = new ()
        {
            new () { TableName = "fxrate", TableVar = "fxrate", Caption = "FX Rate", Allowed = true, ProjectId = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}", Url = "fxratelist" },
            new () { TableName = "product", TableVar = "product", Caption = "Products", Allowed = true, ProjectId = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}", Url = "productlist" },
            new () { TableName = "order", TableVar = "order", Caption = "Orders", Allowed = true, ProjectId = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}", Url = "orderlist" },
            new () { TableName = "orderlineitem", TableVar = "orderlineitem", Caption = "OrderLineItem", Allowed = true, ProjectId = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}", Url = "" }
        };
    }
} // End Partial class
