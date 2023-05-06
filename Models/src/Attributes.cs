namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Attribute class
    /// </summary>
    public class Attributes : Dictionary<string, object>
    {
        // Constructor
        public Attributes(IDictionary<string, object> d) : base(d) {}

        public Attributes() : base() {}

        public Attributes(object obj) : base(HtmlHelper.AnonymousObjectToHtmlAttributes(obj)) {}

        // Indexer
        public new object this[string key]
        {
            get => TryGetValue(key, out object? value) ? value : "";
            set => base[key] = value;
        }

        // Add
        public new void Add(string key, object value)
        {
            if (SameText(key, "class") || SameText(key, "style"))
                Append(key, ConvertToString(value));
            else
                this[key] = value;
        }

        // Add range
        public void AddRange(IDictionary<string, object>? attrs)
        {
            if (attrs != null) {
                foreach (var (key, value) in attrs)
                    Add(key, value);
            }
        }

        // Merge attributes // *** to be tested
        // public void Merge(Attributes attrs) => base.Concat(attrs).GroupBy(p => p.Key).ToDictionary(g => g.Key, g => g.Last().Value);
        public void Merge(IDictionary<string, object>? attrs) => AddRange(attrs);

        // Append
        public void Append(string key, string value, string sep = "")
        {
            if (SameText(key, "class"))
                AppendClass(value);
            else if (SameText(key, "style"))
                Concat(key, value, ";");
            else
                Concat(key, value, sep);
        }

        // Prepend
        public void Prepend(string key, string value, string sep = "")
        {
            if (SameText(key, "class"))
                PrependClass(value);
            else if (SameText(key, "style"))
                this[key] = Concatenate(value, ConvertToString(this[key]), ";");
            else
                this[key] = Concatenate(value, ConvertToString(this[key]), sep);
        }

        // Concat
        public void Concat(string key, string value, string sep) => this[key] = Concatenate(ConvertToString(this[key]), value, sep);

        /// <summary>
        /// Append CSS class name(s) to the "class" attribute
        /// </summary>
        /// <param name="value">CSS class name(s) to append</param>
        public void AppendClass(string value) => this["class"] = GLOBALS.AppendClass(ConvertToString(this["class"]), value);

        /// <summary>
        /// Prepend CSS class name(s) to the "class" attribute
        /// </summary>
        /// <param name="value">CSS class name(s) to prepend</param>
        public void PrependClass(string value) => this["class"] = GLOBALS.PrependClass(value, ConvertToString(this["class"]));

        /// <summary>
        /// Remove CSS class name(s) from the "class" attribute
        /// </summary>
        public void RemoveClass(string value) => this["class"] = GLOBALS.RemoveClass(ConvertToString(this["class"]), value);

        // Get an attribute value and remove it from dictionary
        public object? Extract(string key)
        {
            if (TryGetValue(key, out object? val)) {
                base.Remove(key);
                return val;
            }
            return null; // Returns null if key does not exist
        }

        // Check attributes for hyperlink
        public void CheckLinkAttributes()
        {
            string onclick = ConvertToString(this["onclick"]);
            string href = ConvertToString(this["href"]);
            if (!Empty(onclick)) {
                if (!Empty(href))
                    this["href"] = "#";
                if (!onclick.StartsWith("return ") && !onclick.EndsWith("return false;"))
                    Append("onclick", "return false;", ";");
            }
        }

        /// <summary>
        /// Is true/false (boolean attributes)
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <returns></returns>
        public bool Is(string name)
        {
            if (IsBooleanAttribute(name))
                return ConvertToBool(this[name]);
            return false;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <param name="exclude">Attributes to be excluded</param>
        /// <returns>HTML attributes</returns>
        public string ToString(List<string>? exclude = null)
        {
            string att = "";
            foreach (var (k, v) in this) {
                string key = k.Trim();
                if (exclude?.Contains(key) ?? false)
                    continue;
                string value = ConvertToString(v).Trim();
                if (IsBooleanAttribute(key) && ConvertToBool(value)) { // Allow boolean attributes, e.g. "disabled"
                    att += " " + key;
                } else if (key != "" && !Empty(value)) {
                    att += " " + key + "=\"" + value + "\"";
                } else if (key == "alt" && Empty(value)) { // Allow alt="" since it is a required attribute
                    att += " alt=\"\"";
                }
            }
            return att;
        }
    }
} // End Partial class
