namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Email class
    /// </summary>
    public class Email
    {
        private SecureSocketOptions Options = Enum.Parse<SecureSocketOptions>(Config.SmtpSecureOption); // SecureSocketOptions

        public string Sender { get; set; } = ""; // Sender

        public string Recipient { get; set; } = ""; // Recipient

        public string Cc { get; set; } = ""; // Cc

        public string Bcc { get; set; } = ""; // Bcc

        public string Subject { get; set; } = ""; // Subject

        public string Format { get; set; } = "HTML"; // Format

        public string Content { get; set; } = ""; // Content

        public string Charset { get; set; } = Config.EmailCharset; // Charset

        public string To
        {
            get => Recipient;
            set => Recipient = value;
        } // Alias of Recipient

        public string From
        {
            get => Sender;
            set => Sender = value;
        } // Alias of Sender

        public static SmtpClient? Mailer { get; set; } = null;

        public List<Dictionary<string, string>> Attachments = new (); // Attachments

        public List<string> EmbeddedImages = new (); // Embedded image

        public string SendError = ""; // Send error description

        // Load email from template
        public async Task Load(string fn, string langId = "")
        {
            langId = Empty(langId) ? CurrentLanguage : langId;
            string wrk = "";
            int pos = fn.LastIndexOf('.');
            if (pos > -1) {
                string name = fn.Substring(0, pos); // Get file name
                string ext = fn.Substring(pos + 1); // Get file extension
                string path = IncludeTrailingDelimiter(Config.EmailTemplatePath, true); // Get file path
                List<string> list = !Empty(langId) ? new () { "_" + langId, "-" + langId, "" } : new ();
                string? file = list.Select(suffix => path + name + suffix + "." + ext).FirstOrDefault(f => FileExists(MapPath(f)));
                if (file == null)
                    throw new System.Exception("Email template '" + path + fn + "' not found");
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

        // Get email address with display name
        private string GetEmailAddress(string email, string displayName = "") =>
            Empty(displayName) ? email : displayName + " <" + email + ">";

        // Replace sender
        public void ReplaceSender(string sender, string senderName = "") =>
            Sender = Sender.Replace("<!--$From-->", GetEmailAddress(sender, senderName));

        // Replace recipient
        public void ReplaceRecipient(string recipient, string recipientName = "") =>
            Recipient = Recipient.Replace("<!--$To-->", GetEmailAddress(recipient, recipientName));

        // Method to add recipient
        public void AddRecipient(string recipient, string recipientName = "") =>
            Recipient = Concatenate(Recipient, GetEmailAddress(recipient, recipientName), ",");

        // Add cc email
        public void AddCc(string cc, string ccName = "") =>
            Cc = Concatenate(Cc, GetEmailAddress(cc, ccName), ",");

        // Add bcc email
        public void AddBcc(string bcc, string bccName = "") =>
            Bcc = Concatenate(Bcc, GetEmailAddress(bcc, bccName), ",");

        // Replace subject
        public void ReplaceSubject(string subject) =>
            Subject = Subject.Replace("<!--$Subject-->", subject);

        // Replace content
        public void ReplaceContent(string find, string replaceWith) =>
            Content = Content.Replace(find, replaceWith);

        // Method to add embedded image
        public void AddEmbeddedImage(string image)
        {
            if (!Empty(image))
                EmbeddedImages.Add(image);
        }

        // Method to add attachment
        public void AddAttachment(string fileName, string content = "")
        {
            if (!Empty(fileName))
                Attachments.Add(new () { { "filename", fileName }, { "content", content } });
        }

        // Send email
        public bool Send()
        {
            bool send = SendEmail(Sender, Recipient, Cc, Bcc, Subject, Content, Format, Charset, Options, Attachments, EmbeddedImages, Mailer);
            if (!send)
                SendError = ClientSendError; // Send error description
            return send;
        }

        // Send email
        public async Task<bool> SendAsync()
        {
            bool send = await SendEmailAsync(Sender, Recipient, Cc, Bcc, Subject, Content, Format, Charset, Options, Attachments, EmbeddedImages, Mailer);
            if (!send)
                SendError = ClientSendError; // Send error description
            return send;
        }
    }
} // End Partial class
