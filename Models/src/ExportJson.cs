namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to JSON
    /// </summary>
    public class ExportJson : AbstractExport
    {
        public override string FileExtension { get; set; } = "json";

        public override string Disposition { get; set; } = "inline";

        private Dictionary<string, object> Item = new ();

        public List<Dictionary<string, object>> Items = new ();

        // Constructor
        public ExportJson(object tbl) : base(tbl) {}

        // Style
        public override void SetStyle(string style) {}

        // Field caption
        public override void ExportCaption(DbField fld) {}

        // Field value
        public override void ExportValue(DbField fld) {}

        // Field aggregate
        public override void ExportAggregate(DbField fld, string type) {}

        // Get meta tag for charset
        public override string CharsetMetaTag() => "";

        // Has parent
        public bool HasParent;

        // Get parent
        public Dictionary<string, object>? Parent => Items.LastOrDefault();

        // Table header
        public override void ExportTableHeader()
        {
            HasParent = Items.Any();
            if (!Empty(Table?.Name))
                Parent?.Add(Table?.Name, new List<Dictionary<string, object>>());
        }

        // Export a value (caption, field value, or aggregate)
        public override void ExportValue(DbField fld, object val) {}

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0)
        {
            if (rowCnt <= 0)
                return;
        }

        // End a row
        public override void EndExportRow(int rowCnt = 0)
        {
            if (rowCnt <= 0)
                return;
            if (HasParent && !Empty(Table?.Name))
                Parent?[Table?.Name].Add(Item);
            else
                Items.Add(Item);
        }

        // Empty row
        public override void ExportEmptyRow() {}

        // Page break
        public override void ExportPageBreak() {}

        #pragma warning disable 1998
        // Export a field
        public override async Task ExportField(DbField fld)
        {
            if (!fld.Exportable || fld.IsBlob)
                return;
            Item[fld.Name] = fld.UploadMultiple ? fld.Upload.DbValue : fld.ExportValue;
        }
        #pragma warning restore 1998

        // Table Footer
        public override void ExportTableFooter() {}

        #pragma warning disable 1998
        // Add HTML tags
        public override async Task ExportHeaderAndFooter() {}
        #pragma warning restore 1998

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            object data = Items.Count == 1 ? Items[0] : Items;
            string json = System.Text.Json.JsonSerializer.Serialize(data);
            Text.Clear();
            Text.Append(json);

            // Save to folder
            if (save)
                await SaveFile(ExportPath(true), GetSaveFileName(), Text.ToString());

            // Output
            if (output) {
                Newtonsoft.Json.JsonSerializerSettings options = Config.Debug ? new () { Formatting = Newtonsoft.Json.Formatting.Indented } : new ();
                WriteHeaders(fileName);
                return Controller.Json(data, options);
            }
            return new EmptyResult();
        }
    }
} // End Partial class
