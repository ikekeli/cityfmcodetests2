namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Chart class
    /// </summary>
    public class DbChart
    {
        public dynamic Table; // Table object

        public string Type; // Chart type

        public int Position; // Chart position

        public string TableVar = ""; // Retained for compatibility

        public string TableName = ""; // Retained for compatibility

        public string Name = ""; // Chart name

        public string ChartVar = ""; // Chart variable name

        public string XFieldName = ""; // Chart X field name

        public string YFieldName = ""; // Chart Y field name

        public string SeriesFieldName = ""; // Chart series field name

        public int SeriesType; // Chart series type

        public string SeriesRenderAs = ""; // Chart series renderAs

        public string SeriesYAxis = ""; // Chart series Y axis

        public bool RunTimeSort = false; // Chart run time sort

        public string SortType = ""; // Chart sort type

        public string SortSequence = ""; // Chart sort sequence

        public string SummaryType = ""; // Chart summary type

        public int Width; // Chart width

        public int Height; // Chart height

        public int GridHeight = 200; // Chart grid height

        public string DrillDownTable = ""; // Chart drill down table name

        public string GridConfig = "";

        public string Align = ""; // Chart align

        public string ContainerClass = "overlay-wrapper"; // Container class

        public string DrillDownUrl = ""; // Chart drill down URL

        public bool UseDrillDownPanel = Config.UseDrillDownPanel; // Use drill down panel

        public string DefaultNumberFormat = Config.DefaultNumberFormat;

        public string SqlSelect = "";

        public string SqlWhere = "";

        public string SqlGroupBy = "";

        public string SqlOrderBy = "";

        public string XAxisDateFormat = "";

        public List<string> YFieldFormat = new ();

        public List<string> YAxisFormat = new ();

        public string SeriesDateType = "";

        public string SqlSelectSeries = "";

        public string SqlWhereSeries = "";

        public string SqlGroupBySeries = "";

        public string SqlOrderBySeries = "";

        public string SeriesSql = "";

        public string Sql = "";

        public string PageBreakType = "before"; // "before" or "after" // DN

        public string PageBreakClass = ""; // "break-before-page" and/or "break-after-page"

        public bool DrillDownInPanel = false;

        public bool ScrollChart = false;

        public bool IsCustomTemplate = false;

        public bool CompactDataMode = false; // For zoomline chart only

        public string DataSeparator = "|"; // For zoomline chart only

        public string ID = "";

        public DotAccessData Parameters = new ();

        public List<Annotation> Trends = new ();

        public List<Dictionary<string, object>> Data = new ();

        public List<Dictionary<string, object?>> ViewData = new ();

        public List<object> Series = new ();

        public Regex RegexColor = new ("^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$");

        public string DataSource = ""; // Data source for renderer

        public string DataFormat = "json";

        public bool ScaleBeginWithZero = Config.ChartScaleBeginWithZero;

        public double? MinValue = Config.ChartScaleMinimumValue != 0 ? Config.ChartScaleMinimumValue : null;

        public double? MaxValue = Config.ChartScaleMaximumValue != 0 ? Config.ChartScaleMaximumValue : null;

        public bool ShowPercentage = false; // Pie / Doughnut charts only

        public bool ShowLookupForXAxis = true;

        public bool ShowChart = true;

        private string caption = "";

        private bool dataLoaded = false;

        // Constructor for Chart // DN
        public DbChart(dynamic table, string type)
        {
            Table = table;
            TableVar = table.TableVar; // For compatiblity
            TableName = table.Name; // For compatiblity
            Type = type;
            ShowPercentage = Config.ChartShowPercentage;
        }

        // Chart caption
        public string Caption
        {
            get => caption ?? Language.ChartPhrase(Table.TableVar, ChartVar, "ChartCaption");
            set => caption = value;
        }

        // XAxisname
        public string XAxisName => Language.ChartPhrase(Table.TableVar, ChartVar, "ChartXAxisName");

        // YAxisname
        public string YAxisName => Language.ChartPhrase(Table.TableVar, ChartVar, "ChartYAxisName");

        // PYAxisName
        public string PrimaryYAxisName => Language.ChartPhrase(Table.TableVar, ChartVar, "ChartPYAxisName");

        // SYAxisName
        public string SecondaryYAxisName => Language.ChartPhrase(Table.TableVar, ChartVar, "ChartSYAxisName");

        // Sort
        public string Sort
        {
            get => Session.GetString(Config.ProjectName + "_" + Table.TableVar + "_" + Config.TableSortChart + "_" + ChartVar);
            set {
                if (!SameString(Session[Config.ProjectName + "_" + Table.TableVar + "_" + Config.TableSortChart + "_" + ChartVar], value))
                    Session.SetString(Config.ProjectName + "_" + Table.TableVar + "_" + Config.TableSortChart + "_" + ChartVar, value);
            }
        }

        // Get chart parameter
        public string GetParameter(string name) => ConvertToString(Parameters.Get(name));

        // Set chart parameter
        public void SetParameter(string name, dynamic value) => Parameters.Set(name, value);

        // Set up default chart parameter
        public void SetDefaultParameter(string name, object value)
        {
            string parm = LoadParameter(name);
            if (parm == null)
                SaveParameter(name, ConvertToString(value));
        }

        // Load chart parameter
        public string LoadParameter(string name) => ConvertToString(Parameters.Get(name));

        // Load chart parameter // DN
        public T LoadParameter<T>(string name) => ChangeType<T>(Parameters.Get(name));

        // Save chart parameter
        public void SaveParameter(string name, string value) => Parameters.Set(name, value);

        // Load chart parameters
        public void LoadParameters()
        {
            // Initialize default values
            SetDefaultParameter("caption", "Chart");

            // Show names/values/hover
            SetDefaultParameter("showlabels", "1"); // Default show names
            SetDefaultParameter("showvalues", "1"); // Default show values

            // Get showvalues/showhovercap
            bool showValues = LoadParameter<bool>("showvalues");
            bool showHoverCap = LoadParameter<bool>("showhovercap") || LoadParameter<bool>("showToolTip");

            // Show tooltip
            if (showHoverCap && !LoadParameter<bool>("showToolTip"))
                SaveParameter("showToolTip", "1");

            // Format percent for Pie charts
            bool showPercentageValues = LoadParameter<bool>("showPercentageValues");
            bool showPercentageInLabel = LoadParameter<bool>("showPercentageInLabel");
            if (IsPieChart || IsDoughnutChart) {
                if (showHoverCap && showPercentageValues || showValues && showPercentageInLabel) {
                    SetDefaultParameter("formatNumber", "1");
                    SaveParameter("formatNumber", "1");
                }
            }

            // Hide legend for single series (Column 2D / Line 2D / Area 2D)
            bool singleSeries = ScrollChart && IsSingleSeries;
            if (singleSeries) {
                SetDefaultParameter("showLegend", "0");
                SaveParameter("showLegend", "0");
            }
        }

        // Load view data
        public void LoadViewData()
        {
            string sdt = SeriesDateType;
            string xdt = XAxisDateFormat;
            string ndt = ""; // Not used
            if (!Empty(sdt))
                xdt = sdt;
            ViewData.Clear();
            if (Empty(sdt) && Empty(xdt) && Empty(ndt)) { // Format Y values
                for (var i = 0; i < Data.Count; i++) {
                    var temp = new Dictionary<string, object?>();
                    var chartRow = Data[i];
                    var rowCount = chartRow.Count;
                    var yCount = SeriesType == 1 && Series.Count > 0 ? Series.Count : 1;
                    var valueList = chartRow.Values.ToList();
                    for (var j = 0; j < rowCount; j++) {
                        if (j >= rowCount - yCount)
                            temp.Add(ConvertToString(j), FormatNumber(valueList[j])); // Y values
                        else
                            temp.Add(ConvertToString(j), valueList[j]);
                    }
                    ViewData.Add(temp);
                }
            } else if (IsList(Data)) { // Format data
                for (var i = 0; i < Data.Count; i++) {
                    var temp = new Dictionary<string, object?>();
                    var chartRow = Data[i];
                    var rowCount = chartRow.Count;
                    var valueList = chartRow.Values.ToList();
                    temp.Add("0", GetXValue(valueList[0], xdt)); // X value
                    temp.Add("1", SeriesValue(valueList[1], sdt)); // Series value
                    for (var j = 2; j < rowCount; j++) {
                        if (!Empty(ndt) && j == rowCount - 1)
                            temp.Add(ConvertToString(j), GetXValue(valueList[j], ndt)); // Name value
                        else
                            temp.Add(ConvertToString(j), FormatNumber(valueList[j])); // Y values
                    }
                    ViewData.Add(temp);
                }
            }
        }

        // Set up chart
        public void Setup()
        {
            string sqlSelect, sqlChartSelect, sqlChartBase, sql, chartFilter;

            // Set up chart base SQL
            if (SameString(Table.TableReportType, "crosstab")) { // Crosstab chart
                sqlSelect = Table.SqlSelect.Replace("{DistinctColumnFields}", Table.DistinctColumnFields);
                sqlChartSelect = SqlSelect;
            } else {
                sqlSelect = Table.SqlSelect;
                sqlChartSelect = SqlSelect;
            }
            string pageFilter = DashboardReport
                ? ConvertToString(CurrentPage?.Invoke("ApplyUserIDFilters", new object[] { "", "" }) ?? "")
                : CurrentPage?.Filter ?? "";
            if (Table.SourceTableIsCustomView) {
                string sqlWhere = Table.SqlWhere;
                string sqlGroupBy = Table.SqlGroupBy;
                string sqlHaving = Table.SqlHaving;
                string sqlOrderBy = (GetConnectionType() == "MSSQL") ? Table.SqlOrderBy : "";
                sqlChartBase = "(" + BuildReportSql(sqlSelect, sqlWhere, sqlGroupBy, sqlHaving, sqlOrderBy, pageFilter, "") + ") TMP_TABLE";
            } else {
                sqlChartBase = Table.SqlFrom;
            }

            // Set up chart series
            if (!EmptyString(SeriesFieldName)) {
                if (SeriesType == 1) { // Multiple Y fields
                    string[] ar = !Empty(SeriesFieldName) ? SeriesFieldName.Split('|') : new string[] {};
                    string[] yaxis = !Empty(SeriesYAxis) ? SeriesYAxis.Split(',') : new string[] {};
                    for(int i = 0; i < ar.Length; i++) {
                        var fld = Table.Fields[ar[i]];
                        string series = "";
                        if (Type.StartsWith("4")) { // Combination charts
                            if (yaxis.Length > 0)
                                series = SameString(yaxis[i], "2") ? "y1" : "y";
                            else
                                series = "y";
                            Series.Add(new[] {fld.Caption, series});
                        } else {
                            Series.Add(fld.Caption);
                        }
                    }
                } else if (SameString(Table.TableReportType, "crosstab") && SameString(SeriesFieldName, Table.ColumnFieldName) && ConvertToBool(Table.ColumnDateSelection) && SameString(Table.ColumnDateSelection, "q")) { // Quarter
                    for (int i = 1; i <= 4; i++)
                        Series.Add(QuarterName(i));
                } else if (SameString(Table.TableReportType, "crosstab") && SameString(SeriesFieldName, Table.ColumnFieldName) && ConvertToBool(Table.ColumnDateSelection) && SameString(Table.ColumnDateType, "m")) { // Month
                    for (int i = 1; i <= 12; i++)
                        Series.Add(MonthName(i));
                } else { // Load chart series from sql directly
                    if (Table.SourceTableIsCustomView) {
                        sql = SqlSelectSeries + sqlChartBase;
                        sql = BuildReportSql(sql, SqlWhereSeries, SqlGroupBySeries, "", SqlOrderBySeries, "", "");
                    } else {
                        sql = SqlSelectSeries + sqlChartBase;
                        chartFilter = SqlWhereSeries;
                        AddFilter(ref chartFilter, Table.SqlWhere);
                        sql = BuildReportSql(sql, chartFilter, SqlGroupBySeries, "", SqlOrderBySeries, pageFilter, "");
                    }
                    SeriesSql = sql;
                }
            }

            // Run time sort, update SqlOrderBy
            if (RunTimeSort)
                SqlOrderBy += (SortType == "2") ? " DESC" : "";

            // Set up SQL
            if (Table.SourceTableIsCustomView) {
                sql = sqlChartSelect + sqlChartBase;
                sql = BuildReportSql(sql, SqlWhere, SqlGroupBy, "", SqlOrderBy, "", "");
            } else {
                sql = sqlChartSelect + sqlChartBase;
                chartFilter = SqlWhere;
                AddFilter(ref chartFilter, Table.SqlWhere);
                sql = BuildReportSql(sql, chartFilter, SqlGroupBy, "", SqlOrderBy, pageFilter, "");
            }
            Sql = sql;
        }

        // Load chart data
        public async Task LoadData()
        {
            // Data already loaded, return
            if (!dataLoaded) {
                // Setup chart series data
                if (!Empty(SeriesSql)) {
                    await LoadSeries();
                    if (Config.Debug)
                        SetDebugMessage("(Chart Series SQL): " + SeriesSql);
                }

                // Setup chart data
                if (!Empty(Sql)) {
                    var rscht = await (await GetConnectionAsync(Table.DbId)).GetRowsAsync(Sql);
                    if (rscht != null) {
                        foreach (Dictionary<string, object> row in rscht) {
                            var newRow = ShowLookupForXAxis ? await Table.RenderChartXAxis(ChartVar, row) : row;
                            Data.Add(newRow);
                        }
                    }
                    if (Config.Debug)
                        SetDebugMessage("(Chart SQL): " + Sql);
                }

                // Sort data
                if (!Empty(SeriesFieldName) && SeriesType != 1)
                    SortMultiData();
                else
                    SortData();
                dataLoaded = true;
            }
        }

        // Load Chart Series
        public async Task LoadSeries()
        {
            string sql = SeriesSql;
            var rswrk = await (await GetConnectionAsync(Table.DbId)).GetRowsAsync(sql);
            string sdt = SeriesDateType;
            if (rswrk?.Count > 0) {
                foreach (Dictionary<string, object> row in rswrk) {
                    var listwrk = row.Values.ToList();
                    Series.Add(SeriesValue(ConvertToString(listwrk[0]), sdt)); // Series value
                }
            }
        }

        // Get Chart Series value
        public string SeriesValue(object? val, string dt) => dt.ToLower() switch {
                "syq" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2
                    ? ar[0] + " " + QuarterName(ar[1])
                    : ConvertToString(val),
                "sym" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2
                    ? ar[0] + " " + MonthName(ar[1])
                    : ConvertToString(val),
                "sq" => QuarterName(val),
                "sm" => MonthName(val),
                _ => ConvertToString(val).Trim()
            };

        // Get Chart X value
        public string GetXValue(object? val, string dt)
        {
            if (IsNull(val)) {
                return Language.Phrase("NullLabel");
            } else if (Empty(val)) {
                return Language.Phrase("EmptyLabel");
            } else if (IsNumeric(dt)) {
                return FormatDateTime(val, ConvertToInt(dt));
            }
            return dt switch {
                "y" => ConvertToString(val),
                "xyq" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2
                    ? ar[0] + " " + QuarterName(ar[1])
                    : ConvertToString(val),
                "xym" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2
                    ? ar[0] + " " + MonthName(ar[1])
                    : ConvertToString(val),
                "xq" => QuarterName(val),
                "xm" => MonthName(val),
                _ => ConvertToString(val).Trim()
            };
        }

        // Sort chart data
        public void SortData()
        {
            int opt = ConvertToInt(SortType);
            string seq = SortSequence;
            if ((opt < 3 || opt > 4) && Empty(seq) || (opt < 1 || opt > 4) && !Empty(seq))
                return;
            if ((opt == 3 || opt == 4) && Empty(seq))
                seq = "_number";
            ChartDataComparer? comparer = opt switch {
                1 => new (0, seq, "ASC"), // X values ascending
                2 => new (0, seq, "DESC"), // X values descending
                3 => new (2, seq, "ASC"), // Y values ascending
                4 => new (2, seq, "DESC"), // Y values descending
                _ => null
            };
            if (comparer != null)
                Data.Sort(comparer);
        }

        // Sort chart multi series data
        public void SortMultiData()
        {
            int opt = ConvertToInt(SortType);
            string seq = SortSequence;
            if (!IsList(Data) || ((opt < 3 || opt > 4) && Empty(seq)) || ((opt < 1 || opt > 4) && !Empty(seq)))
                return;
            if ((opt == 3 || opt == 4) && Empty(seq))
                seq = "_number";

            // Obtain a list of columns
            Dictionary<string, object> xsums = new ();
            foreach (var d in Data) {
                string key = d.Keys.ToList()[0];
                var values = d.Values.ToList();
                xsums[key] = xsums.ContainsKey(key)
                    ? ConvertToDouble(xsums[key]) + ConvertToDouble(values[2])
                    : values[2];
            }

            // Set up Y sum
            int idx = -1;
            if (opt == 3 || opt == 4) {
                foreach (var d in Data) {
                    string key = d.Keys.ToList()[0];
                    if (idx == -1)
                        idx = d.Count;
                    d.Add(ConvertToString(idx), xsums[key]);
                }
            }
            switch (opt) {
                case 1: // X values ascending
                    Data.Sort(new ChartDataComparer(0, seq, "ASC"));
                    break;
                case 2: // X values descending
                    Data.Sort(new ChartDataComparer(0, seq, "DESC"));
                    break;
                case 3: // Y values ascending
                    Data.Sort(new ChartDataComparer(idx, seq, "ASC"));
                    break;
                case 4: // Y values descending
                    Data.Sort(new ChartDataComparer(idx, seq, "DESC"));
                    break;
            }
        }

        // Get color
        public string GetPaletteColor(int i)
        {
            string colorPalette = LoadParameter("colorpalette");
            string[] colors = colorPalette.Split(new char[] { '|', ',', ' ' });
            return colors[i % colors.Length];
        }

        // Get RGBA color
        public string GetPaletteRgbaColor(int i, double? opacity = null)
        {
            string color = GetPaletteColor(i);

            // Return default if no color provided
            if (Empty(color))
                return ""; // Use chart default

            // Check opacity
            opacity ??= GetOpacity(LoadParameter("alpha"));
            return GetRgbaColor(color, opacity);
        }

        // Format name for chart
        public string FormatName(object? name)
        {
            if (IsNull(name))
                return Language.Phrase("NullLabel");
            else if (Empty(name))
                return Language.Phrase("EmptyLabel");
            return ConvertToString(name);
        }

        // Is single series chart
        public bool IsSingleSeries => Type.StartsWith("1");

        // Is zoom line chart
        public bool IsZoomLineChart => Type.StartsWith("92");

        // Is column chart
        public bool IsColumnChart => Type.EndsWith("01");

        // Is line chart
        public bool IsLineChart => Type.EndsWith("02");

        // Is area chart
        public bool IsAreaChart => Type.EndsWith("03");

        // Is bar chart
        public bool IsBarChart => Type.EndsWith("04");

        // Is pie chart
        public bool IsPieChart => Type.EndsWith("05");

        // Is doughnut chart
        public bool IsDoughnutChart => Type.EndsWith("06");

        // Is stack chart
        public bool IsStackedChart => Type.StartsWith("3") || (new[] { "4021", "4121", "4141" }).Contains(Type);

        // Is combination chart
        public bool IsCombinationChart => Type.StartsWith("4");

        // Is dual axis chart
        public bool IsDualAxisChart => (new[] { "4031", "4131", "4141" }).Contains(Type);

        // Format number for chart
        public string? FormatNumber(object? v)
        {
            if (v == null)
                return null;
            if (v is double val) // DN
                return !Empty(DefaultNumberFormat)
                    ? val.ToString(DefaultNumberFormat, new System.Globalization.CultureInfo("en-US"))
                    : val.ToString("F", new System.Globalization.CultureInfo("en-US")); // DN
            return ConvertToString(v);
        }

        // Get Chart X SQL
        public string GetXSql(string fldsql, DataType fldtype, object? val, string dt)
        {
            var table = Table;
            string dbid = table.DbId;
            if (IsNumeric(dt))
                return fldsql + " = " + QuotedValue(UnformatDateTime(val, dt), fldtype, dbid);
            return dt switch {
                "y" => IsNumeric(val)
                    ? GroupSql(fldsql, "y", 0, dbid) + " = " + QuotedValue(val, DataType.Number, dbid)
                    : fldsql + " = " + QuotedValue(val, fldtype, dbid),
                "xyq" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2 && IsNumeric(ar[0]) && IsNumeric(ar[1])
                    ? GroupSql(fldsql, "y", 0, dbid) + " = " + QuotedValue(ar[0], DataType.Number, dbid) + " AND " + GroupSql(fldsql, "xq", 0, dbid) + " = " + QuotedValue(ar[1], DataType.Number, dbid)
                    : fldsql + " = " + QuotedValue(val, fldtype, dbid),
                "xym" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2 && IsNumeric(ar[0]) && IsNumeric(ar[1])
                    ? GroupSql(fldsql, "y", 0, dbid) + " = " + QuotedValue(ar[0], DataType.Number, dbid) + " AND " + GroupSql(fldsql, "xm", 0, dbid) + " = " + QuotedValue(ar[1], DataType.Number, dbid)
                    : fldsql + " = " + QuotedValue(val, fldtype, dbid),
                "xq" => GroupSql(fldsql, "xq", 0, dbid) + " = " + QuotedValue(val, DataType.Number, dbid),
                "xm" => GroupSql(fldsql, "xm", 0, dbid) + " = " + QuotedValue(val, DataType.Number, dbid),
                _ => fldsql + " = " + QuotedValue(val, fldtype, dbid)
            };
        }

        // Get Chart Series SQL
        public string GetSeriesSql(string fldsql, DataType fldtype, object? val, string dt)
        {
            var table = Table;
            string dbid = table.DbId;
            return dt switch {
                "syq" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2 && IsNumeric(ar[0]) && IsNumeric(ar[1])
                    ? GroupSql(fldsql, "y", 0, dbid) + " = " + QuotedValue(ar[0], DataType.Number, dbid) + " AND " + GroupSql(fldsql, "xq", 0, dbid) + " = " + QuotedValue(ar[1], DataType.Number, dbid)
                    : fldsql + " = " + QuotedValue(val, fldtype, dbid),
                "sym" => ConvertToString(val).Split('|') is var ar && ar.Length >= 2 && IsNumeric(ar[0]) && IsNumeric(ar[1])
                    ? GroupSql(fldsql, "y", 0, dbid) + " = " + QuotedValue(ar[0], DataType.Number, dbid) + " AND " + GroupSql(fldsql, "xm", 0, dbid) + " = " + QuotedValue(ar[1], DataType.Number, dbid)
                    : fldsql + " = " + QuotedValue(val, fldtype, dbid),
                "sq" => GroupSql(fldsql, "xq", 0, dbid) + " = " + QuotedValue(val, DataType.Number, dbid),
                "sm" => GroupSql(fldsql, "xm", 0, dbid) + " = " + QuotedValue(val, DataType.Number, dbid),
                _ => fldsql + " = " + QuotedValue(val, fldtype, dbid)
            };
        }

        // Format XML // DN
        public string FormatXml(string xml)
        {
            var document = new XmlDocument();
            document.Load(new StringReader(xml));
            XmlWriterSettings settings = new () { OmitXmlDeclaration = true, Indent = true };
            StringBuilder builder = new ();
            using var writer = XmlWriter.Create(builder, settings);
            document.Save(writer);
            return builder.ToString();
        }

        // Get renderAs
        public string GetRenderAs(int i)
        {
            string[] ar = SeriesRenderAs.Split(',');
            return (i < ar.Length) ? ar[i] : "";
        }

        // Has data
        public bool HasData => Data?.Any() ?? false;

        // Render chart
        public virtual async Task<IHtmlContent> Render(string className, int width = -1, int height = -1)
        {
            // Skip if ShowChart disabled
            if (!ShowChart)
                return new HtmlString("");

            // Skip if isAddOrEdit
            var page = CurrentPage;
            if (page != null) {
                var mi = GetMethod(page, "IsAddOrEdit");
                if (mi != null)
                    if (ConvertToBool(mi.Invoke(page, null)))
                        return new HtmlString("");
            }

            // Get renderer class
            Type rendererClass = ChartTypes.GetRendererClass();

            // Check chart size
            if (width <= 0)
                width = Width;
            if (height <= 0)
                height = Height;
            if (!IsNumeric(width) || width <= 0)
                width = ConvertToInt(rendererClass.GetField("DefaultWidth", BindingFlags.Static)?.GetValue(null));
            if (!IsNumeric(height) || height <= 0)
                height = ConvertToInt(rendererClass.GetField("DefaultHeight", BindingFlags.Static)?.GetValue(null));

            // Set up chart first
            Setup();

            // Output HTML
            className = AppendClass(className, ContainerClass); // Add container class
            string html = $"<div class=\"{className}\" data-chart=\"" + ID + "\">"; // Start chart

            // Load chart data
            await LoadData();
            LoadParameters();
            LoadViewData();

            // Disable animation if export
            if (IsExport()) {
                SetParameter("options.animation", false); // Disables all animations
                SetParameter("options.animations.colors", false); // Disables animation defined by the collection of 'colors' properties
                SetParameter("options.animations.x", false); // Disables animation defined by the 'x' property
                SetParameter("options.transitions.active.animation.duration", 0); // Disables the animation for 'active' mode
            }

            // Get renderer
            IChartRenderer renderer = Activator.CreateInstance(rendererClass, new object[] { this }) is IChartRenderer r
                ? r
                : throw new System.Exception("Failed to create instance from class " + rendererClass.Name);

            // Output chart html first
            bool isDrillDown = CurrentPage?.DrillDown ?? false;
            html += "<a id=\"cht_" + ID + "\"></a>" + // Anchor
                "<div id=\"div_cht_" + ID + "\" class=\"ew-chart" + (!DashboardReport && !Empty(PageBreakClass) ? " " + PageBreakClass : "") + "\">";
            // Not dashboard / run time sort / Not export / Not drilldown
            if (RunTimeSort && !isDrillDown && !IsExport() && HasData) {
                string url = AppPath(Table.DefaultRouteUrl + "/" + ChartVar);
                html += "<form class=\"row mb-3 ew-chart-sort\" action=\"" + url + "\"><div class=\"col-sm-auto\">" +
                    Language.Phrase("ChartOrder") +
                    "</div><div class=\"col-sm-auto\"><select id=\"chartordertype\" name=\"chartordertype\" class=\"form-select\" data-ew-action=\"chart-order\">" +
                    "<option value=\"1\"" + (SortType == "1" ? " selected" : "") + ">" + Language.Phrase("ChartOrderXAsc") + "</option>" +
                    "<option value=\"2\"" + (SortType == "2" ? " selected" : "") + ">" + Language.Phrase("ChartOrderXDesc") + "</option>" +
                    "<option value=\"3\"" + (SortType == "3" ? " selected" : "") + ">" + Language.Phrase("ChartOrderYAsc") + "</option>" +
                    "<option value=\"4\"" + (SortType == "4" ? " selected" : "") + ">" + Language.Phrase("ChartOrderYDesc") + "</option>" +
                    "</select>" +
                    "<input type=\"hidden\" id=\"width\" name=\"width\" value=\"" + ConvertToString(width) + "\">" +
                    "<input type=\"hidden\" id=\"height\" name=\"height\" value=\"" + ConvertToString(height) + "\">" +
                    "</div></form>";
            }
            html += renderer.GetContainer(width, height);
            html += "</div>";

            // Output JavaScript
            html += renderer.GetScript(width, height);
            html += "</div>"; // End chart span
            return new HtmlString(html);
        }
    }
} // End Partial class
