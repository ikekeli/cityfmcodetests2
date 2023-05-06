namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// ASP.NET Page abstract class
    /// </summary>
    public abstract class ZaharuddinPage : IZaharuddinPage, IDisposable
    {
        // Private properties
        private string _message = "";

        private string _failureMessage = "";

        private string _successMessage = "";

        private string _warningMessage = "";

        private string _messageHeading = "";

        private bool _disposed;

        private Lazy<ListOptions> _listOptions;

        private Lazy<ListOptions> _exportOptions;

        private Lazy<ListOptions> _searchOptions;

        private Lazy<ListOptions> _filterOptions;

        private Lazy<ListOptions> _importOptions;

        private Lazy<ListOptionsDictionary> _otherOptions;

        private Lazy<ListActions> _listActions;

        private Lazy<ListOptions> _layoutOptions;

        public string TableVar = "";

        public bool? UseJavascriptMessage = null; // Use JavaScript message

        // Constructor
        public ZaharuddinPage()
        {
            // List options
            _listOptions = new Lazy<ListOptions>(() => new ListOptions { Tag = "td", TableVar = TableVar });

            // Export options
            _exportOptions = new Lazy<ListOptions>(() => new ListOptions { TagClassName = "ew-export-option" });

            // Search options
            _searchOptions = new Lazy<ListOptions>(() => new ListOptions { TagClassName = "ew-search-option" });

            // Filter options
            _filterOptions = new Lazy<ListOptions>(() => new ListOptions { TagClassName = "ew-filter-option" });

            // Import options
            _importOptions = new Lazy<ListOptions>(() => new ListOptions { TagClassName = "ew-import-option" });

            // Layout options
            _layoutOptions = new Lazy<ListOptions>(() => new ListOptions { TagClassName = "ew-layout-option mb-3" });

            // Other options
            _otherOptions = new Lazy<ListOptionsDictionary>(() => new ListOptionsDictionary());

            // List actions
            _listActions = new Lazy<ListActions>(() => new ListActions());
        }

        // List options
        public ListOptions ListOptions => _listOptions.Value;

        // Export options
        public ListOptions ExportOptions => _exportOptions.Value;

        // Search options
        public ListOptions SearchOptions => _searchOptions.Value;

        // Filter options
        public ListOptions FilterOptions => _filterOptions.Value;

        // Import options
        public ListOptions ImportOptions => _importOptions.Value;

        // Other options
        public ListOptionsDictionary OtherOptions => _otherOptions.Value;

        // List actions
        public ListActions ListActions => _listActions.Value;

        // Layout options
        public ListOptions LayoutOptions => _layoutOptions.Value;

        // Overrides Object.Finalize
        ~ZaharuddinPage() => Dispose(false);

        // Releases all resources used by this object
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
        }

        // Message
        public string Message
        {
            get => Session.TryGetValue(Config.SessionMessage, out string? message) ? message : _message;
            set {
                _message = AddMessage(Message, value);
                Session[Config.SessionMessage] = _message;
            }
        }

        // Failure Message
        public string FailureMessage
        {
            get => Session.TryGetValue(Config.SessionFailureMessage, out string? failureMessage) ? failureMessage : _failureMessage;
            set {
                _failureMessage = AddMessage(FailureMessage, value);
                Session[Config.SessionFailureMessage] = _failureMessage;
            }
        }

        // Success Message
        public string SuccessMessage
        {
            get => Session.TryGetValue(Config.SessionSuccessMessage, out string? successMessage) ? successMessage : _successMessage;
            set {
                _successMessage = AddMessage(SuccessMessage, value);
                Session[Config.SessionSuccessMessage] = _successMessage;
            }
        }

        // Warning Message
        public string WarningMessage
        {
            get => Session.TryGetValue(Config.SessionWarningMessage, out string? warningMessage) ? warningMessage : _warningMessage;
            set {
                _warningMessage = AddMessage(WarningMessage, value);
                Session[Config.SessionWarningMessage] = _warningMessage;
            }
        }

        // Message Heading
        public string MessageHeading
        {
            get => Session.TryGetValue(Config.SessionMessageHeading, out string? messageHeading) ? messageHeading : _messageHeading;
            set {
                _messageHeading = value;
                Session[Config.SessionMessageHeading] = _messageHeading;
            }
        }

        // Clear message heading
        public void ClearMessageHeading()
        {
            _messageHeading = "";
            Session[Config.SessionMessageHeading] = _messageHeading;
        }

        // Clear message
        public void ClearMessage()
        {
            _message = "";
            Session[Config.SessionMessage] = _message;
            ClearMessageHeading();
        }

        // Clear failure message
        public void ClearFailureMessage()
        {
            _failureMessage = "";
            Session[Config.SessionFailureMessage] = _failureMessage;
            ClearMessageHeading();
        }

        // Clear success message
        public void ClearSuccessMessage()
        {
            _successMessage = "";
            Session[Config.SessionSuccessMessage] = _successMessage;
            ClearMessageHeading();
        }

        // Clear warning message
        public void ClearWarningMessage()
        {
            _warningMessage = "";
            Session[Config.SessionWarningMessage] = _warningMessage;
            ClearMessageHeading();
        }

        // Clear all messages
        public void ClearMessages()
        {
            ClearMessage();
            ClearFailureMessage();
            ClearSuccessMessage();
            ClearWarningMessage();
        }

        // Get message heading // DN
        private string GetMessageHeading()
        {
            return Empty(MessageHeading) ? "" : "<h5 class=\"alert-heading\">" + MessageHeading + "</h5>";
        }

        // Get all messages as HTML
        public string GetMessage() { // DN
            bool hidden = UseJavascriptMessage ?? Config.UseJavascriptMessage;
            string html = "", message = "";
            object[] args;
            // Message
            args = new object[] { Message, "" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message)) {
                html += "<div class=\"alert alert-info alert-dismissible ew-info\">" + GetMessageHeading() + "<i class=\"icon fa-solid fa-info\"></i>" + message + "</div>";
            }
            // Warning message
            args = new object[] { WarningMessage, "warning" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message)) {
                html += "<div class=\"alert alert-warning alert-dismissible ew-warning\">" + GetMessageHeading() + "<i class=\"icon fa-solid fa-exclamation\"></i>" + message + "</div>";
            }
            // Success message
            args = new object[] { SuccessMessage, "success" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message)) {
                html += "<div class=\"alert alert-success alert-dismissible ew-success\">" + GetMessageHeading() + "<i class=\"icon fa-solid fa-check\"></i>" + message + "</div>";
            }
            // Failure message
            args = new object[] { FailureMessage, "failure" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message)) {
                html += "<div class=\"alert alert-danger alert-dismissible ew-error\">" + GetMessageHeading() + "<i class=\"icon fa-solid fa-ban\"></i>" + message + "</div>";
            }
            ClearMessages();
            if (!Empty(html) && !hidden)
                html = "<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\" aria-label=\"" + Language.Phrase("CloseBtn") + "\"></button>" + html;
            return "<div class=\"ew-message-dialog" + (hidden ? " d-none" : "") + "\">" + html + "</div>"; // DN
        }

        // Get failure message then clear
        public string GetFailureMessage()
        {
            string msg = FailureMessage;
            ClearFailureMessage();
            return msg;
        }

        // Get success message then clear
        public string GetSuccessMessage()
        {
            string msg = SuccessMessage;
            ClearSuccessMessage();
            return msg;
        }

        // Get warning message then clear
        public string GetWarningMessage()
        {
            string msg = WarningMessage;
            ClearWarningMessage();
            return msg;
        }

        // Set failure message
        public string SetFailureMessage(string value) => FailureMessage = value;

        // Set success message
        public string SetSuccessMessage(string value) => SuccessMessage = value;

        // Get warning message
        public string GetWarningMessage(string value) => WarningMessage = value;

        // Show all messages as IHtmlContent
        public IHtmlContent ShowMessages() => new HtmlString(GetMessage());

        // Get all messages as dictionary
        public Dictionary<string, string> GetMessages()
        {
            Dictionary<string, string> d = new ();
            string message = "";
            object[] args;
            // Message heading
            string heading = MessageHeading;
            if (!Empty(heading))
                d.Add("heading", heading);
            // Message
            args = new object[] { Message, "" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message))
                d.Add("message", message);
            // Warning message
            args = new object[] { WarningMessage, "warning" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message))
                d.Add("warningMessage", message);
            // Success message
            args = new object[] { SuccessMessage, "success" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message))
                d.Add("successMessage", message);
            // Failure message
            args = new object[] { FailureMessage, "failure" };
            Invoke(this, "MessageShowing", args);
            message = (string)args[0];
            if (!Empty(message))
                d.Add("failureMessage", message);
            ClearMessages();
            return d;
        }

        public abstract Task<IActionResult> Run();

        public abstract IActionResult Terminate(string url = "");
    }
} // End Partial class
