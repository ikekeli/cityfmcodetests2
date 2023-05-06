namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Data types
    /// </summary>
    public enum DataType {
        Number = 1,
        Date = 2,
        String = 3,
        Boolean = 4,
        Memo = 5,
        Blob = 6,
        Time = 7,
        Guid = 8,
        Xml = 9,
        Bit = 10,
        Other = 11
    }

    /// <summary>
    /// Row type
    /// </summary>
    public enum RowType {
        Header = 0, // Row type view
        View = 1, // Row type view
        Add = 2, // Row type add
        Edit = 3, // Row type edit
        Search = 4, // Row type search
        Master = 5, // Row type master record
        AggregateInit = 6, // Row type aggregate init
        Aggregate = 7, // Row type aggregate
        Detail = 8, // Row type detail
        Total = 9, // Row type group summary
        Preview = 10, // Preview record
        PreviewField = 11 // Preview field
    }

    /// <summary>
    /// Row summary
    /// </summary>
    public enum RowSummary {
        Detail = 0, // Detail
        Group = 1, // Page summary
        Page = 2, // Page summary
        Grand = 3 // Grand summary
    }

    /// <summary>
    /// Row total
    /// </summary>
    public enum RowTotal {
        Header = 0, // Header
        Footer = 1, // Footer
        Sum = 2, // SUM
        Avg = 3, // AVG
        Min = 4, // MIN
        Max = 5, // MAX
        Cnt = 6 // CNT
    }

    /// <summary>
    /// Allow
    /// </summary>
    [Flags]
    public enum Allow {
        None = 0,
        Add = 1, // Add
        Delete = 2, // Delete
        Edit = 4, // Edit
        List = 8, // List
        Admin = 16, // Admin
        View = 32, // View
        Search = 64, // Search
        Import = 128, // Import
        Lookup = 256, // Lookup
        Push = 512, // Lookup
        Export = 1024, // Export
        All = Add | Delete | Edit | List | Admin | View | Search | Import | Lookup | Push | Export
    }
} // End Partial class
