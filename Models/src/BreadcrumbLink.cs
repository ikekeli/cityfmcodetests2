namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// BreadcrumbLink class
    /// </summary>
    public class BreadcrumbLink
    { // DN

        public string Id = "";

        public string Title = "";

        public string Url = "";

        public string ClassName = "";

        public string TableVar = "";

        public bool IsCurrent;

        // Constructor
        public BreadcrumbLink(string id, string title, string url, string classname, string tablevar = "", bool current = false)
        {
            Id = id;
            Title = title;
            Url = url;
            ClassName = classname;
            TableVar = tablevar;
            IsCurrent = current;
        }
    }
} // End Partial class
