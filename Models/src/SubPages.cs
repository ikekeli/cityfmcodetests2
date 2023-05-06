namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Sub pages class
    /// </summary>
    public class SubPages
    {
        public bool Justified = false;

        public bool Fill = false;

        public bool Vertical = false;

        public string Align = ""; // "start" or "center" or "end"

        public string Style = "tabs"; // "tabs" (nav nav-tabs) or "pills" (nav nav-pills) or "" (nav) or "accordion"

        public string Classes = ""; // Other CSS classes

        public string Parent = "false"; // For the data-bs-parent attribute on each .accordion-collapse

        public Dictionary<object, SubPage> Items = new ();

        public List<string> ValidKeys = new ();

        public object? ActiveIndex = null;

        // Get .nav classes
        public string NavClasses
        {
            get {
                string style = "nav";
                if (!Vertical) {
                    style += " nav-" + Style;
                    if (Justified)
                        style += " nav-justified";
                    if (Fill)
                        style += " nav-fill";
                    if ((new[] { "start", "center", "end" }).Contains(Align))
                        style += " justify-content-" + Align;
                } else {
                    if (IsPills) // Vertical does not support tabs
                        style += " nav-" + Style;
                    style += " flex-column me-3";
                }
                if (!Empty(Classes))
                    style += " " + Classes;
                return style;
            }
        }

        // Get .nav-link classes
        public string NavLinkClasses(object k)
        {
            string classes = "nav-link";
            if (IsActive(k)) { // Active page
                classes += " active";
            } else {
                var item = GetItem(k);
                if (item != null) {
                    if (!item.Visible)
                        classes += " d-none ew-hidden";
                    else if (item.Disabled)
                        classes += " disabled ew-disabled";
                }
            }
            return classes;
        }

        // Check if a page is active
        public bool IsActive(object k) => SameString(ActivePageIndex, k);

        // Check if accordion
        public bool IsAccordion => Style == "accordion";

        // Check if tabs
        public bool IsTabs => Style == "tabs";

        // Check if pills
        public bool IsPills => Style == "pills";

        // Check if tabs or pills
        public bool IsTabsOrPills => IsTabs || IsPills;

        // Get container classes (for .nav.lex-column)
        public string ContainerClasses => Vertical ? " d-flex align-items-start" : "";

        // Get tab content classes (for .tab-content)
        public string TabContentClasses => "tab-content" + (Vertical ? " flex-grow-1" : "");

        // Get active classes (for .nav-link if tabs/pills or .accordion-collapse if accordion)
        public string ActiveClasses(object k)
        {
            if (IsActive(k)) { // Active page
                if (IsTabsOrPills) // Tabs/Pills
                    return " active";
                else if (IsAccordion) // Accordion
                    return " show";
            }
            return "";
        }

        // Get .tab-pane classes (for .tab-pane if tabs/pills)
        public string TabPaneClasses(object k)
        {
            string classes = "tab-pane fade";
            if (IsActive(k)) // Active page
                classes += " active show";
            return classes;
        }

        // Get count
        public int Count => Items.Count;

        // Add item by name
        public SubPage Add(object k)
        {
            var item = new SubPage();
            if (!Empty(k))
                Items.Add(k, item);
            return item;
        }

        // Get item by key
        public SubPage? GetItem(object k) => Items.TryGetValue(k, out SubPage? p) ? p : null;

        // Active page index
        public object? ActivePageIndex
        {
            get {
                if (ActiveIndex != null)
                    return ActiveIndex;

                // Return first active page
                object index = Items.FirstOrDefault(kvp => (ValidKeys.Count == 0 || ValidKeys.Contains(kvp.Key)) &&
                    kvp.Value.Visible && !kvp.Value.Disabled && kvp.Value.Active && (!IsNumeric(kvp.Key) || ConvertToInt(kvp.Key) != 0)).Key;
                if (index != null) {
                    ActiveIndex = index;
                    return index;
                }

                // If not found, return first visible page
                index = Items.FirstOrDefault(kvp => (ValidKeys.Count == 0 || ValidKeys.Contains(kvp.Key)) &&
                    kvp.Value.Visible && !kvp.Value.Disabled && (!IsNumeric(kvp.Key) || ConvertToInt(kvp.Key) != 0)).Key;
                if (index != null) {
                    ActiveIndex = index;
                    return index;
                }

                // Not found
                return null;
            }
        }

        // Indexer
        public SubPage? this[object index] => GetItem(index);
    }
} // End Partial class
