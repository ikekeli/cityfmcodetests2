namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to Word
    /// </summary>
    public class ExportWord : AbstractExport
    {
        public override string FileExtension { get; set; } = "doc";

        public override bool UseCharset { get; set; } = true; // Add charset to content type

        public override bool UseBom { get; set; } = true; // Output byte order mark

        public override bool UseInlineStyles { get; set; } = true; // Use inline styles (Does not support multiple CSS classes)

        public override bool ExportImages { get; set; } = false; // Does not support images

        // Constructor
        public ExportWord(object tbl) : base(tbl) {}

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
