namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// ListOptions Dictionary
    /// </summary>
    public class ListOptionsDictionary : Dictionary<string, ListOptions>
    {
        // ForEach
        public ListOptionsDictionary ForEach(Action<ListOptions> action)
        {
            Values.ToList().ForEach(action);
            return this;
        }

        // Render all options
        public IHtmlContent Render(string part, string pos = "") =>
            new HtmlString(Values.Aggregate("", (output, opt) => output + opt.RenderHtml(part, pos)));

        // Render all options body
        public IHtmlContent RenderBody(string pos = "") => Render("body", pos);
    }
} // End Partial class
