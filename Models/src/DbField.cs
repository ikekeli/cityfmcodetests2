namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Field class
    /// </summary>
    public class DbField
    {
        public DbTableBase? Table = null; // DN

        public string TableName = ""; // Table name

        public string TableVar = ""; // Table variable name

        public string SourceTableVar = ""; // Source Table variable name (for Report only)

        public string Name = ""; // Field name

        public string FieldVar = ""; // Field variable name

        public string Param = ""; // Field parameter name

        public string Expression = ""; // Field expression (used in SQL)

        public bool UseBasicSearch = false; // Use basic search

        public string BasicSearchExpression = ""; // Field expression (used in basic search SQL)

        public string LookupExpression = ""; // Lookup expression

        public bool IsCustom = false; // Custom field

        public bool IsVirtual; // Virtual field

        public string VirtualExpression = ""; // Virtual field expression (used in ListSql)

        public bool ForceSelection; // Autosuggest force selection

        public bool IsNativeSelect = false; // Native Select Tag

        public bool SelectMultiple; // Select multiple

        public bool VirtualSearch; // Search as virtual field

        public string ErrorMessage = ""; // Error message

        public string DefaultErrorMessage = ""; // Default error message

        public bool IsInvalid = false; // Invalid

        public object? VirtualValue; // Virtual field value

        public object? TooltipValue; // Field tooltip value

        public int TooltipWidth = 0; // Field tooltip width

        public int Type; // Field type (ADO data type)

        public int Size; // Field size

        public DataType DataType; // Field type (ASP.NET data type)

        public string BlobType = ""; // For Oracle only

        private string _inputTextType = "text"; // Field input text type

        public bool Visible = true; // Visible

        public string ViewTag = ""; // View Tag

        public string HtmlTag = ""; // Html Tag

        public bool IsDetailKey = false; // Field is detail key

        public bool IsAutoIncrement = false; // Autoincrement field (FldAutoIncrement)

        public bool IsPrimaryKey = false; // Primary key (FldIsPrimaryKey)

        public bool IsForeignKey = false; // Foreign key (Master/Detail key)

        public bool IsEncrypt = false; // Field is encrypted

        public bool Raw; // Raw value (save without removing XSS)

        public bool Nullable = true; // Nullable

        public bool Required = false; // Required

        public List<string> SearchOperators = new (); // Search operators

        public Lazy<AdvancedSearch> _advancedSearch; // Advanced Search

        public List<AdvancedFilter> AdvancedFilters = new (); // Advanced Filters

        private Lazy<HttpUpload> _upload; // Upload Object

        public int DateTimeFormat; // Date time format

        private string _formatPattern = ""; // Format pattern // DN

        public string DefaultNumberFormat = ""; // Default number format // NOT USED

        public string CssStyle = ""; // CSS style

        public string CssClass = ""; // CSS class

        public string ImageAlt = ""; // Image alt

        public string ImageCssClass = ""; // Image CSS class

        public int ImageWidth = 0; // Image width

        public int ImageHeight = 0; // Image height

        public bool ImageResize = false; // Image resize

        public bool IsBlobImage = false; // Is blob image

        public string CellCssClass = ""; // Cell CSS class

        public string CellCssStyle = ""; // Cell CSS style

        public string RowCssClass = ""; // Row CSS class

        public string RowCssStyle = ""; // Row CSS style

        public Attributes CellAttrs = new (); // Cell attributes

        public Attributes EditAttrs = new (); // Edit Attributes

        public Attributes LinkAttrs = new (); // Link attributes

        public Attributes RowAttrs = new (); // Row attributes

        public Attributes ViewAttrs = new (); // View Attributes

        public object? EditCustomAttributes;

        public object? LinkCustomAttributes; // Link custom attributes

        public object? ViewCustomAttributes;

        public string CustomMessage = ""; // Custom message

        public string HeaderCellCssClass = ""; // Header cell (<th>) CSS class

        public string FooterCellCssClass = ""; // Footer cell (<td> in <tfoot>) CSS class

        public string MultiUpdate = ""; // Multi update

        public object? OldValue = null; // Old Value

        public object? DefaultValue = null; // Default Value

        public object? ConfirmValue; // Confirm Value

        public object? CurrentValue; // Current value

        public object? ViewValue; // View value

        public object? EditValue; // Edit value

        public object? EditValue2; // Edit value 2 (search)

        public object? HrefValue; // Href value

        public object? ExportHrefValue; // Href value for export

        private string? _formValue; // Form value

        private string? _queryValue; // QueryString value

        protected object _dbValue = DbNullValue; // Database value

        public bool Sortable = true;

        private string _uploadPath = ""; // Upload path // DN

        private string _oldUploadPath = ""; // Old upload path (for deleting old image) // DN

        public string UploadAllowedFileExtensions = ""; // Allowed file extensions

        public int UploadMaxFileSize; // Upload max file size

        public int? UploadMaxFileCount; // Upload max file count

        public bool UploadMultiple = false; // Multiple Upload

        public bool ImageCropper; // Image cropper

        public Lookup<DbField>? Lookup = null;

        public int OptionCount = 0;

        public bool UseLookupCache; // Use lookup cache

        public int LookupCacheCount; // Lookup cache count

        public bool UsePleaseSelect = true;

        public string PleaseSelectText = "";

        public int Count = 0; // Count

        public double Total = 0; // Total

        public string TrueValue = "1";

        public string FalseValue = "0";

        public bool Disabled; // Disabled

        public bool ReadOnly; // ReadOnly

        public int MemoMaxLength = 0; // Maximum length for memo field in list page

        public bool TruncateMemoRemoveHtml; // Remove Html from Memo field

        public string CustomMsg = ""; // Custom message

        public bool IsUpload; // DN

        public bool UseColorbox; // Use Colorbox

        public object DisplayValueSeparator = ", "; // String or String[]

        public bool Exportable = true;

        public bool AutoFillOriginalValue;

        public string RequiredErrorMessage;

        public int DefaultDecimalPrecision = Config.DefaultDecimalPrecision;

        private List<Dictionary<string, object>>? _options;

        public bool UseFilter = false; // Use table header filter

        public bool ExportOriginalValue; // Export original value

        public bool ExportFieldCaption; // Export field caption

        public bool ExportFieldImage; // Export field image

        private string _hrefPath = ""; // Href path (for download)

        private string _placeHolder = "";

        private string? _caption = null;

        public Func<string> GetSelectFilter { get; set; } = () => ""; // DN

        public Func<object?> GetSearchDefault { get; set; } = () => null; // DN

        public Func<object?> GetSearchDefault2 { get; set; } = () => null; // DN

        public Func<object?> GetEditCustomAttributes { get; set; } = () => null; // DN

        public Func<object?> GetViewCustomAttributes { get; set; } = () => null; // DN

        public Func<object?> GetLinkCustomAttributes { get; set; } = () => null; // DN

        public Func<object?> GetAutoUpdateValue { get; set; } = () => null; // DN

        public Func<object?> GetDefault { get; set; } = () => null; // DN

        public Func<object?> GetHiddenValue { get; set; } = () => null; // DN

        public Func<string> GetUploadPath { get; set; } = () => ""; // DN

        public Func<object?> GetServerValidateArguments { get; set; } = () => null; // DN

        public Func<string> GetLinkPrefix { get; set; } = () => ""; // DN

        public Func<string> GetLinkSuffix { get; set; } = () => ""; // DN

        // Constructor // DN
        public DbField(object table, string fieldVar, int type)
        {
            if (table is DbTableBase tableObject) {
                Table = tableObject;
                TableName = Table.Name;
                TableVar = Table.TableVar;
                Raw = Table.Raw;
                _uploadPath = Table.UploadPath;
                _oldUploadPath = Table.OldUploadPath;
                _hrefPath = Table.HrefPath;
                UploadAllowedFileExtensions = Table.UploadAllowedFileExtensions;
                UploadMaxFileSize = Table.UploadMaxFileSize;
                UploadMaxFileCount = Table.UploadMaxFileCount;
                ImageCropper = Table.ImageCropper;
                UseColorbox = Table.UseColorbox;
                AutoFillOriginalValue = Table.AutoFillOriginalValue;
                UseLookupCache = Table.UseLookupCache;
                LookupCacheCount = Table.LookupCacheCount;
                ExportOriginalValue = Table.ExportOriginalValue;
                ExportFieldCaption = Table.ExportFieldCaption;
                ExportFieldImage = Table.ExportFieldImage;
                DefaultNumberFormat = Table.DefaultNumberFormat;
            } else if (table is string tableName) {
                TableName = tableName;
                TableVar = tableName;
            }
            FieldVar = fieldVar;
            Type = type;
            DataType = FieldDataType(Type);
            Param = FieldVar.StartsWith("x_") ? FieldVar.Substring(2) : FieldVar; // Remove "x_" if exists
            _upload = new Lazy<HttpUpload>(() => new HttpUpload(this));
            _advancedSearch = new Lazy<AdvancedSearch>(() => new AdvancedSearch(this));
            RequiredErrorMessage = ResolveLanguage().Phrase("EnterRequiredField");
        }

        // Get table object
        public DbTableBase? GetTable() => Table;

        // Advanced Search Object
        public AdvancedSearch AdvancedSearch => _advancedSearch.Value;

        // Upload Object
        public HttpUpload Upload => _upload.Value;

        // Field parm // DN
        internal string FldParm => FieldVar.Substring(2);

        // Get ICU format pattern // DN
        public string FormatPattern {
            get {
                string fmt = _formatPattern;
                if (Empty(fmt)) {
                    string dtFormat = DateFormat(DateTimeFormat);
                    if (DataType == DataType.Date)
                        fmt = !Empty(dtFormat) ? dtFormat : CurrentDateTimeFormat.ShortDatePattern;
                    else if (DataType == DataType.Time)
                        fmt = !Empty(dtFormat) ? dtFormat : CurrentDateTimeFormat.ShortTimePattern;
                }
                return fmt;
            }
            set => _formatPattern = value;
        }

        // Get client side date/time format pattern
        public string ClientFormatPattern => DataType == DataType.Date || DataType == DataType.Time
            ? Regex.Replace(FormatPattern, @"\b(g+)\b", m => m.Value.ToUpper()) // Make sure "G" in ICU format // C#
            : "";

        // Is boolean field
        public bool IsBoolean => DataType == DataType.Boolean || DataType == DataType.Bit && Size == 1;

        // Is selected for multiple update
        public bool MultiUpdateSelected => MultiUpdate == "1";

        // Field encryption/decryption required
        public bool UseEncryption { // DN
            get {
                bool encrypt = IsEncrypt && (DataType == DataType.String || DataType == DataType.Memo) &&
                    !IsPrimaryKey && !IsAutoIncrement && !IsForeignKey;
                // Do not encrypt username/password/userid/userlevel/profile/activate/email fields in user table
                var fieldNames = new List<string> {
                    Config.LoginUsernameFieldName,
                    Config.LoginPasswordFieldName,
                    Config.UserIdFieldName,
                    Config.ParentUserIdFieldName,
                    Config.UserLevelFieldName,
                    Config.UserProfileFieldName,
                    Config.RegisterActivateFieldName
                };
                if (encrypt && (TableName == Config.UserTableName && fieldNames.Contains(Name)))
                    encrypt = false;
                return encrypt;
            }
        }

        // Upload path // DN
        public string UploadPath
        {
            get => IncludeTrailingDelimiter(_uploadPath, false);
            set => _uploadPath = value;
        }

        // Old upload path // DN
        public string OldUploadPath
        {
            get => IncludeTrailingDelimiter(_oldUploadPath, false);
            set => _oldUploadPath = value;
        }

        // Is BLOB field
        public bool IsBlob => DataType == DataType.Blob;

        // Get Custom Message (help text)
        public string GetCustomMessage
        {
            get {
                string msg = CustomMessage.Trim();
                if (Empty(msg))
                    return "";
                if (Regex.IsMatch(msg, "^<.+>$")) // Html content
                    return msg;
                else
                    return $@"<small id=""{FieldVar}_help"" class=""form-text"">{msg}</small>";
            }
        }

        // Get Input type attribute (TEXT only)
        public string InputTextType
        {
            get => EditAttrs.ContainsKey("type") ? ConvertToString(EditAttrs["type"]) : _inputTextType;
            set => _inputTextType = value;
        }

        // Place holder
        public string PlaceHolder
        {
            get => (ReadOnly || EditAttrs.Is("readonly")) ? "" : _placeHolder;
            set => _placeHolder = value;
        }

        // Field caption
        public string Caption
        {
            get => _caption ?? Language.FieldPhrase(TableVar, Param, "FldCaption");
            set => _caption = value;
        }

        // Field title
        public string Title => Language.FieldPhrase(TableVar, Param, "FldTitle");

        // Field alt
        public string Alt => Language.FieldPhrase(TableVar, Param, "FldAlt");

        // Clear error message
        public void ClearErrorMessage()
        {
            IsInvalid = false;
            ErrorMessage = "";
        }

        // Add error message
        public void AddErrorMessage(string message)
        {
            IsInvalid = true;
            ErrorMessage = AddMessage(ErrorMessage, message);
        }

        // Field error msg
        public string GetErrorMessage(bool required = true)
        {
            string errMsg = ErrorMessage;
            if (Empty(errMsg)) {
                errMsg = Language.FieldPhrase(TableVar, Param, "FldErrMsg");
                if (Empty(errMsg) && !Empty(DefaultErrorMessage))
                    errMsg = DefaultErrorMessage + " - " + Caption;
            }
            if (Required && required)
                errMsg = AddMessage(errMsg, RequiredErrorMessage.Replace("%s", Caption));
            return errMsg;
        }

        // Get is-invalid class
        public string IsInvalidClass => IsInvalid ? " is-invalid" : "";

        // Field option value
        public string TagValue(int i) => Language.FieldPhrase(TableVar, Param, "FldTagValue" + i.ToString());

        // Field option caption
        public string TagCaption(int i) => Language.FieldPhrase(TableVar, Param, "FldTagCaption" + i.ToString());

        // Set field visibility
        public void SetVisibility() => Visible = Table?.GetFieldVisibility(Name) ?? Visible;

        // Check if multiple selection
        public bool IsMultiSelect => SameText(HtmlTag, "SELECT") && SelectMultiple || SameText(HtmlTag, "CHECKBOX") && !IsBoolean && DataType == DataType.String;

        // Set native select
        public void SetNativeSelect(bool value) => IsNativeSelect = value && HtmlTag == "SELECT" && !SelectMultiple; // Select one

        // Set select multiple
        public void SetSelectMultiple(bool value)
        {
            SelectMultiple = value;
            if (!SelectMultiple && Config.UseNativeSelectOne) // Select one
                SetNativeSelect(true);
        }

        // Get highlight value // DN
        public string HighlightValue(object? obj)
        {
            List<string> kwlist = new ();
            if (Table != null && UseBasicSearch && (IsVirtual || (new[] {"NO", "TEXT", "TEXTAREA"}).Contains(HtmlTag))) {
                kwlist = Table.BasicSearch.KeywordList();
                if (Empty(Table.BasicSearch.Type)) // Auto, remove ALL "OR"
                    kwlist.Remove("OR");
            }
            List<string> oprs = new () { "=", "LIKE", "STARTS WITH", "ENDS WITH" }; // Valid operators for highlight
            if (oprs.Contains(AdvancedSearch.GetValue("z"))) {
                string akw = AdvancedSearch.GetValue();
                if (akw.Length > 0)
                    kwlist.Add(akw);
            }
            if (oprs.Contains(AdvancedSearch.GetValue("w"))) {
                string akw = AdvancedSearch.GetValue("y");
                if (akw.Length > 0)
                    kwlist.Add(akw);
            }
            string src = ConvertToString(obj);
            if (kwlist.Count == 0 || Empty(src) || Table != null && !Table.SearchHighlight)
                return src;
            int pos1 = 0;
            int pos2 = 0;
            string val = "";
            string src1 = "";
            foreach (Match match in Regex.Matches(src, @"<([^>]*)>", RegexOptions.IgnoreCase)) {
                pos2 = match.Index;
                if (pos2 > pos1) {
                    src1 = src.Substring(pos1, pos2 - pos1);
                    val += HighlightText(kwlist, src1);
                }
                val += match.Value;
                pos1 = pos2 + match.Value.Length;
            }
            pos2 = src.Length;
            if (pos2 > pos1) {
                src1 = src.Substring(pos1, pos2 - pos1);
                val += HighlightText(kwlist, src1);
            }
            return val;
        }

        // Highlight text fields
        public string HighlightText(List<string> kwlist, string src)
        {
            string pattern = "";
            foreach (var kw in kwlist)
                pattern += (pattern == "" ? "" : "|") + Regex.Escape(kw);
            if (pattern == "")
                return src;
            pattern = "(" + pattern + ")";
            string dest = "<mark class=\"" + PrependClass("mark ew-mark", Table?.HighlightName ?? "") + "\">$1</mark>";
            src = Config.HighlightCompare
                ? Regex.Replace(src, pattern, dest, RegexOptions.IgnoreCase)
                : src = Regex.Replace(src, pattern, dest);
            return src;
        }

        // Highlight matching lookup display value
        public string HighlightLookup(string val, string display)
        {
            List<string> kwlist = new ();
            List<string> oprs = new () { "=", "LIKE", "STARTS WITH", "ENDS WITH" }; // Valid operators for highlight
            if (oprs.Contains(AdvancedSearch.GetValue("z"))) {
                string akw = AdvancedSearch.GetValue();
                if (!Empty(akw))
                    if (IsMultiSelect) // Multiple options
                        kwlist.AddRange(akw.Split(','));
                    else
                        kwlist.Add(akw);
            }
            if (oprs.Contains(AdvancedSearch.GetValue("w"))) {
                string akw = AdvancedSearch.GetValue("y");
                if (!Empty(akw))
                    if (IsMultiSelect) // Multiple options
                        kwlist.AddRange(akw.Split(','));
                    else
                        kwlist.Add(akw);
            }
            if (kwlist.Count == 0 || Table != null && !Table.SearchHighlight || CurrentPageID() != "list" || !kwlist.Contains(val))
                return display;
            else // Matching lookup option
                return "<mark class=\"" + PrependClass("mark ew-mark", Table?.HighlightName ?? "") + "\">" + display + "</mark>";
        }

        // Field lookup cache option
        public OptionValues LookupCacheOption(string? val)
        {
            OptionValues res = new ();
            if (Empty(val) || Empty(Lookup) || Lookup.Options.Count == 0)
                return res;
            if (IsMultiSelect) { // Multiple options
                var arwrk = val.Split(',');
                foreach (string wrk in arwrk) {
                    if (Lookup.Options.TryGetValue(wrk.Trim(), out Dictionary<string, object>? ar)) // Lookup data found in cache
                        res.Add(HighlightLookup(wrk, DisplayValue(ar.Values.ToList())));
                    else
                        res.Add(HighlightLookup(wrk, wrk)); // Not found, use original value
                }
            } else {
                if (Lookup.Options.TryGetValue(val, out Dictionary<string, object>? ar)) // Lookup data found in cache
                    res.Add(HighlightLookup(val, DisplayValue(ar.Values.ToList())));
                else
                    res.Add(HighlightLookup(val, val)); // Not found, use original value
            }
            return res;
        }

        // Has field lookup options
        public bool HasLookupOptions => !Empty(Lookup) && (OptionCount > 0 || Lookup.Options.Count > 0);

        // Lookup options
        public List<Dictionary<string, object>> LookupOptions => Lookup?.Options?.Values.ToList() ?? new ();

        // Field option caption by option value
        public string OptionCaption(string val)
        {
            if (!Empty(Lookup) && Lookup.Options.Count > 0) {
                if (Lookup.Options.TryGetValue(val, out Dictionary<string, object>? ar)) // Lookup data found in cache
                    return DisplayValue(ar.Values.ToList());
            } else {
                for (var i = 0; i < OptionCount; i++) {
                    if (val == TagValue(i + 1)) {
                        val = Empty(TagCaption(i + 1)) ? val : TagCaption(i + 1);
                        break;
                    }
                }
            }
            return val;
        }

        // Get field user options as array
        public List<Dictionary<string, object>> Options(bool pleaseSelect = false)
        { // DN
            List<Dictionary<string, object>> list = new ();
            if (pleaseSelect) // Add "Please Select"
                list.Add(new () { {"lf", ""}, {"df", Language.Phrase("PleaseSelect")} });
            if (!Empty(Lookup) && Lookup.Options.Count > 0) {
                list.Concat(LookupOptions).ToList();
            } else {
                for (var i = 0; i < OptionCount; i++) {
                    string value = TagValue(i + 1);
                    string caption = !Empty(TagCaption(i + 1)) ? TagCaption(i + 1) : value;
                    list.Add(new () { {"lf", value}, {"df", caption} });
                }
            }
            return list;
        }

        // Href path
        public string HrefPath
        {
            get => MapPath(false, !Empty(_hrefPath) ? _hrefPath : UploadPath);
            set => _hrefPath = value;
        }

        // Physical upload path
        public string PhysicalUploadPath => ServerMapPath(UploadPath);

        // Old Physical upload path
        public string OldPhysicalUploadPath => ServerMapPath(OldUploadPath);

        // Get select options HTML // DN
        public virtual IHtmlContent SelectOptionListHtml(string name = "", bool? multiple = null)
        {
            string str = "";
            bool isEmpty = true;
            var curValue = CurrentPage.RowType == RowType.Search ? (name.StartsWith("y") ? AdvancedSearch.SearchValue2 : AdvancedSearch.SearchValue) : CurrentValue;
            bool isMultiple = multiple.HasValue ? multiple.Value : IsMultiSelect;
            string?[] curValues = isMultiple && !Empty(curValue) ? ConvertToString(curValue).Split(',') : new string?[] {};
            if (IsList<Dictionary<string, object>>(EditValue) && !UseFilter) {
                var rows = (List<Dictionary<string, object>>)EditValue;
                if (isMultiple) {
                    foreach (var row in rows) {
                        var list = row.Values.ToList();
                        var selected = false;
                        for (int i = 0; i < curValues.Length; i++) {
                            if (SameString(list[0], curValues[i])) {
                                curValues[i] = null; // Marked for removal
                                selected = true;
                                isEmpty = false;
                                break;
                            }
                        }
                        if (!selected)
                            continue;
                        for (var i = 1; i < list.Count; i++)
                            list[i] = RemoveHtml(ConvertToString(list[i]));
                        str += "<option value=\"" + HtmlEncode(list[0]) + "\" selected>" + DisplayValue(list) + "</option>";
                    }
                } else {
                    if (UsePleaseSelect)
                        str += "<option value=\"\">" + (IsNativeSelect ? Language.Phrase("PleaseSelect") : Language.Phrase("BlankOptionText")) + "</option>"; // Blank option
                    foreach (var row in rows) {
                        var list = row.Values.ToList();
                        if (SameString(curValue, list[0]))
                            isEmpty = false;
                        else
                            continue;
                        for (var i = 1; i < list.Count; i++)
                            list[i] = RemoveHtml(ConvertToString(list[i]));
                        str += "<option value=\"" + HtmlEncode(list[0]) + "\" selected>" + DisplayValue(list) + "</option>";
                    }
                }
            }
            if (isMultiple) {
                for (var i = 0; i < curValues.Length; i++) {
                    if (curValues[i] != null)
                        str += "<option value=\"" + HtmlEncode(curValues[i]) + "\" selected></option>";
                }
            } else {
                if (isEmpty && !Empty(curValue))
                    str += "<option value=\"" + HtmlEncode(curValue) + "\" selected></option>";
            }
            if (isEmpty)
                OldValue = "";
            return new HtmlString(str);
        }

        /// <summary>
        /// Get display field value separator
        /// </summary>
        /// <param name="idx">Display field index (1|2|3)</param>
        /// <returns>The separator for the index</returns>
        protected string? GetDisplayValueSeparator(int idx) {
            if (IsList<string>(DisplayValueSeparator)) {
                var sep = (IList<string>)DisplayValueSeparator;
                return (idx < sep.Count) ? sep[idx - 1] : null;
            } else { // string
                string sep = ConvertToString(DisplayValueSeparator);
                return (sep != "") ? sep : ", ";
            }
        }

        // Get display field value separator as attribute value
        public string DisplayValueSeparatorAttribute =>
            IsList<string>(DisplayValueSeparator) ? ConvertToJson(DisplayValueSeparator) : ConvertToString(DisplayValueSeparator);

        /// <summary>
        /// Get display value (for lookup field)
        /// </summary>
        /// <param name="row">Dictionary/List of object to be displayed</param>
        /// <returns>The display value of the lookup field</returns>
        public string DisplayValue(object? row)
        {
            object v = row switch {
                Dictionary<string, object> dict => dict.Select(kvp => kvp.Value).ToList(),
                List<object> ls => ls,
                string s => s,
                _ => ""
            };
            if (v is List<object> list) {
                if (list.Count == 1)
                    return ConvertToString(list[0]); // Link field
                string val = ConvertToString(list[1]); // Display field 1
                for (int i = 2; i <= 4; i++) { // Display field 2 to 4
                    string? sep = GetDisplayValueSeparator(i - 1);
                    if (sep == null || i >= list.Count) // No separator, break
                        break;
                    if (!Empty(list[i]))
                        val += sep + ConvertToString(list[i]);
                }
                return val;
            }
            return (string)v;
        }

        // Reset attributes for field object
        public void ResetAttributes()
        {
            CssStyle = "";
            CssClass = "";
            CellCssStyle = "";
            CellCssClass = "";
            CellAttrs.Clear();
            EditAttrs.Clear();
            ViewAttrs.Clear();
            LinkAttrs.Clear();
        }

        // View Attributes
        public string ViewAttributes
        {
            get {
                var viewattrs = ViewAttrs;
                if (ViewTag == "IMAGE")
                    viewattrs["alt"] = (ImageAlt.Trim() != "") ? ImageAlt.Trim() : ""; // IMG tag requires alt attribute
                string attrs = ""; // Custom attributes
                if (IsDictionary(ViewCustomAttributes) || IsAnonymousType(ViewCustomAttributes)) // Custom attributes
                    viewattrs.AddRange(ConvertToDictionary<object>(ViewCustomAttributes));
                else
                    attrs = ConvertToString(ViewCustomAttributes);
                viewattrs.AppendClass(CssClass);
                if (ViewTag == "IMAGE" && !Regex.IsMatch(ConvertToString(viewattrs["class"]), @"\bcard-img\b")) {
                    if (ImageWidth > 0 && (!ImageResize || ImageHeight <= 0))
                        viewattrs.Concat("style", "width: " + ImageWidth + "px", ";");
                    if (ImageHeight > 0 && (!ImageResize || ImageWidth <= 0))
                        viewattrs.Concat("style", "height: " + ImageHeight + "px", ";");
                }
                viewattrs.Concat("style", CssStyle, ";");
                string att = viewattrs.ToString();
                if (!Empty(attrs)) // Custom attributes as string
                    att += " " + attrs;
                return att;
            }
        }

        // Edit Attributes
        public string EditAttributes
        {
            get {
                string att = "";
                string style = CssStyle;
                string cls = CssClass;
                var editattrs = EditAttrs;
                string attrs = ""; // Custom attributes
                if (IsDictionary(EditCustomAttributes) || IsAnonymousType(EditCustomAttributes)) // Custom attributes
                    editattrs.AddRange(ConvertToDictionary<object>(EditCustomAttributes));
                else
                    attrs = ConvertToString(EditCustomAttributes);
                editattrs.Concat("style", CssStyle, ";");
                editattrs.AppendClass(CssClass);
                if (Disabled)
                    editattrs["disabled"] = "true";
                if (IsInvalid && (Table == null || !SameString(GetPropertyValue(Table, "RowIndex"), "$rowindex$")))
                    editattrs.AppendClass("is-invalid");
                if (ReadOnly) {// For TEXT/PASSWORD/TEXTAREA only
                    if ((new[] {"TEXT", "PASSWORD", "TEXTAREA"}).Contains(HtmlTag)) { // Elements support readonly
                        editattrs["readonly"] = "true";
                    } else { // Elements do not support readonly
                        editattrs["disabled"] = "true";
                        editattrs["data-readonly"] = "1";
                        editattrs.AppendClass("disabled");
                    }
                }
                att = editattrs.ToString();
                if (!Empty(attrs)) // Custom attributes as string
                    att += " " + attrs;
                return att;
            }
        }

        // Link attributes
        public string LinkAttributes
        {
            get {
                var linkattrs = LinkAttrs;
                string attrs = ""; // Custom attributes
                if (IsDictionary(LinkCustomAttributes) || IsAnonymousType(LinkCustomAttributes)) // Custom attributes
                    linkattrs.AddRange(ConvertToDictionary<object>(LinkCustomAttributes));
                else
                    attrs = ConvertToString(LinkCustomAttributes);
                string href = ConvertToString(HrefValue).Trim();
                if (!Empty(href))
                    linkattrs["href"] = href;
                string att = linkattrs.ToString();
                if (!Empty(attrs)) // Custom attributes as string
                    att += " " + attrs;
                return att;
            }
        }

        // Header cell CSS class
        public string HeaderCellClass => AppendClass("ew-table-header-cell", HeaderCellCssClass);

        // Footer cell CSS class
        public string FooterCellClass => AppendClass("ew-table-footer-cell", FooterCellCssClass);

        // Add CSS class to all class properties
        public void AddClass(string className)
        {
            CellCssClass = AppendClass(CellCssClass, className);
            RowCssClass = AppendClass(RowCssClass, className);
            HeaderCellCssClass = AppendClass(HeaderCellCssClass, className);
            FooterCellCssClass = AppendClass(FooterCellCssClass, className);
        }

        // Delete CSS class from all class properties
        public void DeleteClass(string className)
        {
            CellCssClass = RemoveClass(CellCssClass, className);
            RowCssClass = RemoveClass(RowCssClass, className);
            HeaderCellCssClass = RemoveClass(HeaderCellCssClass, className);
            FooterCellCssClass = RemoveClass(FooterCellCssClass, className);
        }

        public void SetupEditAttributes(Attributes? attrs = null)
        {
            string classNames = InputTextType switch {
                "color" => "form-control form-control-color",
                "range" => "form-range",
                _ => "form-control"
            };
            EditAttrs.AppendClass(classNames);
            EditAttrs.Merge(attrs);
        }

        // Cell Styles (Used in export)
        public string CellStyles
        {
            get {
                string att = "";
                string style = Concatenate(CellCssStyle, ConvertToString(CellAttrs["style"]), ";");
                string cls = AppendClass(CellCssClass, ConvertToString(CellAttrs["class"]));
                if (!Empty(style))
                    att += " style=\"" + style.Trim() + "\"";
                if (!Empty(cls))
                    att += " class=\"" + cls.Trim() + "\"";
                return att;
            }
        }

        // Cell Attributes
        public string CellAttributes
        {
            get {
                var cellattrs = CellAttrs;
                cellattrs.Concat("style", CellCssStyle, ";");
                cellattrs.AppendClass(CellCssClass);
                return cellattrs.ToString();
            }
        }

        // Row Attributes
        public string RowAttributes
        {
            get {
                var rowattrs = RowAttrs;
                rowattrs.Concat("style", RowCssStyle, ";");
                rowattrs.AppendClass(RowCssClass);
                return rowattrs.ToString();
            }
        }

        // Sort Attributes
        public string Sort
        {
            get => Session.GetString(Config.ProjectName + "_" + TableVar + "_" + Config.TableSort + "_" + FieldVar);
            set {
                if (!SameText(Session[Config.ProjectName + "_" + TableVar + "_" + Config.TableSort + "_" + FieldVar], value))
                    Session[Config.ProjectName + "_" + TableVar + "_" + Config.TableSort + "_" + FieldVar] = value;
            }
        }

        // Next sorting order
        public string NextSort =>
            Sort switch {
                "ASC" => "DESC",
                "DESC" => SameText(Config.SortOption, "Tristate") ? "NO" : "ASC",
                "NO" => "ASC",
                _ =>"ASC"
            };

        // Get sorting order icon
        public string SortIcon =>
            Sort switch {
                "ASC" => Language.Phrase("SortUp"),
                "DESC" => Language.Phrase("SortDown"),
                _ => ""
            };

        // Get View value
        public string GetViewValue() => ViewValue is IHtmlValue v ? v.ToHtml() : ConvertToString(ViewValue);

        // Export Caption
        public string ExportCaption => ExportFieldCaption ? Caption : Name;

        // Export Value
        public string ExportValue => ExportOriginalValue ? ConvertToString(CurrentValue) : ConvertToString(ViewValue);

        // Get temp image
        public async Task<List<string>> GetTempImage()
        {
            List<string> images = new ();
            if (IsBlob) {
                if (!IsNull(Upload.DbValue)) { // DN
                    byte[] wrkdata = (byte[])Upload.DbValue;
                    if (ImageResize) {
                        int wrkwidth = ImageWidth;
                        int wrkheight = ImageHeight;
                        ResizeBinary(ref wrkdata, ref wrkwidth, ref wrkheight);
                    }
                    images.Add(await TempImage(wrkdata));
                }
            } else {
                string wrkfile = SameText(HtmlTag, "FILE") ? ConvertToString(Upload.DbValue) : ConvertToString(DbValue);
                if (Empty(wrkfile))
                    wrkfile = ConvertToString(CurrentValue);
                if (!Empty(wrkfile)) {
                    if (!UploadMultiple) {
                        string imagefn = PhysicalUploadPath + wrkfile;
                        if (FileExists(imagefn)) {
                            byte[] wrkdata = await FileReadAllBytes(imagefn); // DN
                            if (ImageResize) {
                                int wrkwidth = ImageWidth;
                                int wrkheight = ImageHeight;
                                ResizeBinary(ref wrkdata, ref wrkwidth, ref wrkheight);
                            }
                            images.Add(await TempImage(wrkdata));
                        }
                    } else {
                        var tmpfiles = wrkfile.Split(Config.MultipleUploadSeparator);
                        foreach (var tmpfile in tmpfiles) {
                            if (!Empty(tmpfile)) {
                                string imagefn = PhysicalUploadPath + tmpfile;
                                if (!FileExists(imagefn))
                                    continue;
                                byte[] wrkdata = await FileReadAllBytes(imagefn); // DN
                                if (ImageResize) {
                                    int wrkwidth = ImageWidth;
                                    int wrkheight = ImageHeight;
                                    ResizeBinary(ref wrkdata, ref wrkwidth, ref wrkheight);
                                }
                                images.Add(await TempImage(wrkdata));
                            }
                        }
                    }
                }
            }
            return images;
        }

        // Set form value
        public void SetFormValue(string? value, bool current = true, bool validate = true)
        {
            if (!Raw && DataType != DataType.Xml)
                value = RemoveXss(value);
            SetRawFormValue(value, current, validate);
        }

        // Set form value (Raw)
        public void SetRawFormValue(string? value, bool current = true, bool validate = true)
        {
            _formValue = validate && !Empty(value) && DataType == DataType.Number && !IsNumeric(value) // Check data type if server validation disabled
                ? null
                : value;
            if (current)
                CurrentValue = _formValue;
        }

        // Set old value
        public void SetOldValue(string? value) =>
            OldValue = !Empty(value) && DataType == DataType.Number && !IsNumeric(value) // Check data type
                ? null
                : value;

        // Form value
        public string? FormValue
        {
            get => _formValue;
            set => SetFormValue(value);
        }

        // Set query value
        public void SetQueryValue(string? value, bool current = true)
        {
            if (!Raw)
                value = RemoveXss(value);
            SetRawQueryValue(value, current);
        }

        // Set query value
        public void SetRawQueryValue(string? value, bool current = true)
        {
            _queryValue = !Empty(value) && DataType == DataType.Number && !IsNumeric(value) // Check data type
                ? null
                : value;
            if (current)
                CurrentValue = _queryValue;
        }

        // Query value
        public string? QueryValue
        {
            get => _queryValue;
            set => SetQueryValue(value);
        }

       // Set DB value (and current value)
        public void SetDbValue(object? value)
        {
            _dbValue = value ?? DbNullValue;
            if (UseEncryption && !Empty(value))
                value = AesDecrypt(ConvertToString(value));
            CurrentValue = value;
        }

        // DB value
        public object? DbValue
        {
            get => _dbValue;
            set => _dbValue = value ?? DbNullValue;
        }

        // Session Value
        public object? SessionValue
        {
            get => Session.GetValue(Config.ProjectName + "_" + TableVar + "_" + FieldVar + "_SessionValue"); // DN
            set => Session.SetValue(Config.ProjectName + "_" + TableVar + "_" + FieldVar + "_SessionValue", value); // DN
        }

        // Reverse sort
        public string ReverseSort() => (Sort == "ASC") ? "DESC" : "ASC";

        // Default value for NOT NULL field
        public object DbNotNullValue =>
            DataType switch {
                DataType.Number => 0,
                DataType.Date => DateTime.Now,
                DataType.String or DataType.Memo or DataType.Xml => "",
                DataType.Boolean or DataType.Bit => false,
                DataType.Blob => new byte[0],
                DataType.Time => TimeSpan.Zero,
                DataType.Guid => Guid.Empty,
                _ => DbNullValue // Unknown
            };

        // Set raw database value
        public void SetDbValue(Dictionary<string, object> dict, object? value, bool skip = false)
        {
            if (skip || !Visible || Disabled) {
                dict.Remove(Name);
                return;
            }
            object def = Nullable ? DbNullValue : DbNotNullValue;
            OldValue = _dbValue; // Save old DbValue in OldValue
            value = Type switch {
                2 or 3 or 16 or 17 or 18 or 19 or 20 or 21 => ParseInteger(value?.ToString()) is var i && IsNumeric(value) && i.HasValue ? ConvertType(i, Type) : def, // Integer
                5 or 6 or 14 or 131 or 139 => ConvertToFloatString(value) is var d && IsNumeric(d) ? ConvertType(d, Type) : def, // Double/Decimal
                4 => ConvertToFloatString(value) is var s && IsNumeric(s) ? ConvertType(s, Type) : def, // Single
                7 or 133 or 135 => IsDate(value) ? Convert.ToDateTime(value) : def, // Date
                134 or 145 => TimeSpan.TryParse(value?.ToString(), out TimeSpan ts) ? ts : def, // Time
                146 => DateTimeOffset.TryParse(value?.ToString(), out DateTimeOffset dt) ? dt : def, // DateTimeOffset
                201 or 203 or 129 or 130 or 200 or 202 => ConvertToString(value) is var s && !Empty(s) ? (UseEncryption ? AesEncrypt(s) : value) : def, // String
                141 => !Empty(value) ? value : def, // XML
                128 or 204 or 205 => IsNull(value) ? def : value, // Binary
                72 => !Empty(value) && CheckGuid(value?.ToString()) ? value : def, // GUID
                _ => value
            };
            _dbValue = value ?? DbNullValue;
            dict[Name] = _dbValue;
        }

        // HTML decode
        public string HtmlDecode(object value) => Raw ? ConvertToString(value) : WebUtility.HtmlDecode(ConvertToString(value));

        // Allowed file types (for jQuery File Upload)
        public string AcceptFileTypes => !Empty(UploadAllowedFileExtensions) ? "/\\.(" +  UploadAllowedFileExtensions.Replace(",", "|") + ")$/i" : "null";

        /// <summary>
        /// Set options
        /// </summary>
        /// <param name="options">User options</param>
        public void SetOptions(List<Dictionary<string, object>> options) => _options = options;

        /// <summary>
        /// Get options
        /// </summary>
        /// <returns>Options</returns>
        public List<Dictionary<string, object>> GetOptions() => _options
            ?? (OptionCount > 0
            ? Options(false) // User values
            : LookupOptions); // Lookup table

        /// <summary>
        /// Client side search operators
        /// </summary>
        /// <returns>Client side search operators</returns>
        public List<string> ClientSearchOperators() =>
            SearchOperators.Where(opr => Config.ClientSearchOperators.ContainsKey(opr)).Select(opr => Config.ClientSearchOperators[opr]).ToList();

        /// <summary>
        /// Output properties as JSON
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToClientList(dynamic currentPage)
        {
            Dictionary<string, object> dict = new ();
            if (Lookup != null) {
                List<Dictionary<string, object>> options = Lookup.HasParentTable() ? new () : GetOptions();
                dict = Merge(Lookup.ToClientList(currentPage), new {
                    lookupOptions = options,
                    multiple = SameText(HtmlTag, "SELECT") && SelectMultiple || SameText(HtmlTag, "CHECKBOX") && !IsBoolean // Do not use IsMultiSelect since data type could be int
                });
            }
            return ConvertToJson(dict);
        }
    }
} // End Partial class
