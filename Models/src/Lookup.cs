namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Lookup class
    /// </summary>
    public class Lookup<T>
        where T : DbField
    {
        public string LookupType = "";

        public Dictionary<string, Dictionary<string, object>> Options = new ();

        public string Template = "";

        public string CurrentFilter = "";

        public string UserSelect = "";

        public string UserFilter = "";

        public string UserOrderBy = "";

        public List<string> FilterValues = new ();

        public string SearchValue = "";

        private string _searchExpression = "";

        public int PageSize = -1;

        public int Offset = -1;

        public string ModalLookupSearchType = "AND";

        public bool KeepCrLf = false;

        public Func<string>? LookupFilter;

        public string RenderViewFunc = "RenderListRow";

        public string RenderEditFunc = "RenderEditRow";

        public string Name = "";

        public string LinkTable = "";

        public bool Distinct = false;

        public string LinkField = "";

        public List<string> DisplayFields;

        public string GroupByField = "";

        public string GroupByExpression = "";

        public List<string> ParentFields = new ();

        public List<string> ChildFields = new ();

        public Dictionary<string, string> FilterFields = new ();

        public List<string> FilterFieldVars = new ();

        public List<string> AutoFillSourceFields = new ();

        public List<string> AutoFillTargetFields = new ();

        public dynamic? Table = null;

        public bool FormatAutoFill = false;

        public bool UseParentFilter = false;

        public bool LookupAllDisplayFields = false;

        public bool SelectOffset = true; // DN // Handle SelectOffset

        public bool UseHtmlDecode = true;

        /// <summary>
        /// Constructor for the Lookup class
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="linkTable">Link table</param>
        /// <param name="distinct">Distinct</param>
        /// <param name="linkField">Link field</param>
        /// <param name="displayFields">Display fields</param>
        /// <param name="groupByField">Group by field</param>
        /// <param name="groupByExpression">Group by expression</param>
        /// <param name="parentFields">Parent fields</param>
        /// <param name="childFields">Child fields</param>
        /// <param name="filterFields">Filter fields</param>
        /// <param name="filterFieldVars">Filter field variables</param>
        /// <param name="autoFillSourceFields">AutoFill source fields</param>
        /// <param name="autoFillTargetFields">AutoFill target fields</param>
        /// <param name="orderBy">Order By clause</param>
        /// <param name="template">Option template</param>
        /// <param name="searchExpression">Search expression</param>
        public Lookup(
            string name,
            string linkTable,
            bool distinct,
            string linkField,
            List<string> displayFields,
            string groupByField = "",
            string groupByExpression = "",
            List<string>? parentFields = null,
            List<string>? childFields = null,
            List<string>? filterFields = null,
            List<string>? filterFieldVars = null,
            List<string>? autoFillSourceFields = null,
            List<string>? autoFillTargetFields = null,
            string orderBy = "",
            string template = "",
            string searchExpression = ""
        )
        {
            Name = name;
            LinkTable = linkTable;
            Distinct = distinct;
            LinkField = linkField;
            DisplayFields = displayFields;
            GroupByField = groupByField;
            GroupByExpression = groupByExpression;
            ParentFields = parentFields ?? ParentFields;
            ChildFields = childFields ?? ChildFields;
            if (filterFields != null)
                FilterFields = filterFields.ToDictionary(f => f, f => "="); // Default filter operator
            FilterFieldVars = filterFieldVars ?? FilterFieldVars;
            AutoFillSourceFields = autoFillSourceFields ?? AutoFillSourceFields;
            AutoFillTargetFields = autoFillTargetFields ?? AutoFillTargetFields;
            UserOrderBy = orderBy;
            Template = template;
            _searchExpression = searchExpression;
            LookupAllDisplayFields = Config.LookupAllDisplayFields;
        }

        /// <summary>
        /// Get lookup SQL based on current filter/lookup filter, call Lookup Selecting if necessary
        /// </summary>
        /// <param name="useParentFilter">Use parent filter</param>
        /// <param name="currentFilter">Current filter</param>
        /// <param name="lookupFilter">Lookup filter</param>
        /// <param name="page">Page object</param>
        /// <param name="skipFilterFields">Skip filter fields</param>
        /// <param name="clearUserFilter">Clear user filter</param>
        /// <returns>Lookup SQL</returns>
        public string GetSql(bool useParentFilter = true, string currentFilter = "", Func<string>? lookupFilter = null, dynamic? page = null, bool skipFilterFields = false, bool clearUserFilter = false)
        {
            UseParentFilter = useParentFilter; // Save last call
            CurrentFilter = currentFilter;
            LookupFilter = lookupFilter; // Save last call
            if (clearUserFilter)
                UserFilter = "";
            if (!Empty(page)) {
                string filter = GetUserFilter(useParentFilter);
                string newFilter = filter;
                DbField? fld = page?.Fields[Name];
                if (fld != null)
                    Invoke(page, "LookupSelecting", new object[] { fld, newFilter }); // Call Lookup Selecting
                if (filter != newFilter) // Filter changed
                    AddFilter(ref UserFilter, newFilter);
            }
            if (lookupFilter != null) // Add lookup filter as part of user filter
                AddFilter(ref UserFilter, lookupFilter()); // DN
            string sql = GetSqlPart("", true, useParentFilter, skipFilterFields);
            return sql;
        }

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">Dictionary&lt;string, Dictionary&lt;string, object&gt;&gt;</param>
        /// <returns></returns>
        public void SetOptions(Dictionary<string, Dictionary<string, object>> options) => Options = options;

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">
        /// Options, e.g. new Dictionary&lt;string, List&lt;object&gt;&gt; {
        /// 	{"lv1", new List&lt;object&gt; {"lv1", "dv", "dv2", "dv3", "dv4"}},
        /// 	{"lv2", new List&lt;object&gt; {"lv2", "dv", "dv2", "dv3", "dv4"}}
        /// }
        /// </param>
        /// <returns>Whether the options are setup successfully</returns>
        public void SetOptions(Dictionary<string, List<object>> options)
        {
            var keys = new List<string> { "lf", "df", "df2", "df3", "df4", "ff", "ff2", "ff3", "ff4" };
            Options = options.Where(kvp => kvp.Value.Count() >= 2)
                .ToDictionary(kvp => kvp.Key, kvp => keys.Zip(kvp.Value, (key, values) => (key, values)).ToDictionary(t => t.Item1, t => t.Item2));
        }

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">
        /// Options. List of dictionary returned by ExecuteRows()
        /// </param>
        /// <returns>Whether the options are setup successfully</returns>
        public void SetOptions(List<Dictionary<string, object>> options) =>
            SetOptions(options.ToDictionary(d => ConvertToString(d.Values.First()), d => d.Values.ToList()));

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">
        /// Options, e.g. new List&lt;List&lt;object&gt;&gt; {
        /// 	new List&lt;object&gt; {"1", "option1", "df1", "df2", "df3" },
        /// 	new List&lt;object&gt; {"2", "option2", "df1", "df2" },
        /// 	new List&lt;object&gt; {"3", "option3", "df1", "df3", "df3a" },
        /// }
        /// </param>
        /// <returns>Whether the options are setup successfully</returns>
        public void SetOptions(List<List<object>> options) => SetOptions(options.ToDictionary(list => ConvertToString(list[0]), list => list));

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">
        /// Options, e.g. new List&lt;object[]&gt; {
        /// 	new object[] {"1", "option1", "df1", "df2", "df3" },
        /// 	new object[] {"2", "option2", "df1", "df2" },
        /// 	new object[] {"3", "option3", "df1", "df3", "df3a" },
        /// }
        /// </param>
        /// <returns>Whether the options are setup successfully</returns>
        public void SetOptions(List<object[]> options) => SetOptions(options.ToDictionary(list => ConvertToString(list[0]), list => list.ToList()));

        /// <summary>
        /// Set filter field operator
        /// </summary>
        /// <param name="name">Filter field name</param>
        /// <param name="opr">Filter search operator</param>
        public void SetFilterOperator(string name, string opr)
        {
            if (FilterFields.ContainsKey(name) && IsValidOperator(opr))
                FilterFields[name] = opr;
        }

        /// <summary>
        /// Get user parameters as hidden tag
        /// </summary>
        /// <param name="currentPage">Current page object</param>
        /// <param name="name">variable name</param>
        /// <returns>The hidden tag HTML markup</returns>
        public string GetParamTag(dynamic currentPage, string name)
        {
            UserSelect = "";
            UserFilter = "";
            UserOrderBy = "";
            string sql = GetSql(UseParentFilter, CurrentFilter, LookupFilter, currentPage); // Call Lookup Selecting again based on last setting
            var ar = new Dictionary<string, string>();
            if (!Empty(UserSelect))
                ar.Add("s", Encrypt(UserSelect));
            if (!Empty(UserFilter))
                ar.Add("f", Encrypt(UserFilter));
            if (!Empty(UserOrderBy))
                ar.Add("o", Encrypt(UserOrderBy));
            if (ar.Count > 0)
                return "<input type=\"hidden\" id=\"" + name + "\" name=\"" + name + "\" value=\"" + String.Join("&", ar.Select(kvp => kvp.Key + "=" + kvp.Value)) + "\">";
            return "";
        }

        /// <summary>
        /// Output properties as dictionary
        /// </summary>
        /// <param name="currentPage">Current page</param>
        /// <returns>Dictionary</returns>
        public Dictionary<string, object> ToClientList(dynamic currentPage)
        {
            string pageName = currentPage?.PageObjName.Length > 0 ? currentPage.PageObjName : "";
            string pagePrefix = pageName.Length > 0 && pageName.Substring(0, 1) == "_" ? "_" : ""; // Handle table start with "_"
            pageName = pageName.Substring(pagePrefix.Length);
            pageName = pageName.Length > 0
                ? (pageName.Substring(0, 1).ToUpperInvariant() == pageName.Substring(0, 1) ? "_" + pageName.Substring(0, 1) : pageName.Substring(0, 1).ToUpperInvariant()) + pageName.Substring(1)
                : ""; // CamelCase to PascalCase // DN
            return new () {

                ["page"] = pagePrefix + pageName,
                ["field"] = Name,
                ["linkField"] = LinkField,
                ["displayFields"] = DisplayFields,
                ["groupByField"] = GroupByField,
                ["parentFields"] = currentPage?.PageID != "grid" && HasParentTable() ? new List<string>() : ParentFields,
                ["childFields"] = ChildFields,
                ["filterFields"] = currentPage?.PageID != "grid" && HasParentTable() ? new List<string>() : FilterFields.Keys,
                ["filterFieldVars"] = currentPage?.PageID != "grid" && HasParentTable() ? new List<string>() : FilterFieldVars,
                ["autoFillTargetFields"] = AutoFillTargetFields,
                ["ajax"] = !Empty(LinkTable),
                ["template"] = Template
            };
        }

        /// <summary>
        /// Execute SQL and output JSON response
        /// </summary>
        /// <returns>The lookup result as JSON</returns>
        public virtual async Task<Dictionary<string, object>> ToJson(dynamic? page = null)
        {
            Dictionary<string, object> emptyResult = new ();
            if (page == null)
                return emptyResult;

            // Get table object
            var tbl = GetTable();
            if (tbl == null)
                return emptyResult;

            // Check if lookup to report source table
            bool isReport = page is ReportTable report && (new List<string> { report.ReportSourceTable, report.TableVar }).Contains(tbl.TableVar);
            var renderer = isReport ? page : tbl;

            // Update expression for grouping fields (reports)
            if (isReport) {
                foreach (string displayField in DisplayFields) {
                    if (!Empty(displayField)) {
                        DbField? pageDisplayField = page?.Fields[displayField];
                        DbField? tblDisplayField = tbl?.Fields[displayField];
                        if (pageDisplayField != null && tblDisplayField != null && !Empty(pageDisplayField.LookupExpression)) {
                            if (!Empty(UserOrderBy))
                                UserOrderBy = UserOrderBy.Replace(tblDisplayField.Expression, pageDisplayField.LookupExpression);
                            tblDisplayField.Expression = pageDisplayField.LookupExpression;
                            Distinct = true; // Use DISTINCT for grouping fields
                        }
                    }
                }
            }
            bool useParentFilter = FilterValues.Count > 0 && FilterValues.Skip(1).All(fv => !Empty(fv)) || !HasParentTable() && LookupType != "filter";
            string sql = GetSql(useParentFilter, "", null, page, !useParentFilter);
            string orderBy = UserOrderBy;
            int pageSize = PageSize;
            int offset = Offset;
            List<Dictionary<string, object>>? rs = await GetRecordset(sql, orderBy, pageSize, offset);
            if (!IsList(rs))
                return emptyResult;
            int totalCnt = pageSize > 0 ? await tbl.TryGetRecordCountAsync(sql) : rs.Count;
            // Skip offset for MSSQL 2008 // DN
            if (offset > 0 && !SelectOffset)
                rs.RemoveRange(0, offset);
            // Output
            for (int j = 0; j < rs.Count; j++) {
                Dictionary<string, object> row = rs[j].ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
                var keys = ((IEnumerable<string>)row.Keys).ToList();
                T field;
                string key = keys[0];
                if (tbl.Fields.ContainsKey(LinkField)) {
                    field = tbl.Fields[LinkField];
                    field.SetDbValue(row[key]);
                }
                for (int i = 1; i < row.Count; i++) {
                    key = keys[i];
                    if (row[key] is string str && !KeepCrLf) {
                        str = str.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                        row[key] = str;
                    }
                    if (SameText(LookupType, "autofill")) {
                        if (renderer.Fields.ContainsKey(AutoFillSourceFields[i - 1])) {
                            field = renderer.Fields[AutoFillSourceFields[i - 1]];
                            field.SetDbValue(row[key]);
                        }
                    }
                }
                if (SameText(LookupType, "autofill")) {
                    if (FormatAutoFill) { // Format auto fill
                        renderer.RowType = RowType.Edit;
                        var mi = renderer.GetType().GetMethod(RenderEditFunc);
                        if (mi != null)
                            await (Task)mi.Invoke(renderer, null);
                        for (int i = 1; i < row.Count; i++) {
                            key = keys[i];
                            if (renderer.Fields.ContainsKey(AutoFillSourceFields[i - 1])) {
                                field = renderer.Fields[AutoFillSourceFields[i - 1]];
                                row[key] = mi == null || field.AutoFillOriginalValue
                                    ? IsNull(field.CurrentValue) ? DbNullValue : field.CurrentValue
                                    : IsList(field.EditValue) || field.EditValue == null
                                    ? IsNull(field.CurrentValue) ? DbNullValue : field.CurrentValue
                                    : field.EditValue;
                            }
                        }
                    }
                } else if (LookupType != "unknown") { // Format display fields for known lookup type
                    row = RenderViewRow(row, renderer);
                }
                rs[j] = row;
            }

            // Set up advanced filter (reports)
            if (isReport) {
                if ((new[] { "updateoption", "modal", "autosuggest" }).Contains(LookupType)) {
                    if (page != null)
                        Invoke(page, "PageFilterLoad");
                    DbField? linkField = page?.Fields[LinkField];
                    if (linkField != null && IsList(linkField.AdvancedFilters)) {
                        foreach (var filter in linkField.AdvancedFilters) {
                            if (filter.Enabled)
                                rs.Add(new () { { "lf", filter.ID }, { "df", filter.Name } });
                        }
                    }
                }
            }
            Dictionary<string, object> result = new () { {"result", "OK"}, {"records", rs}, {"totalRecordCount", totalCnt}, {"version", Config.ProductVersion} };
            if (Config.Debug)
                result["sql"] = sql;
            return result;
        }

        private bool rendering = false;

        /// <summary>
        /// Render view row
        /// </summary>
        /// <param name="row">Input data</param>
        /// <param name="renderer">Whether the UserSelect, CurrentFilter and UserFilter should be used</param>
        /// <returns>Output data</returns>
        public dynamic RenderViewRow(dynamic row, dynamic? renderer = null)
        {
            if (rendering) // Avoid recursive calls
                return row;

            // Use table as renderer if not defined
            bool sameTable = false;
            if (renderer == null)
                renderer = GetTable(true); // Create instance so that Field properties will not be overriden
            else if (renderer.Name == LinkTable)
                sameTable = true;
            if (renderer == null)
                return row;
            var mi = renderer.GetType().GetMethod(RenderViewFunc);
            if (mi == null)
                return row;
            rendering = true;

            // Handle Dictionary
            if (row is Dictionary<string, object> dict) {
                // Set up DbValue / CurrentValue
                var keys = ((IEnumerable<string>)row.Keys).ToList();
                for (var i = 1; i < row.Count; i++) {
                    var key = keys[i];
                    string? displayField = DisplayFields.ElementAtOrDefault(i - 1);
                    if (!Empty(displayField) && renderer.Fields.ContainsKey(displayField)) {
                        var field = renderer.Fields[displayField];
                        field.SetDbValue(row[key]);
                    }
                }

                // Convert DateTime as Link Field // DN
                string lf = ConvertToString(keys[0]);
                if (row[lf] is DateTime)
                    row[lf] = ConvertToString(row[lf]);

                // Render data
                RowType rowType = renderer.RowType; // Save RowType
                renderer.RowType = RowType.View;
                mi.Invoke(renderer, null);
                renderer.RowType = rowType; // Restore RowType

                // Output data from ViewValue
                for (int i = 1; i < row.Count; i++) {
                    var key = keys[i];
                    string? name = DisplayFields.ElementAtOrDefault(i - 1);
                    if (!Empty(name) && renderer.Fields.ContainsKey(name)) {
                        var field = renderer.Fields[name];
                        if (mi != null && !Empty(field.ViewValue) && !(sameTable && name == Name)) {
                            row[key] = field.ViewValue;
                        }
                    }
                }

            // Handle List
            } else if (row is List<object>) {
                // Set up DbValue / CurrentValue
                for (var i = 1; i < row.Count; i++) {
                    string? displayField = DisplayFields.ElementAtOrDefault(i - 1);
                    if (!Empty(displayField) && renderer.Fields.ContainsKey(displayField)) {
                        var field = renderer.Fields[displayField];
                        field.SetDbValue(row[i]);
                    }
                }

                // Render data
                RowType rowType = renderer.RowType; // Save RowType
                renderer.RowType = RowType.View;
                mi.Invoke(renderer, null);
                renderer.RowType = rowType; // Restore RowType

                // Output data from ViewValue
                for (int i = 1; i < row.Count; i++) {
                    string? name = DisplayFields.ElementAtOrDefault(i - 1);
                    if (!Empty(name) && renderer.Fields.ContainsKey(name)) {
                        var field = renderer.Fields[name];
                         // Make sure that ViewValue is not empty and not self lookup field (except Date/Time) and not field with user values
                        if (mi != null && !Empty(field.ViewValue) && !(sameTable && name == Name && !(new List<DataType> { DataType.Date, DataType.Time }).Contains(field.DataType) && field.OptionCount == 0))
                            row[i] = field.ViewValue;
                    }
                }
            }
            rendering = false;
            return row;
        }

        public bool HasParentTable() => ParentFields.Any(pf => !Empty(pf) && pf.Contains(" "));

        /// <summary>
        /// Return a part of the lookup SQL
        /// </summary>
        /// <param name="part">The part of the SQL: "select", "where", "orderby", or "" (the complete SQL)</param>
        /// <param name="isUser">Whether the UserSelect, CurrentFilter and UserFilter should be used</param>
        /// <param name="useParentFilter">Whether the ParentFilter should be used</param>
        /// <param name="skipFilterFields">Whether the filter fields should be skipped</param>
        /// <returns>The lookup SQL</returns>
        protected string GetSqlPart(string part = "", bool isUser = true, bool useParentFilter = true, bool skipFilterFields = false)
        {
            var tbl = GetTable();
            if (tbl == null)
                return "";

            // Set up SELECT ... FROM ...
            string dbid = tbl.DbId;
            string select = "SELECT";
            if (Distinct)
                select += " DISTINCT";

            // Set up link field
            if (!tbl.Fields.TryGetValue(LinkField, out T linkField))
                return "";

            // Set up group by field
            var groupByField = tbl.FieldByName(GroupByField);
            select += " " + linkField.Expression;
            if (LookupType != "unknown") // Known lookup types
                select += " AS " + QuotedName("lf", dbid);

            // Set up lookup fields
            var lookupCnt = 0;
            if (SameText(LookupType, "autofill")) {
                for (int i = 0; i < AutoFillSourceFields.Count; i++) {
                    var autoFillSourceField = AutoFillSourceFields[i];
                    if (!tbl.Fields.TryGetValue(autoFillSourceField, out T af)) {
                        select += ", '' AS " + QuotedName("af" + i, dbid);
                    } else {
                        select += ", " + af.Expression + " AS " + QuotedName("af" + i, dbid);
                        if (!af.AutoFillOriginalValue) {
                            FormatAutoFill = true;
                        }
                    }
                    lookupCnt++;
                }
            } else {
                for (int i = 0; i < DisplayFields.Count; i++) {
                    var displayField = DisplayFields[i];
                    if (!tbl.Fields.TryGetValue(displayField, out T df)) {
                        select += ", '' AS " + QuotedName("df" + ((i == 0) ? "" : ConvertToString(i + 1)), dbid);
                    } else {
                        select += ", " + df.Expression;
                        if (LookupType != "unknown") // Known lookup types
                            select += " AS " + QuotedName("df" + ((i == 0) ? "" : ConvertToString(i + 1)), dbid);
                    }
                    lookupCnt++;
                }
                if (!useParentFilter && !skipFilterFields) {
                    int i = 0;
                    foreach (var (key, value) in FilterFields) {
                        if (!tbl.Fields.TryGetValue(key, out T filterField)) {
                            select += ", '' AS " + QuotedName("ff" + ((i == 0) ? "" : ConvertToString(i + 1)), dbid);
                        } else {
                            var filterOpr = value;
                            select += ", " + filterField.Expression;
                            if (LookupType != "unknown") // Known lookup types
                                select += " AS " + QuotedName("ff" + ((i == 0) ? "" : ConvertToString(i + 1)), dbid);
                        }
                        i++;
                        lookupCnt++;
                    }
                }
                if (groupByField != null) {
                    select += ", " + GroupByExpression;
                    if (LookupType != "unknown") // Known lookup types
                        select += " AS " + QuotedName("gf", dbid);
                }
            }
            if (lookupCnt == 0)
                return "";
            select += " FROM " + tbl.SqlFrom;

            // User SELECT
            if (UserSelect != "" && isUser)
                select = UserSelect;

            // Set up WHERE
            string where = ConvertToString(tbl.Invoke("ApplyUserIDFilters", new object[] { "", "lookup" }));

            // Set up current filter
            int cnt = FilterValues.Count;
            if (cnt > 0) {
                var val = FilterValues[0];
            if (!Empty(val)) {
                if (linkField.DataType == DataType.Guid && !CheckGuid(val))
                    AddFilter(ref where, "1=0"); // Disallow
                else
                    AddFilter(ref where, GetFilter(linkField, "=", val, tbl.DbId));
            }

            // Set up parent filters
            if (useParentFilter) {
                    int i = 1;
                    foreach (var (key, value) in FilterFields) {
                        if (!Empty(key)) {
                            var filterOpr = value;
                            if (!tbl.Fields.TryGetValue(key, out T filterField))
                                return "";
                            if (cnt <= i) {
                                AddFilter(ref where, "1=0"); // Disallow
                            } else {
                                val = ConvertToString(FilterValues[i]);
                                AddFilter(ref where, GetFilter(filterField, filterOpr, val, tbl.DbId));
                            }
                        }
                        i++;
                    }
                }
            }

            // Set up search
            if (!Empty(SearchValue)) {
                // Normal autosuggest
                if (SameText(LookupType, "autosuggest") && !LookupAllDisplayFields)
                    AddFilter(ref where, GetAutoSuggestFilter(SearchValue));
                else // Use quick search logic
                    AddFilter(ref where, GetModalSearchFilter(SearchValue, tbl.DbId));
            }

            // Add filters
            if (!Empty(CurrentFilter) && isUser)
                AddFilter(ref where, CurrentFilter);

            // User Filter
            if (!Empty(UserFilter) && isUser)
                AddFilter(ref where, UserFilter);

            // Set up ORDER BY
            string orderBy = UserOrderBy;
            if (groupByField != null) { // Sort GroupByField first
                string groupByExpression = GroupByExpression.StartsWith("(") && GroupByExpression.EndsWith(")")
                    ?  QuotedName("gf", dbid)
                    : GroupByExpression;
                orderBy = groupByExpression + " ASC" + (Empty(orderBy) ? "" : ", " + orderBy);
            }

            // Return SQL part
            if (part == "select") {
                return select;
            } else if (part == "where") {
                return where;
            } else if (part == "orderby") {
                return orderBy;
            } else {
                string sql = select;
                string dbType = GetConnectionType(tbl.DbId);
                if (!Empty(where))
                    sql += " WHERE " + where;
                if (!Empty(orderBy))
                    if (dbType == "MSSQL")
                        sql += " /*BeginOrderBy*/ORDER BY " + orderBy + "/*EndOrderBy*/";
                    else
                        sql += " ORDER BY " + orderBy;
                return sql;
            }
        }

        /// <summary>
        /// Get filter
        /// </summary>
        /// <param name="fld">Search field object</param>
        /// <param name="opr">Search operator</param>
        /// <param name="val">Search value</param>
        /// <param name="dbid">Database ID</param>
        /// <returns>Search filter (SQL "where" part)</returns>
        protected string GetFilter(T fld, string opr, string val, string dbid)
        {
            bool valid = !Empty(val);
            string[] values = val.Split(Config.MultipleOptionSeparator);
            if (fld.DataType == DataType.Number) // Validate numeric fields
                valid = values.All(value => IsNumeric(value));
            if (valid) { // Allow
                if (opr == "=") { // Use the IN operator
                    var result = values.Select(value => QuotedValue(value, fld.DataType, dbid));
                    return fld.Expression + " IN (" + String.Join(", ", result) + ")";
                } else { // Custom operator
                    return values.Aggregate("", (where, val) => {
                        string fldOpr, filter;
                        if ((new[] {"LIKE", "NOT LIKE", "STARTS WITH", "ENDS WITH"}).Contains(opr)) {
                            fldOpr = (opr == "NOT LIKE") ? "NOT LIKE" : "LIKE";
                            filter = LikeOrNotLike(fldOpr, QuotedValue(val, fld.DataType, dbid, opr), dbid);
                        } else {
                            fldOpr = opr;
                            val = QuotedValue(val, fld.DataType, dbid);
                            filter = fld.Expression + fldOpr + val;
                        }
                        return AddFilter(where, filter);
                    });
                }
            } else {
                return "1=0"; // Disallow
            }
        }

        /// <summary>
        /// Get user filter
        /// </summary>
        /// <param name="useParentFilter">Use parent filter or not</param>
        /// <returns>SQL WHERE clause</returns>
        protected string GetUserFilter(bool useParentFilter = false) =>
            GetSqlPart("where", false, useParentFilter);

        /// <summary>
        /// Get table object
        /// </summary>
        /// <param name="create">Create new instance</param>
        /// <returns>Instance of table object</returns>
        public virtual dynamic? GetTable(bool create = false)
        {
            if (Empty(LinkTable))
                return null;
            if (Table == null) {
                Table = GetPropertyValue(LinkTable) ?? Resolve(LinkTable);
                if (Table != null) {
                    if (Table.Type == "REPORT" && !Empty(Table.ReportSourceTableName)) // Use source table // DN
                        Table = Resolve(Table.ReportSourceTableName);
                }
            }
            if (create && Table != null) // Create new instance
                return CreateInstance(Table!.TableVar);
            return Table;
        }

        /// <summary>
        /// Get recordset
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="orderBy">ORDER BY clause</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of records as dictionary</returns>
        protected async Task<List<Dictionary<string, object>>?> GetRecordset(string sql, string orderBy, int pageSize, int offset)
        {
            var tbl = GetTable();
            if (tbl == null)
                return null;
            var conn = await GetConnection2Async(tbl.DbId);
            if (pageSize > 0) {
                var dbType = GetConnectionType(tbl.DbId);
                SelectOffset = conn.SelectOffset; // Check if SelectOffset is used // DN
                if (SameString(dbType, "MSSQL"))
                    return await conn.GetRowsAsync(await conn.SelectLimit(sql, pageSize, offset, !Empty(orderBy)));
                else
                    return await conn.GetRowsAsync(await conn.SelectLimit(sql, pageSize, offset));
            }
            var dr = await conn.GetDataReaderAsync(sql);
            var rows = await conn.GetRowsAsync(dr);
            dr.Close(); // Make sure that data reader is closed // DN
            return rows;
        }

        /// <summary>
        /// Get/Set search expression
        /// </summary>
        /// <returns>Search expression for auto suggest / modal search</returns>
        protected string SearchExpression
        {
            get => !Empty(_searchExpression)
                ? _searchExpression
                : GetTable()?.Fields[DisplayFields[0]]?.Expression ?? "";
            set => _searchExpression = value;
        }

        /// <summary>
        /// Get Auto-Suggest filter
        /// </summary>
        /// <param name="sv">Search value</param>
        /// <returns>The WHERE clause</returns>
        protected string GetAutoSuggestFilter(string sv) => Table != null
            ? SearchExpression + Like(QuotedValue(sv, DataType.String, Table.DbId, "STARTS WITH"), Table.DbId)
            : "";

        /// <summary>
        /// Get CAST SQL for LIKE operator
        /// </summary>
        /// <param name="sv">Search value</param>
        /// <param name="dbid">Database ID</param>
        /// <returns>The WHERE clause</returns>
        protected string GetModalSearchFilter(string sv, string dbid)
        {
            if (EmptyString(sv))
                return "";
            string search = sv.Trim();
            string searchType = ModalLookupSearchType;
            List<string> keywords = GetQuickSearchKeywords(search, searchType);
            string filter = "";
            foreach (string keyword in keywords) {
                if (!Empty(keyword)) {
                    string thisFilter = SearchExpression + Like(QuotedValue(keyword, DataType.String, dbid, "LIKE"), dbid);
                    AddFilter(ref filter, thisFilter, searchType);
                }
            }
            return filter;
        }
    }
} // End Partial class
