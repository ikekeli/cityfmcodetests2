namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Pager item class
    /// </summary>
    public class PagerItem
    {
        public int PageSize;

        public int Start;

        public string Text = "";

        public bool Enabled;

        public string ContextClass = "";

        // Constructor
        public PagerItem(string contextClass, int pageSize, int start = 1, string text = "", bool enabled = false)
        {
            ContextClass = contextClass;
            PageSize = pageSize;
            Start = start;
            Text = text;
            Enabled = enabled;
        }

        // Constructor
        public PagerItem() {}

        /// <summary>
        /// Get page number
        /// </summary>
        /// <returns>Page number</returns>
        public int PageNumber => PageSize > 0 && Start > 0 ? ConvertToInt(Math.Ceiling((double)Start / PageSize)) : 1;

        /// <summary>
        /// Get URL or query string
        /// </summary>
        /// <param name="url">URL without query string</param>
        /// <returns>URL</returns>
        public string GetUrl(string url = "")
        {
            string qs = Config.TablePageNumber + "=" + PageNumber;
            if (DashboardReport)
                qs += "&" + Config.PageDashboard + "=true";
            return !Empty(url) ? UrlAddQuery(url, qs) : qs;
        }

        /// <summary>
        /// Get "disabled" class
        /// </summary>
        /// <returns>Disabled class</returns>
        public string DisabledClass => Enabled ? "" : " disabled";

        /// <summary>
        /// Get "active" class
        /// </summary>
        /// <returns>Active class</returns>
        public string ActiveClass => Enabled ? "" : " active";

        /**
        * Get attributes
        * - data-ew-action and data-url for normal List pages
        * - data-page for other pages
        *
        * @param string $url URL without query string
        * @param string $action Action (redirect/refresh)
        * @return string
        */
        /// <summary>
        /// Get attributes
        /// - data-ew-action and data-url for normal List pages
        /// - data-page for other pages
        /// </summary>
        /// <param name="url">URL without query string</param>
        /// <param name="action">data-ew-action</param>
        /// <returns>Pager item attributes</returns>
        public string GetAttributes(string url = "", string action = "redirect")
        {
            return "data-ew-action=\"" + (Enabled ? action : "none") + "\" data-url=\"" + GetUrl(url) + "\" data-page=\"" + PageNumber + "\"" +
                (!Empty(ContextClass) ? " data-context=\"" + HtmlEncode(ContextClass) + "\"" : "");
        }
    }
} // End Partial class
