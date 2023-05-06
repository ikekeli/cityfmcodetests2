namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to HTML
    /// </summary>
    public class ExportHtml : AbstractExport
    {
        public override string FileExtension { get; set; } = "html";

        public override bool UseCharset { get; set; } = true; // Add charset to content type

        // Constructor
        public ExportHtml(object tbl) : base(tbl) {}

        public override object ExportFieldValue(DbField fld)
        {
            var exportValue = fld.ExportValue;
            if (fld.ExportFieldImage && fld.ViewTag == "IMAGE") {
                List<string> images = fld.GetTempImage().GetAwaiter().GetResult();
                exportValue = GetFileImgTag(images.Select(f => ImageFileToBase64Url(f).GetAwaiter().GetResult()).ToList());
            } else if (fld.ExportFieldImage && !Empty(fld.ExportHrefValue)) { // Export custom view tag
                exportValue = GetFileImgTag(new List<string> { ImageFileToBase64Url(ConvertToString(fld.ExportHrefValue)).GetAwaiter().GetResult() });
            }
            return exportValue;
        }

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="imageFile">Image file name</param>
        /// <param name="breakType">Page break type (before / after)</param>
        public override void AddImage(string imageFile, string breakType = "")
        {
            string classes = "ew-export";
            if (SameText(breakType, "before"))
                classes += " break-before-page";
            else if (SameText(breakType, "after"))
                classes += " break-after-page";
            string html = "<div class=\"" + classes + "\">" + GetFileImgTag(new List<string> { ImageFileToBase64Url(imageFile).GetAwaiter().GetResult() }) + "</div>";
            if (Text.ToString().Contains("</body>"))
                Text.Replace("</body>", html + "</body>"); // Insert before </body>
            else
                Text.Append(html); // Append to end
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
                return Controller.Content(Text.ToString(), "text/html", Encoding.UTF8);
            }
            return new EmptyResult();
        }
    }
} // End Partial class
