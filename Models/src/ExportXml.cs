namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to XML
    /// </summary>
    public class ExportXml : AbstractExport
    {
        public override string FileExtension { get; set; } = "xml";

        public override string Disposition { get; set; } = "inline";

        public static string NullString = "null";

        public XmlDoc Document = new ();

        // Constructor
        public ExportXml(object tbl) : base(tbl) {}

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

        // Table header
        public override void ExportTableHeader()
        {
            HasParent = Document.DocumentElement != null;
            if (!HasParent && !Empty(Table?.TableVar))
                Document.AddRoot(Table?.TableVar);
        }

        // Export a value (caption, field value, or aggregate)
        public override void ExportValue(DbField fld, object val) {}

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0)
        {
            if (rowCnt <= 0)
                return;
            if (HasParent && !Empty(Table?.TableVar))
                Document.AddRow(Table?.TableVar);
            else
                Document.AddRow();
        }

        // End a row
        public override void EndExportRow(int rowCnt = 0) {}

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
            object value = fld.UploadMultiple ? fld.Upload.DbValue : fld.ExportValue;
            if (IsNull(value))
                value = NullString;
            Document.AddField(fld.Param, value);
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
            Text.Clear();
            Text.Append(Document.OuterXml);

            // Save to folder
            if (save)
                await SaveFile(ExportPath(true), GetSaveFileName(), Text.ToString());

            // Output
            if (output) {
                WriteHeaders(fileName);
                return Controller.Content(Text.ToString());
            }
            return new EmptyResult();
        }
    }
} // End Partial class
