namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Export to Excel class
    /// </summary>
    public class ExportExcel : AbstractExport
    {
        public override string FileExtension { get; set; } = "xls";

        public override bool UseCharset { get; set; } = true; // Add charset to content type

        public override bool UseBom { get; set; } = true; // Output byte order mark

        public override bool UseInlineStyles { get; set; } = true; // Use inline styles (Does not support multiple CSS classes)

        public override bool ExportImages { get; set; } = false; // Does not support images

        // Constructor
        public ExportExcel(object tbl) : base(tbl) {}

        // Export a value (caption, field value, or aggregate)
        public override void ExportValue(DbField fld, object val)
        {
            if ((fld.DataType == DataType.String || fld.DataType == DataType.Memo) && IsNumeric(val))
                val = "=\"" + ConvertToString(val) + "\"";
            base.ExportValue(fld, val);
        }

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            await AdjustHtml();

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
