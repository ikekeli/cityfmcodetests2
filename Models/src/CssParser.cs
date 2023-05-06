namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// CSS parser
    /// </summary>
    public class CssParser
    {
        public Dictionary<string, Dictionary<string, string>> Css = new ();

        // Constructor
        public CssParser()
        {
        }

        // Clear all styles
        public void Clear()
        {
            foreach (var dict in Css.Values)
                dict.Clear();
            Css.Clear();
        }

        // add a section
        public void Add(string key, string codestr)
        {
            key = key.ToLower().Trim();
            if (key == "")
                return;
            codestr = codestr.ToLower();
            if (!Css.ContainsKey(key))
                Css[key] = new ();
            string[] codes = codestr.Split(';');
            if (codes.Length > 0) {
                foreach (string code in codes) {
                    string[] arCode = code.Split(':');
                    string codekey = arCode[0];
                    string codevalue = "";
                    if (arCode.Length > 1)
                        codevalue = arCode[1];
                    if (codekey.Length > 0)
                        Css[key][codekey.Trim()] = codevalue.Trim();
                }
            }
        }

        // explode a string into two
        private (string, string) Explode(string str, char sep)
        {
            string[] ar = str.Split(sep);
            return (ar[0], (ar.Length > 1) ? ar[1] : "");
        }

        // Get a style
        public string Get(string key, string property)
        {
            key = key.ToLower();
            property = property.ToLower();
            string tag = "", subtag = "", cls = "", id = "";
            (tag, subtag) = Explode(key, ':');
            (tag, cls) = Explode(tag, '.');
            (tag, id) = Explode(tag, '#');
            string result = "";
            foreach (var (t, _) in Css) {
                string _tag = t, _subtag = "", _cls = "", _id = "";
                (_tag, _subtag) = Explode(_tag, ':');
                (_tag, _cls) = Explode(_tag, '.');
                (_tag, _id) = Explode(_tag, '#');
                bool tagmatch = (tag == _tag || _tag.Length == 0);
                bool subtagmatch = (subtag == _subtag || _subtag.Length == 0);
                bool classmatch = (cls == _cls || _cls.Length == 0);
                bool idmatch = (id == _id);
                if (tagmatch && subtagmatch && classmatch && idmatch) {
                    string temp = _tag;
                    if (temp.Length > 0 && _cls.Length > 0) {
                        temp += "." + _cls;
                    } else if (temp.Length == 0) {
                        temp = "." + _cls;
                    }
                    if (temp.Length > 0 && _subtag.Length > 0) {
                        temp += ":" + _subtag;
                    } else if (temp.Length == 0) {
                        temp = ":" + _subtag;
                    }
                    if (Css[temp].TryGetValue(property, out string? val))
                        result = val;
                }
            }
            return result;
        }

        // Get section as dictionary
        public Dictionary<string, string> GetSection(string key)
        {
            key = key.ToLower();
            string tag = "", subtag = "", cls = "", id = "";
            (tag, subtag) = Explode(key, ':');
            (tag, cls) = Explode(tag, '.');
            (tag, id) = Explode(tag, '#');
            var result = new Dictionary<string, string>();
            foreach (var (t, _) in Css) {
                string _tag = t, _subtag = "", _cls = "", _id = "";
                (_tag, _subtag) = Explode(_tag, ':');
                (_tag, _cls) = Explode(_tag, '.');
                (_tag, _id) = Explode(_tag, '#');
                bool tagmatch = (tag == _tag || _tag.Length == 0);
                bool subtagmatch = (subtag == _subtag || _subtag.Length == 0);
                bool classmatch = (cls == _cls || _cls.Length == 0);
                bool idmatch = (id == _id);
                if (tagmatch && subtagmatch && classmatch && idmatch) {
                    string temp = _tag;
                    if (temp.Length > 0 && _cls.Length > 0) {
                        temp += "." + _cls;
                    } else if (temp.Length == 0) {
                        temp = "." + _cls;
                    }
                    if (temp.Length > 0 && _subtag.Length > 0) {
                        temp += ":" + _subtag;
                    } else if (temp.Length == 0) {
                        temp = ":" + _subtag;
                    }
                    foreach (var (k, v) in Css[temp])
                        result[k] = v;
                }
            }
            return result;
        }

        // Get section as string
        public string GetSectionString(string key) =>
            GetSection(key).Aggregate("", (result, kvp) => result + kvp.Key + ":" + kvp.Value + ";");

        // Parse string
        public bool ParseString(string str)
        {
            Clear();
            // Remove comments
            str = Regex.Replace(str, @"\/\*(.*)?\*\/", "");
            // Parse the CSS code
            var parts = str.Split('}');
            if (parts.Length > 0) {
                foreach (string part in parts) {
                    var (keystr, codestr) = Explode(part, '{');
                    var keys = keystr.Split(',');
                    keys.Where(key => key.Length > 0).ToList().ForEach(key => {
                        string result = key.Replace("\n", "").Replace("\r", "").Replace("\\", ""); // Only support non-alphanumeric characters
                        Add(result, codestr.Trim());
                    });
                }
            }
            return (Css.Count > 0);
        }

        // Get CSS string
        public string GetCss()
        {
            string result = "";
            foreach (var (k, d) in Css) {
                result += k + " {\n";
                foreach (var (key, value) in d)
                    result += "\t" + key + ": " + value + ";\n";
                result += "}\n\n";
            }
            return result;
        }
    }
} // End Partial class
