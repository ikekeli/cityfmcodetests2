namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for export to email
    /// </summary>
    public class ExportEmail : AbstractExport
    {
        public override string FileExtension { get; set; } = "html";

        public override string Disposition { get; set; } = "inline";

        // Constructor
        public ExportEmail(object tbl) : base(tbl) {}

        /// <summary>
        /// Table header
        /// </summary>
        public override void ExportTableHeader() => Text.Append("<table style=\"border-collapse: collapse;\">"); // Use inline style for Gmail

        /// <summary>
        /// Cell styles
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <returns>Cell styles</returns>
        public override string CellStyles(DbField fld)
        {
            string cellStyles = String.Join(";", Config.ExportTableCellSyles.Select(kv => kv.Key + ":" + kv.Value));
            fld.CellAttrs.Prepend("style", cellStyles); // Use inline style for Gmail
            return ExportStyles ? fld.CellStyles : "";
        }

        /// <summary>
        /// Export field value
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <returns>Field value</returns>
        public override object ExportFieldValue(DbField fld)
        {
            var value = fld.ExportValue;
            if (fld.ExportFieldImage && fld.ViewTag == "IMAGE") {
                if (fld.ImageResize) {
                    value = GetFileImgTag(fld.GetTempImage().GetAwaiter().GetResult());
                } else if (!Empty(fld.ExportHrefValue) && fld.Upload != null) {
                    if (!Empty(fld.Upload.DbValue))
                        value = GetFileATag(fld, fld.ExportHrefValue);
                }
            } else if (fld.ExportFieldImage && fld.ExportHrefValue is Func<string, Task<string>> func) { // Export Custom View Tag // DN
                value = func("email").GetAwaiter().GetResult(); // DN
            }
            return value;
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
            string html = "<div class=\"" + classes + "\">" + GetFileImgTag(new List<string> { imageFile }) + "</div>";
            if (Text.ToString().Contains("</body>"))
                Text.Replace("</body>", html + "</body>"); // Insert before </body>
            else
                Text.Append(html); // Append to end
        }

        /// <summary>
        /// Adjust src attribute of image tags
        /// </summary>
        /// <param name="html">HTML</param>
        /// <returns>HTML</returns>
        public async Task<string> AdjustImage(string html)
        {
            var doc = GetDocument(html); // Load html
            bool inline = GetDisposition() == "inline"; // Inline
            foreach (var img in doc.QuerySelectorAll("img")) {
                string src = img.GetAttribute("src") ?? "";
                if (src.StartsWith("data:") && src.Contains(";base64,")) { // Data URL
                    if (inline) // Inline (No change required if disposition is "attachment")
                        img.SetAttribute("src", await TempImage(DataFromBase64Url(src), true));
                } else if (File.Exists(src)) { // Not embedded image
                    if (inline) // Inline
                        img.SetAttribute("src", await TempImage(await FileReadAllBytes(src), true)); // Create temp image as cid URL
                    else // Attachment
                        img.SetAttribute("src", ImageFileToBase64Url(src)); // Replace image by data URL
                }
            }
            return doc.DocumentElement.OuterHtml;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> Send(string fileName)
        {
            string sender = Param("sender");
            string recipient = Param("recipient");
            string cc = Param("cc");
            string bcc = Param("bcc");
            string subject = Param("subject");
            string message = Param("message");
            bool inline = GetDisposition() == "inline"; // Inline
            string content = await AdjustImage(Text.ToString());

            // Send email
            try {
                var email = new Email();
                email.Sender = sender; // Sender
                email.Recipient = recipient; // Recipient
                email.Cc = cc; // cc
                email.Bcc = bcc; // bcc
                email.Subject = subject; // Subject
                email.Format = "html";
                email.Charset = Config.EmailCharset;
                if (!Empty(message))
                    message = RemoveXss(message) + "<br><br>";
                email.Content = message;
                if (inline) { // Inline
                    foreach (string tmpimage in TempImages)
                        email.AddEmbeddedImage(tmpimage);
                    email.Content += content;
                } else {
                    email.AddAttachment(fileName, content);
                }
                var m = CurrentPage?.GetType().GetMethod("EmailSending");
                bool emailSent = ConvertToBool(m?.Invoke(CurrentPage, new object?[] { email, null }) ?? true) ? email.Send() : false;

                // Check email sent status
                if (emailSent) {
                    // Update email sent count
                    Session.SetInt(Config.ExportEmailCounter, Session.GetInt(Config.ExportEmailCounter) + 1);

                    // Sent email success
                    return Controller.Json(new { success = true, message = Language.Phrase("SendEmailSuccess") });
                } else {
                    // Sent email failure
                    return Controller.Json(new { success = false, message = !Empty(email.SendError) ? email.SendError : Language.Phrase("FailedToSendMail") });
                }
            } catch (Exception e) {
                return Controller.Json(new { success = false, message = e.Message }); // DN
            }
        }

        // Export
        public override async Task<IActionResult> Export(string fileName = "", bool output = true, bool save = false)
        {
            await AdjustHtml();

            // Save to folder
            if (save)
                await SaveFile(ExportPath(true), GetSaveFileName(), Text.ToString());

            // Output
            if (output)
                return await Send(fileName);
            return new EmptyResult();
        }
    }
} // End Partial class
