namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// List action class
    /// </summary>
    public class ListAction
    {
        public string Action = "";

        public string Caption = "";

        public bool Allowed = true;

        public string Method = Config.ActionPostback; // Post back (p) / Ajax (a)

        public string Select = Config.ActionMultiple; // Multiple (m) / Single (s)

        public string Message = "";

        public string Icon = "fa-solid fa-star ew-icon"; // Icon

        public string Success = ""; // JavaScript callback function name

        // Constructor
        public ListAction(string action, string caption, bool allowed = true, string method = Config.ActionPostback, string select = Config.ActionMultiple, string message = "", string icon = "fa-solid fa-star ew-icon", string success = "")
        {
            Action = action;
            Caption = caption;
            Allowed = allowed;
            Method = method;
            Select = select;
            Message = message;
            Icon = icon;
            Success = success;
        }

        // To JSON
        public string ToJson(bool htmlencode = false)
        {
            var d = new { msg = Message, action = Action, method = Method, select = Select, success = Success };
            var json = ConvertToJson(d);
            if (htmlencode)
                json = HtmlEncode(json);
            return json;
        }

        // To data-* attributes
        public string ToDataAttrs()
        {
            Attributes attrs = new () {
                { "data-msg", HtmlEncode(Message) },
                { "data-action", HtmlEncode(Action) },
                { "data-method", HtmlEncode(Method) },
                { "data-select", HtmlEncode(Select) },
                { "data-success", HtmlEncode(Success) }
            };
            return attrs.ToString();
        }
    }
} // End Partial class
