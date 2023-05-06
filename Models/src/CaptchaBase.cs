namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Captcha base class // DN
    /// </summary>
    public class CaptchaBase : ICaptcha
    {
        public string FailureMessage { get; set; } = String.Empty;

        public string ResponseField { get; set; } = String.Empty;

        public string Response { get; set; } = String.Empty;

        // Element name
        public virtual string ElementName
        {
            get {
                return ResponseField;
            }
        }

        // Element id
        public virtual string ElementId
        {
            get {
                string id = ResponseField;
                string pageId = CurrentPageID();
                if (!Empty(id) && !Empty(pageId))
                    id += "-" + pageId;
                return id;
            }
        }

        // Session Name
        public virtual string SessionName
        {
            get {
                string name = Config.SessionCaptchaCode;
                string pageId = RouteValues.TryGetValue("PageId", out object? id) ? ConvertToString(id) : CurrentPageID();
                if (!Empty(pageId))
                    name += "_" + pageId;
                return name;
            }
        }

        // HTML tag
        public virtual string GetHtml() => String.Empty;

        // HTML tag for confirm page
        public virtual string GetConfirmHtml() => String.Empty;

        // Validate
        public virtual bool Validate() => true;

        // Client side validation script
        public virtual string GetScript() => String.Empty;

        // Set default failure message
        public virtual void SetDefaultFailureMessage()
        {
            if (Empty(Response))
                FailureMessage = Language.Phrase("EnterValidateCode");
            else
                FailureMessage = Language.Phrase("IncorrectValidationCode");
        }
    }
} // End Partial class
