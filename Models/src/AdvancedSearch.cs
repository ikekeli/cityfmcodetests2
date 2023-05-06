namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Advanced Search class
    /// </summary>
    public class AdvancedSearch
    {
        private string? _searchValue = null; // Search value

        private string _searchOperator = "="; // Search operator

        private string _searchCondition = "AND"; // Search condition

        private string? _searchValue2 = null; // Search value 2

        private string _searchOperator2 = "="; // Search operator 2

        public DbField Field;

        public object? ViewValue = null; // View value

        public object? ViewValue2 = null; // View value 2

        public string? SearchValueDefault = null; // Search value default

        public string SearchOperatorDefault = ""; // Search operator default

        public string SearchConditionDefault = ""; // Search condition default

        public string? SearchValue2Default = null; // Search value 2 default

        public string SearchOperator2Default = ""; // Search operator 2 default

        public bool Raw = false;

        protected string _prefix = "";

        protected string _suffix = "";

        // Constructor
        public AdvancedSearch(DbField fld)
        {
            Field = fld;
            _prefix = Config.ProjectName + "_" + Field.TableVar + "_" + Config.TableAdvancedSearch + "_";
            _suffix = "_" + Field.Param;
            Raw = !Config.RemoveXss;
        }

        // Set SearchValue
        public string SearchValue
        {
            get => _searchValue ?? "";
            set {
                value = Raw ? value : RemoveXss(value);
                _searchValue = value;
            }
        }

        // Set SearchOperator
        public string SearchOperator
        {
            get => _searchOperator;
            set {
                if (IsValidOperator(value))
                    _searchOperator = value;
            }
        }

        // Set SearchCondition
        public string SearchCondition
        {
            get => _searchCondition;
            set {
                value = RemoveXss(value);
                _searchCondition = value;
            }
        }

        // Set SearchValue2
        public string SearchValue2
        {
            get => _searchValue2 ?? "";
            set {
                value = Raw ? value : RemoveXss(value);
                _searchValue2 = value;
            }
        }

        // Set SearchOperator2
        public string SearchOperator2
        {
            get => _searchOperator2;
            set {
                if (IsValidOperator(value))
                    _searchOperator2 = value;
            }
        }

        // Session variable name
        public string GetSessionName(string infix) => _prefix + infix + _suffix;

        // Unset session
        public void UnsetSession()
        {
            Session.Remove(GetSessionName("x"));
            Session.Remove(GetSessionName("z"));
            Session.Remove(GetSessionName("v"));
            Session.Remove(GetSessionName("y"));
            Session.Remove(GetSessionName("w"));
        }

        // Isset session
        public bool IssetSession => Session[GetSessionName("x")] != null || Session[GetSessionName("y")] != null;

        // Get values from dictionary
        public bool Get(IDictionary<string, string> d)
        {
            bool hasValue = false;
            string? value;
            if (d.TryGetValue($"x_{Field.Param}", out value)) {
                SearchValue = value;
                hasValue = true;
            } else if (d.TryGetValue(Field.Param, out value)) { // Support SearchValue without "x_"
                if ((Field.DataType != DataType.String && Field.DataType != DataType.Memo) && !Field.IsVirtual) {
                    ParseSearchValue(value); // Support search format field=<opr><value><cond><value2> (e.g. Field=greater_or_equal1)
                } else {
                    SearchValue = value;
                }
                hasValue = true;
            }
            if (d.TryGetValue($"z_{Field.Param}", out value)) {
                SearchOperator = value;
                hasValue = true;
            }
            if (d.TryGetValue($"v_{Field.Param}", out value)) {
                SearchCondition = value;
                hasValue = true;
            }
            if (d.TryGetValue($"y_{Field.Param}", out value)) {
                SearchValue2 = value;
                hasValue = true;
            }
            if (d.TryGetValue($"w_{Field.Param}", out value)) {
                SearchOperator2 = value;
                hasValue = true;
            }
            return hasValue;
        }

        // Get values from query string
        public bool Get()
        {
            bool hasValue = false;
            StringValues value;
            if (Query.TryGetValue($"x_{Field.Param}", out value)) {
                SearchValue = String.Join(Config.MultipleOptionSeparator, value.ToString());
                hasValue = true;
            } else if (Query.TryGetValue(Field.Param, out value)) { // Support SearchValue without "x_"
                if ((Field.DataType != DataType.String && Field.DataType != DataType.Memo) && !Field.IsVirtual) {
                    ParseSearchValue(value.ToString()); // Support search format field=<opr><value><cond><value2> (e.g. Field=greater_or_equal1)
                } else {
                    SearchValue = value.ToString();
                }
                hasValue = true;
            }
            if (Query.TryGetValue($"z_{Field.Param}", out value)) {
                SearchOperator = value.ToString();
                hasValue = true;
            }
            if (Query.TryGetValue($"v_{Field.Param}", out value)) {
                SearchCondition = value.ToString();
                hasValue = true;
            }
            if (Query.TryGetValue($"y_{Field.Param}", out value)) {
                SearchValue2 = String.Join(Config.MultipleOptionSeparator, value.ToString());
                hasValue = true;
            }
            if (Query.TryGetValue($"w_{Field.Param}", out value)) {
                SearchOperator2 = value.ToString();
                hasValue = true;
            }
            return hasValue;
        }

        // Get values from post
        public bool Post()
        {
            bool hasValue = false;
            StringValues value;
            if (Form.TryGetValue($"x_{Field.Param}", out value)) {
                SearchValue = value.ToString();
                hasValue = true;
            } else if (Form.TryGetValue(Field.Param, out value)) { // Support SearchValue without "x_"
                if ((Field.DataType != DataType.String && Field.DataType != DataType.Memo) && !Field.IsVirtual) {
                    ParseSearchValue(value.ToString()); // Support search format field=<opr><value><cond><value2> (e.g. Field=greater_or_equal1)
                } else {
                    SearchValue = value.ToString();
                }
                hasValue = true;
            }
            if (Form.TryGetValue($"z_{Field.Param}", out value)) {
                SearchOperator = value.ToString();
                hasValue = true;
            }
            if (Form.TryGetValue($"v_{Field.Param}", out value)) {
                SearchCondition = value.ToString();
                hasValue = true;
            }
            if (Form.TryGetValue($"y_{Field.Param}", out value)) {
                SearchValue2 = value.ToString();
                hasValue = true;
            }
            if (Form.TryGetValue($"w_{Field.Param}", out value)) {
                SearchOperator2 = value.ToString();
                hasValue = true;
            }
            return hasValue;
        }

        /// <summary>
        /// Parse search value
        /// - supported format
        /// - {opr}{val} (e.g. >=3)
        /// - {between_opr}{val}|{val2} (e.g.between1|4 = BETWEEN 1 AND 4)
        /// - {opr}{val}|{opr2}{val2} (e.g.greater1|less4 = &gt; 1 AND &lt; 4)
        /// - {opr}{val}||{opr2}{val2} (e.g.less1||greater4 = &lt; 1 OR &gt; 4)
        /// </summary>
        /// <param name="value">Field</param>
        public void ParseSearchValue(string? value)
        {
            if (Empty(value))
                return;
            List<string> oprs = Field.SearchOperators.OrderBy(x => x).ToList();
            List<string> clientOprs = Config.ClientSearchOperators.Where(x => oprs.Contains(x.Key)).Select(x => x.Value).OrderBy(x => x).Reverse().ToList();
            string pattern = "^(" + String.Join('|', oprs) +  ")";
            string clientPattern = "^(" + String.Join('|', clientOprs) + ")";
            string opr = "";
            string val = "";
            Parse(pattern, clientPattern, value, ref opr, ref val);
            if (!Empty(opr) && !Empty(val)) {
                SearchOperator = opr;
                if ((new[] { "BETWEEN", "NOT BETWEEN" }).Contains(opr) && val.Contains(Config.BetweenOperatorValueSeparator)) { // Handle BETWEEN operator
                    string[] arValues = val.Split(Config.BetweenOperatorValueSeparator);
                    SearchValue = arValues[0];
                    SearchValue2 = arValues[1];
                } else if (val.Contains(Config.OrOperatorValueSeparator)) { // Handle OR
                    string[] arValues = val.Split(Config.OrOperatorValueSeparator);
                    SearchValue = arValues[0];
                    SearchCondition = "OR";
                    Parse(pattern, clientPattern, arValues[1], ref opr, ref val);
                    SearchOperator2 = !Empty(opr) ? opr : "=";
                    SearchValue2 = val;
                } else if (val.Contains(Config.BetweenOperatorValueSeparator)) { // Handle AND
                    string[] arValues = val.Split(Config.BetweenOperatorValueSeparator);
                    SearchValue = arValues[0];
                    SearchCondition = "AND";
                    Parse(pattern, clientPattern, arValues[1], ref opr, ref val);
                    SearchOperator2 = !Empty(opr) ? opr : "=";
                    SearchValue2 = val;
                } else {
                    SearchValue  = val;
                }
            } else {
                SearchValue = val;
            }
        }

        private void Parse(string pattern, string clientPattern, string val, ref string opr, ref string parsedValue) // DN
        {
            opr = "";
            parsedValue = val;
            Match? m = Regex.Match(val, pattern);
            if (m.Success) { // Match operators
                opr = m.Groups[1].Value;
                parsedValue = val.Substring(opr.Length);
            } else {
                m = Regex.Match(val, clientPattern);
                if (m.Success) { // Match client operators
                    opr = m.Groups[1].Value;
                    parsedValue = val.Substring(opr.Length);
                    if (Config.ClientSearchOperators.TryGetValue(opr, out string? opr2))
                        opr = opr2;
                }
            }
        }

        // Save to session
        public void Save()
        {
            if (!SameString(Session[GetSessionName("x")], SearchValue))
                Session[GetSessionName("x")] = SearchValue;
            if (!SameString(Session[GetSessionName("y")], SearchValue2))
                Session[GetSessionName("y")] = SearchValue2;
            if (!SameString(Session[GetSessionName("z")], SearchOperator))
                Session[GetSessionName("z")] = SearchOperator;
            if (!SameString(Session[GetSessionName("v")], SearchCondition))
                Session[GetSessionName("v")] = SearchCondition;
            if (!SameString(Session[GetSessionName("w")], SearchOperator2))
                Session[GetSessionName("w")] = SearchOperator2;
        }

        // Load from session
        public void Load()
        {
            SearchValue = ConvertToString(Session[GetSessionName("x")]);
            SearchOperator = ConvertToString(Session[GetSessionName("z")]);
            SearchCondition = ConvertToString(Session[GetSessionName("v")]);
            SearchValue2 = ConvertToString(Session[GetSessionName("y")]);
            SearchOperator2 = ConvertToString(Session[GetSessionName("w")]);
        }

        // Get value (infix = "x|y")
        public string GetValue(string infix = "x") => ConvertToString(Session[GetSessionName(infix)]);

        // Load default values
        public void LoadDefault()
        {
            if (!Empty(SearchValueDefault))
                SearchValue = SearchValueDefault;
            if (!Empty(SearchOperatorDefault))
                SearchOperator = SearchOperatorDefault;
            if (!Empty(SearchConditionDefault))
                SearchCondition = SearchConditionDefault;
            if (!Empty(SearchValue2Default))
                SearchValue2 = SearchValue2Default;
            if (!Empty(SearchOperator2Default))
                SearchOperator2 = SearchOperator2Default;
        }

        // Convert to JSON
        public string ToJson()
        {
            var d = new Dictionary<string, string>();
            string[] oprs = new [] { "IS NULL", "IS NOT NULL", "IS EMPTY", "IS NOT EMPTY" };
            if (!Empty(SearchValue) || !Empty(SearchValue2) || oprs.Contains(SearchOperator) || oprs.Contains(SearchOperator2)) {
                d.Add("x" + _suffix, SearchValue);
                d.Add("z" + _suffix, SearchOperator);
                d.Add("v" + _suffix, SearchCondition);
                d.Add("y" + _suffix, SearchValue2);
                d.Add("w" + _suffix, SearchOperator2);
            }
            return ConvertToJson(d);
        }
    }
} // End Partial class
