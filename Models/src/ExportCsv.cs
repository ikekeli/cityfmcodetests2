namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to CSV
    /// </summary>
    public class ExportCsv : AbstractExport
    {
        public override string FileExtension { get; set; } = "csv";

        public override bool UseCharset { get; set; } = true; // Add charset to content type

        public override bool UseBom { get; set; } = true; // Output byte order mark

        public static string QuoteChar = "\"";

        public static string Separator = ",";

        // Constructor
        public ExportCsv(object tbl) : base(tbl) {}

        // Style
        public override void SetStyle(string style) => Horizontal = true;

        // Table header
        public override void ExportTableHeader() {}

        // Export a value (caption, field value, or aggregate)
        public override void ExportValue(DbField fld, object val)
        {
            if (!fld.IsBlob) {
                if (!Empty(Line))
                    Line += Separator;
                if (fld.IsBoolean && CurrentPage.RowType != RowType.Header)
                    Line += ConvertToBool(val);
                else
                    Line += QuoteChar + ConvertToString(val).Replace(QuoteChar, QuoteChar + QuoteChar) + QuoteChar;
            }
        }

        // Export aggregate
        public override void ExportAggregate(DbField fld, string type) {}

        // Begin a row
        public override void BeginExportRow(int rowCnt = 0) => Line = "";

        // End a row
        public override void EndExportRow(int rowCnt = 0) => Text.AppendLine(Line);

        // Empty row
        public override void ExportEmptyRow() {}

        #pragma warning disable 1998
        // Export a field
        public override async Task ExportField(DbField fld)
        {
            if (!fld.Exportable)
                return;
            if (fld.UploadMultiple)
                ExportValue(fld, fld.Upload.DbValue);
            else
                ExportValue(fld);
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
            // Save to folder
            if (save)
                await SaveFile(ExportPath(true), GetSaveFileName(), Text.ToString());

            // Output
            if (output) {
                WriteHeaders(fileName);
                return Controller.Content(Bom() + Text.ToString());
            }
            return new EmptyResult();
        }
    }
} // End Partial class
