namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Common class for table and report
    /// </summary>
    public class DbTableBase : ZaharuddinPage
    {
        private string? _tableCaption = null;

        private Dictionary<int, string> _pageCaption = new ();

        public string Name = "";

        public string Type = "";

        public string DbId = "DB"; // Table database ID

        public bool UseSelectLimit = true;

        public bool Visible = true;

        public bool EncodeSlash = true;

        public Dictionary<string, DbField> Fields { get; } = new (); // DN

        public Dictionary<string, DbChart> Charts { get; } = new (); // DN

        public List<Dictionary<string, object?>> Rows = new (); // Data for Custom Template DN

        public string OldKey = ""; // Old key (for edit/copy)

        public string OldKeyName; // Old key name (for edit/copy)

        public bool UseCustomTemplate = false; // Use custom template // DN

        public string Export = ""; // Export

        public bool ExportAll;

        public int ExportPageBreakCount; // Page break per every n record (PDF only)

        public string ExportPageOrientation = ""; // Page orientation (PDF only)

        public string ExportPageSize = ""; // Page size (PDF only)

        public float[] ExportColumnWidths = new float[] {}; // Column widths (PDF only) // DN

        public string ExportExcelPageOrientation = ""; // Page orientation (Excel only)

        public string ExportExcelPageSize = ""; // Page size (Excel only)

        public string ExportWordPageOrientation = ""; // Page orientation (Word only)

        public bool SendEmail; // Send email

        public string PageBreakHtml;

        public bool ExportPageBreaks = true; // Page breaks when export

        public bool ImportInsertOnly; // Import by insert only

        public bool ImportUseTransaction = Config.ImportUseTransaction; // Import use transaction

        public int ImportMaxFailures = 0; // Import maximum number of failures

        private Lazy<BasicSearch> _basicSearch; // Basic search

        public string QueryRules = ""; // Rules from jQuery Query builder

        public string CurrentFilter = ""; // Current filter

        public string CurrentOrder = ""; // Current order

        public string CurrentOrderType = ""; // Current order type

        public RowType RowType; // Row type

        public string CssClass = ""; // CSS class

        public string CssStyle = ""; // CSS style

        public Attributes RowAttrs = new (); // DN

        public string CurrentAction = ""; // Current action

        public string ActionValue = ""; // Action value

        public string LastAction = ""; // Last action

        public int UserIdAllowSecurity = 0; // User ID Allow

        public string Command = "";

        public int Count = 0; // Record count (as detail table)

        // Update Table
        public string UpdateTable = "";

        public string SearchOption; // Search option

        public string Filter = "";

        public string DefaultFilter = "";

        public string Sort = "";

        public bool AutoHidePager;

        public bool AutoHidePageSizeSelector;

        public string RowAction = ""; // Row action

        // Charts related
        public bool SourceTableIsCustomView = false;

        public string TableReportType = "";

        public bool ShowDrillDownFilter;

        public bool UseDrillDownPanel; // Use drill down panel

        public bool DrillDown = false;

        public bool DrillDownInPanel = false;

        // Table
        public string TableClass = "";

        public string TableGridClass = ""; // CSS class for .card (with a leading space)

        public string TableContainerClass = ""; // CSS class for .card-body (e.g. height of the main table)

        public string TableContainerStyle = ""; // CSS style for .card-body (e.g. height of the main table)

        public bool UseResponsiveTable = false;

        public string ResponsiveTableClass = "";

        public string ContainerClass = "p-0";

        public string ContextClass = ""; // CSS class name as context

        public bool ShowCurrentFilter;

        // Default field properties
        public bool Raw;

        public string UploadPath;

        public string OldUploadPath;

        public string HrefPath;

        public string UploadAllowedFileExtensions;

        public int UploadMaxFileSize;

        public int? UploadMaxFileCount;

        public bool ImageCropper;

        public bool UseColorbox;

        public bool AutoFillOriginalValue;

        public bool UseLookupCache;

        public int LookupCacheCount;

        public bool ExportOriginalValue;

        public bool ExportFieldCaption;

        public bool ExportFieldImage;

        public string DefaultNumberFormat;

        public bool SearchHighlight = false;

        // Protected fields (Temp data) // DN
        protected string? sqlWrk;

        protected string? filterWrk;

        protected string? whereWrk;

        protected string? curVal;

        protected string? dispVal;

        protected List<Dictionary<string, object>>? rswrk;

        protected string[]? arWrk;

        protected List<object?>? listWrk;

        protected object? guidWrk;

        protected Func<string>? lookupFilter;

        // Constructor
        public DbTableBase()
        {
            _basicSearch = new Lazy<BasicSearch>(() => new BasicSearch(this));
            OldKeyName = Config.FormOldKeyName;
            SearchOption = Config.SearchOption;
            ImportInsertOnly = Config.ImportInsertOnly;
            ImportMaxFailures = Config.ImportMaxFailures;
            AutoHidePager = Config.AutoHidePager;
            AutoHidePageSizeSelector = Config.AutoHidePageSizeSelector;
            UseResponsiveTable = !IsExport() && Config.UseResponsiveTable;
            ResponsiveTableClass = Config.ResponsiveTableClass;
            TableContainerClass = UseResponsiveTable ? ResponsiveTableClass : "";
            ShowCurrentFilter = Config.ShowCurrentFilter;

            // Default field properties
            Raw = !Config.RemoveXss;
            UploadPath = Config.UploadDestPath;
            OldUploadPath = Config.UploadDestPath;
            HrefPath = Config.UploadHrefPath;
            UploadAllowedFileExtensions = Config.UploadAllowedFileExtensions;
            UploadMaxFileSize = Config.MaxFileSize;
            UploadMaxFileCount = Config.MaxFileCount;
            ImageCropper = Config.ImageCropper;
            UseColorbox = Config.UseColorbox;
            AutoFillOriginalValue = Config.AutoFillOriginalValue;
            UseLookupCache = Config.UseLookupCache;
            LookupCacheCount = Config.LookupCacheCount;
            ExportOriginalValue = Config.ExportOriginalValue;
            ExportFieldCaption = Config.ExportFieldCaption;
            ExportFieldImage = Config.ExportFieldImage;
            DefaultNumberFormat = Config.DefaultNumberFormat;

            // Page break
            PageBreakHtml = Config.PageBreakHtml;
        }

        // Basic search
        public BasicSearch BasicSearch => _basicSearch.Value;

        // Search Highlight Name
        public string HighlightName => TableVar + "-highlight";

        // Table caption
        public string Caption
        {
            get => _tableCaption ?? Language.TablePhrase(TableVar, "TblCaption");
            set => _tableCaption = value;
        }

        // Table caption (alias)
        public string TableCaption => Caption;

        // Page caption
        public string PageCaption(int page)
        {
            _pageCaption.TryGetValue(page, out string? caption);
            if (!Empty(caption)) {
                return caption;
            } else {
                caption = Language.TablePhrase(TableVar, "TblPageCaption" + page.ToString());
                if (Empty(caption))
                    caption = "Page " + page.ToString();
                return caption;
            }
        }

        // Row Styles
        public string RowStyles
        {
            get {
                string att = "";
                string style = Concatenate(CssStyle, ConvertToString(RowAttrs["style"]), ";");
                string cls = AppendClass(CssClass, ConvertToString(RowAttrs["class"]));
                if (!Empty(style))
                    att += " style=\"" + style.Trim() + "\"";
                if (!Empty(cls))
                    att += " class=\"" + cls.Trim() + "\"";
                return att;
            }
        }

        // Row Attribute
        public string RowAttributes
        {
            get {
                string att = RowStyles;
                if (!IsExport()) {
                    string attrs = RowAttrs.ToString(new () { "class", "style" });
                    if (!Empty(attrs))
                        att += attrs;
                }
                return att;
            }
        }

        // Convert dictionary to filter // DN
        public string ConvertToFilter(IDictionary<string, object> dict)
        {
            string filter = "";
            foreach (var (name, value) in dict) {
                var fld = FieldByName(name);
                if (fld != null)
                    AddFilter(ref filter, QuotedName(fld.Name, DbId) + " = " + QuotedValue(value, fld.DataType, DbId));
            }
            return filter;
        }

        // Reset attributes for table object
        public void ResetAttributes()
        {
            CssClass = "";
            CssStyle = "";
            RowAttrs.Clear();
            foreach (var (key, fld) in Fields)
                fld.ResetAttributes();
        }

        // Set a property of all fields
        public void SetFieldProperties(string name, object val)
        {
            foreach (var (key, fld) in Fields)
                SetPropertyValue(fld, name, val);
        }

        // Set current values (for number/date/time fields only)
        public void SetCurrentValues(Dictionary<string, object> row)
        {
            foreach (var (name, value) in row) {
                var fld = FieldByName(name);
                if (fld != null && (new[] { DataType.Number, DataType.Date, DataType.Time }).Contains(fld.DataType))
                    fld.CurrentValue = value;
            }
        }

        // Setup field titles
        public void SetupFieldTitles()
        {
            foreach (var (key, fld) in Fields.Where(kvp => !Empty(kvp.Value.Title))) {
                fld.EditAttrs["data-bs-toggle"] = "tooltip";
                fld.EditAttrs["title"] = HtmlEncode(fld.Title);
            }
        }

        // Get form values (for validation)
        public Dictionary<string, string?> GetFormValues() => Fields.ToDictionary(kvp => kvp.Value.Name, kvp => kvp.Value.FormValue); // Use field name

        // Get field values
        public Dictionary<string, object?> GetFieldValues(string name) => Fields.ToDictionary(
                kvp => kvp.Value.Name, // Use field name
                kvp => kvp.Value.GetType().GetProperty(name) is var pi // Property
                    ? pi?.GetValue(kvp.Value, null)
                    : kvp.Value.GetType().GetField(name) is var fi // Field
                    ? fi?.GetValue(kvp.Value)
                    : null
            );

        // Field cell attributes
        public Dictionary<string, string> FieldCellAttributes => Fields.ToDictionary(kvp => kvp.Value.Param, kvp => kvp.Value.CellAttributes); // Use Parm

        // Get field DB values for Custom Template
        public Dictionary<string, object?> CustomTemplateFieldValues()
        {
            return Fields.Where(kvp => Config.CustomTemplateDataTypes.Contains(kvp.Value.DataType) && kvp.Value.Visible).ToDictionary(
                kvp => kvp.Value.Param, // Use Param
                kvp => kvp.Value.DbValue is string value && value.Length > Config.DataStringMaxLength
                    ? value.Substring(0, Config.DataStringMaxLength)
                    : kvp.Value.DbValue
            );
        }

        // Set page caption
        public void SetPageCaption(int page, string v) => _pageCaption[page] = v;

        // Get chart object by Id
        public DbChart? ChartByID(string id) => Charts.FirstOrDefault(kvp => kvp.Value.ID == id).Value;

        // Get chart object by name
        public DbChart? ChartByName(string name) => Charts.FirstOrDefault(kvp => kvp.Key == name).Value;

        // Get chart object by param
        public DbChart? ChartByParam(string parm) => Charts.FirstOrDefault(kvp => kvp.Value.ChartVar == parm).Value;

        // Get field object by name
        public DbField? FieldByName(string name) => Fields.FirstOrDefault(kvp => kvp.Key == name).Value;

        // Get field object by param
        public DbField? FieldByParam(string parm) => Fields.FirstOrDefault(kvp => kvp.Value.Param == parm).Value;

        // Check if fixed header table
        public bool IsFixedHeaderTable => ContainsClass(TableClass, Config.FixedHeaderTableClass);

        /// <summary>
        /// Set fixed header table
        /// </summary>
        /// <param name="enabled">Enable fixed header table</param>
        /// <param name="height">Css height</param>
        public void SetFixedHeaderTable(bool enabled, string? height = null)
        {
            if (enabled && !IsExport()) {
                TableClass = AppendClass(TableClass, Config.FixedHeaderTableClass);
                height ??= Config.FixedHeaderTableHeight;
                if (!Empty(height)) {
                    TableContainerClass = AppendClass(TableContainerClass, height);
                    TableContainerClass = AppendClass(TableContainerClass, "overflow-y-auto");
                }
            } else {
                TableClass = RemoveClass(TableClass, Config.FixedHeaderTableClass);
                TableContainerClass = AppendClass(TableContainerClass, "h-auto"); // Override height class
                TableContainerClass = AppendClass(TableContainerClass, "overflow-y-auto");
            }
        }

        // Has Invalid fields
        public bool HasInvalidFields() => Fields.Values.Any(v => v.IsInvalid);

        // Visible field count
        public int VisibleFieldCount => Fields.Values.Where(fld => fld.Visible).Count();

        // Is export
        public bool IsExport(string format = "") => Empty(format) ? !Empty(Export) : SameText(Export, format);

        // Get field visibility
        public virtual bool GetFieldVisibility(string name) => FieldByName(name)?.Visible ?? false;

        /// <summary>
        /// Set use lookup cache
        /// </summary>
        /// <param name="useLookupCache">Use lookup cache or not</param>
        public void SetUseLookupCache(bool useLookupCache)
        {
            foreach (var (key, fld) in Fields)
                fld.UseLookupCache = useLookupCache;
        }

        /// <summary>
        /// Set lookup cache count
        /// </summary>
        /// <param name="count">The maximum number of options to cache</param>
        public void SetLookupCacheCount(int count)
        {
            foreach (var (key, fld) in Fields)
                fld.LookupCacheCount = count;
        }

        /// <summary>
        /// Convert properties to client side variables
        /// </summary>
        /// <param name="tablePropertyNames">Table property names</param>
        /// <param name="fieldPropertyNames">Field property names</param>
        /// <returns>ClientVar as dictionary</returns>
        public Dictionary<string, dynamic?> ToClientVar(List<string>? tablePropertyNames = null, List<string>? fieldPropertyNames = null)
        {
            Dictionary<string, dynamic?> props = new ();
            if (tablePropertyNames == null || tablePropertyNames.Count == 0)
                tablePropertyNames = Config.TableClientVars;
            if (fieldPropertyNames == null || fieldPropertyNames.Count == 0)
                fieldPropertyNames = Config.FieldClientVars;
            foreach (string name in tablePropertyNames) {
                if (this.GetType().GetMethod(name) is MethodInfo mi)
                    props.Add(LowerCaseFirst(name), mi.Invoke(this, null));
                else if (this.GetType().GetProperty(name) is PropertyInfo pi)
                    props.Add(LowerCaseFirst(name), pi.GetValue(this, null));
                else if (this.GetType().GetField(name) is FieldInfo fi)
                    props.Add(LowerCaseFirst(name), fi.GetValue(this));
            }
            if (fieldPropertyNames != null && fieldPropertyNames.Count > 0) {
                Dictionary<string, object> fieldsProps = new ();
                foreach (var (key, fld) in Fields) {
                    Dictionary<string, object?> fieldProps = new ();
                    fieldsProps.Add(fld.Param, fieldProps);
                    foreach (string name in fieldPropertyNames) {
                        if (fld.GetType().GetMethod(name) is MethodInfo mi)
                            fieldProps.Add(LowerCaseFirst(name), mi.Invoke(fld, null));
                        else if (fld.GetType().GetProperty(name) is PropertyInfo pi)
                            fieldProps.Add(LowerCaseFirst(name), pi.GetValue(fld, null));
                        else if (fld.GetType().GetField(name) is FieldInfo fi)
                            fieldProps.Add(LowerCaseFirst(name), fi.GetValue(fld));
                    };
                }
                props.Add("fields", fieldsProps);
            }
            var clientVar = GetClientVar("tables", TableVar);
            if (clientVar is Dictionary<string, dynamic?> dict)
                props.Concat(dict).GroupBy(p => p.Key).ToDictionary(g => g.Key, g => g.Last().Value);
            return props;
        }

        protected string LowerCaseFirst(string name) => Regex.Replace(name, @"^([A-Za-z])", m => m.Value.ToLower());

        // Encode key value
        public string EncodeKeyValue(object key)
        {
            if (Empty(key))
                return ConvertToString(key);
            else if (EncodeSlash)
                return RawUrlEncode(key);
            else
                return String.Join("/", ConvertToString(key).Split('/').Select(v => RawUrlEncode(v)));
        }

        // Run
        #pragma warning disable 1998

        public override async Task<IActionResult> Run() => new EmptyResult();
        #pragma warning restore 1998

        // Terminate
        public override IActionResult Terminate(string url = "") => new EmptyResult();
    }
} // End Partial class
