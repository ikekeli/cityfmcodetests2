namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Sms class
    /// </summary>
    public class Sms
    {
        public string Recipient { get; set; } = ""; // Recipient

        public string Content { get; set; } = ""; // Content

        public string SendError { get; set; } = ""; // Send error description

        // Load message from template
        public async Task Load(string fn, string langId = "")
        {
            langId = Empty(langId) ? CurrentLanguage : langId;
            string wrk = "";
            int pos = fn.LastIndexOf('.');
            if (pos > -1) {
                string name = fn.Substring(0, pos); // Get file name
                string ext = fn.Substring(pos + 1); // Get file extension
                string path = IncludeTrailingDelimiter(Config.SmsTemplatePath, true); // Get file path
                List<string> list = !Empty(langId) ? new () { "_" + langId, "-" + langId, "" } : new ();
                string? file = list.Select(suffix => path + name + suffix + "." + ext).FirstOrDefault(f => FileExists(MapPath(f)));
                if (file == null)
                    return;
                wrk = await LoadText(file); // Load template file content
                if (wrk.StartsWith("\xEF\xBB\xBF")) // UTF-8 BOM
                    wrk = wrk.Substring(3);
                string id = name + "_content";
                if (wrk.Contains(id)) { // Replace content
                    file = path + id + "." + ext;
                    if (FileExists(MapPath(file))) {
                        string content = await LoadText(file);
                        if (content.StartsWith("\xEF\xBB\xBF")) // UTF-8 BOM
                            content = content.Substring(3);
                        wrk = wrk.Replace("<!--" + id + "-->", content);
                    }
                }
            }
            wrk = wrk.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n"); // Convert line breaks
            if (!Empty(wrk)) {
                string[] ar = wrk.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.RemoveEmptyEntries); // Locate header and mail content
                if (ar.Length > 1) {
                    Content = ar[1];
                    string[] headers = ar[0].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var ti = CultureInfo.GetCultureInfo("en-US").TextInfo;
                    foreach (string header in headers) {
                        string[] ar2 = header.Split(':');
                        if (ar2.Length > 1)
                            SetPropertyValue(this, ti.ToTitleCase(ar2[0].Trim()), ar2[1].Trim());
                    }
                }
            }
        }

        // Replace content
        public void ReplaceContent(string find, string replaceWith) =>
            Content = Content.Replace(find, replaceWith);

        // Send SMS
        public bool Send() => SendAsync().GetAwaiter().GetResult();

        // Send SMS Async
        public Task<bool> SendAsync() => Task.FromResult(false); // Not implemented
    }
} // End Partial class
