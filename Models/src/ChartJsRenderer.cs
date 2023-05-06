namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Chart.js Renderer class
    /// </summary>
    public class ChartJsRenderer : IChartRenderer
    {
        public DbChart Chart;

        public Dictionary<string, dynamic?> Data = new ();

        public Dictionary<string, dynamic?> Options = new ();

        public static int DefaultWidth = 600;

        public static int DefaultHeight = 500;

        // Constructor
        public ChartJsRenderer(DbChart chart) => Chart = chart;

        // Get chart canvas
        // Get chart canvas
        public string GetContainer(int width, int height)
        {
            width = (width > 0) ? width : DefaultWidth;
            height = (height > 0) ? height : DefaultHeight;
            return $"<div id=\"div_{Chart.ID}\" class=\"ew-chart-container\"><canvas id=\"chart_{Chart.ID}\" width=\"{width}\" height=\"{height}\" class=\"ew-chart-canvas\"></canvas></div>";
        }

        // Get chart JavaScript
        public string GetScript(int width, int height)
        {
            bool drilldown = Chart.DrillDownInPanel;
            string typ = !Empty(Chart.Type) ? Chart.Type : ChartTypes.DefaultType; // Chart type (nnnn)
            string id = Chart.ID; // Chart ID
            // scroll = Chart.ScrollChart; // Not supported
            // trends = Chart.Trends;
            // series = Chart.Series;
            // align = Chart.Align;
            string chartType = ChartTypes.GetName(typ); // Chart type name
            string canvasId = "chart_" + id;
            LoadChart();
            var chartData = new { type = chartType, data = Data, options = Options };

            // Output JavaScript for Chart.js
            string dataformat = Chart.DataFormat;
            string chartid = "chart_" + id + (drilldown ? "_" + Random() : "");
            var yFieldFormat = Chart.YFieldFormat;
            var yAxisFormat = Chart.YAxisFormat;
            Dictionary<string, object?> args = new () {
                { "id", id },
                { "canvasId", canvasId },
                { "chartJson", chartData },
                { "yFieldFormat", yFieldFormat },
                { "yAxisFormat", yAxisFormat },
                { "useDrilldownPanel", null }
            };
            if (!Empty(Chart.DrillDownUrl))
                args["useDrilldownPanel"] = Chart.UseDrillDownPanel;
            if (Chart.IsPieChart || Chart.IsDoughnutChart)
                args["showPercentage"] = Chart.ShowPercentage;
            string json = ConvertToJson(args, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });
            string wrk = "<script>ew.createChart(" + json + ");</script>";
            // Show data for debug
            if (Config.Debug)
                SetDebugMessage("(Chart JSON):<pre>" + HtmlEncode(JsonConvert.SerializeObject(chartData, Newtonsoft.Json.Formatting.Indented)) + "</pre>");
            return wrk;
        }

        // Load chart
        protected void LoadChart()
        {
            string chtType = Chart.LoadParameter("type");
            List<dynamic> chartSeries = Chart.Series;
            List<Dictionary<string, object?>> chartData = Chart.ViewData;
            int multiSeries = Chart.IsSingleSeries ? 0 : 1; // $multiSeries = 1 (Multi series charts)
            string seriesType = Chart.LoadParameter("seriestype");

            // Load default options
            Options = Chart.Parameters.Get("options") ?? Options;

            // chartjs-plugin-datalabels options
            // https://chartjs-plugin-datalabels.netlify.app/guide/options.html
            Options = Merge(Options, new { plugins = new { datalabels = new { clamp = true } } });
            string title = Chart.LoadParameter("caption");

            // Init X/Y Axes
            Dictionary<string, dynamic?> yAxes = new ();
            Dictionary<string, dynamic?> x = new ();
            Dictionary<string, dynamic?> y = new ();
            Dictionary<string, dynamic?> scale = Chart.Parameters.Get("scale") ?? new Dictionary<string, dynamic?>(); // Default bar chart scale
            Dictionary<string, dynamic?> vscale = new ();

            // Set up beginAtZero/min/max
            Dictionary<string, dynamic?> vscales = new ();
            vscale["beginAtZero"] = Chart.ScaleBeginWithZero;
            if (Chart.MinValue != null)
                vscale["min"] = Chart.MinValue;
            if (Chart.MaxValue != null)
                vscale["max"] = Chart.MaxValue;
            if (IsList(chartData)) {
                // Multi series
                if (multiSeries == 1) {
                    List<string> labels = new ();
                    List<Dictionary<string, dynamic?>> datasets = new ();

                    // Multi-Y values
                    if (seriesType == "1") {
                        // Set up labels
                        int cntCat = chartData.Count;
                        labels = chartData.Select(item => Chart.FormatName(item.Values.First())).ToList();

                        // Set up datasets
                        int cntData = chartData.Count;
                        int cntSeries = chartSeries.Count;
                        if (cntData > 0 && IsList(chartData[0]) && cntSeries > chartData[0].Count - 2) // DN
                            cntSeries = chartData[0].Count - 2;
                        for (int i = 0; i < cntSeries; i++) {
                            string seriesName = IsList(chartSeries[i]) ? chartSeries[i][0] : ConvertToString(chartSeries[i]);
                            string yAxisId = IsList(chartSeries[i]) ? chartSeries[i][1] : "";
                            if (!EmptyString(yAxisId) && !yAxes.ContainsKey(yAxisId)) { // Dual axis
                                Dictionary<string, dynamic?> p = new () {
                                    { "type", "linear" },
                                    { "display", true },
                                    { "position", yAxisId == "y" ? "left" : "right"}
                                };
                                if (yAxisId != "y")
                                    p.Add("grid", new { drawOnChartArea = false });
                                yAxes.Add(yAxisId, p);
                            }
                            string color = Chart.GetPaletteRgbaColor(i);
                            string renderAs = Chart.GetRenderAs(i);
                            bool showSeries = Config.ChartShowBlankSeries;
                            var data = new List<double>();
                            var links = new List<Dictionary<string, string>>();
                            for (int j = 0; j < cntData; j++) {
                                double val = ConvertToDouble(chartData[j].Values.ToList()[i + 2]);
                                if (val != 0)
                                    showSeries = true;
                                var lnk = GetChartLink(Chart.DrillDownUrl, Chart.Data[j]);
                                links.Add(lnk);
                                data.Add(val);
                            }
                            if (showSeries) {
                                var dataset = GetDataset(data, color, links, seriesName, renderAs, yAxisId);
                                datasets.Add(dataset);
                            }
                        }

                    // Series field
                    } else {
                        // Get series names
                        int cntSeries = IsList(chartSeries) ? chartSeries.Count : 0;

                        // Set up labels
                        int cntData = chartData.Count;
                        labels = chartData.Select(item => ConvertToString(item.Values.First())).Distinct().ToList();

                        // Set up dataset
                        int cntLabels = labels.Count();
                        for (int i = 0; i < cntSeries; i++) {
                            string seriesName = IsList(chartSeries[i]) ? chartSeries[i][0] : ConvertToString(chartSeries[i]);
                            string yAxisId = IsList(chartSeries[i]) ? chartSeries[i][1] : "";
                            if (!EmptyString(yAxisId) && !yAxes.ContainsKey(yAxisId)) { // Dual axis
                                Dictionary<string, dynamic?> p = new () {
                                    { "type", "linear" },
                                    { "display", true },
                                    { "position", yAxisId == "y" ? "left" : "right"}
                                };
                                if (yAxisId != "y")
                                    p.Add("grid", new { drawOnChartArea = false });
                                yAxes.Add(yAxisId, p);
                            }
                            string color = Chart.GetPaletteRgbaColor(i);
                            string renderAs = Chart.GetRenderAs(i);
                            bool showSeries = Config.ChartShowBlankSeries;
                            List<double?> data = new ();
                            List<Dictionary<string, string>> links = new ();
                            for (int j = 0; j < cntLabels; j++) {
                                double? val = Config.ChartShowMissingSeriesValuesAsZero ? 0 : null;
                                for (int k = 0; k < cntData; k++) {
                                    var values = chartData[k].Values.ToList();
                                    if (SameString(values[0], labels[j]) && SameString(values[1], seriesName)) {
                                        val = ConvertToDouble(values[2]);
                                        if (val != 0)
                                            showSeries = true;
                                        var lnk = GetChartLink(Chart.DrillDownUrl, Chart.Data[k]);
                                        links.Add(lnk);
                                        break;
                                    }
                                }
                                data.Add(val);
                            }
                            if (showSeries) {
                                var dataset = GetDataset(data, color, links, seriesName, renderAs, yAxisId);
                                datasets.Add(dataset);
                            }
                        }
                    }

                    // Set up Data/Options
                    Data["labels"] = labels;
                    Data["datasets"] = datasets;
                    Options = Merge(Options, new {
                        responsive = false,
                        legend = new { display = true },
                        title = new { display = true, text = title }
                    });

                    // Set up tooltips for stacked charts
                    if (Chart.IsStackedChart)
                        Options["interaction"] = new { mode = "index" };

                    // Set up X/Y Axes
                    if (Chart.IsCombinationChart) {
                        if (scale.Count > 0)
                            x = scale;
                        if (vscale.Count > 0)
                            y = vscale;
                    } else {
                        var stack = new Dictionary<string, dynamic?> { {"stacked", Chart.IsStackedChart} };
                        var dx = new Dictionary<string, dynamic?>(stack);
                        var dy = new Dictionary<string, dynamic?>(stack);
                        if (Chart.IsBarChart) {
                            dx = Merge(vscale, dx);
                            dy = Merge(scale, dy);
                        } else {
                            dx = Merge(scale, dx);
                            dy = Merge(vscale, dy);
                        }
                        if (dx.Count > 0)
                            x = dx;
                        if (dy.Count > 0)
                            y = dy;
                    }

                // Single series
                } else {
                    int cntData = chartData.Count;
                    List<string> labels = new ();
                    object backgroundColor = new List<string>();
                    List<double?> data = new ();
                    List<Dictionary<string, string>> links = new ();
                    for (int i = 0; i < cntData; i++) {
                        var values = chartData[i].Values.ToList();
                        string name = Chart.FormatName(values[0]);
                        string color = Chart.GetPaletteRgbaColor(i);
                        if (!Empty(values[1]))
                            name += ", " + values[1];
                        double? val = values[2] == null ? null : ConvertToDouble(values[2]);
                        var lnk = GetChartLink(Chart.DrillDownUrl, Chart.Data[i]);
                        links.Add(lnk);
                        labels.Add(name);
                        ((List<string>)backgroundColor).Add(color);
                        data.Add(val);
                    }

                    // Set bar defaults
                    if (Chart.IsBarChart) {
                        if (scale.Count > 0)
                            y = scale;
                        if (vscale.Count > 0)
                            x = vscale;
                    } else {
                        if (scale.Count > 0)
                            x = scale;
                        if (vscale.Count > 0)
                            y = vscale;
                    }

                    // Line/Area chart, use first color
                    if (Chart.IsLineChart || Chart.IsAreaChart)
                        backgroundColor = Chart.GetPaletteRgbaColor(0); // Use first color

                    // Get dataset
                    List<Dictionary<string, dynamic?>> datasets = cntData > 0 ? new () { GetDataset(data, backgroundColor, links) } : new ();

                    // Set up Data/Options
                    Data["labels"] = labels;
                    Data["datasets"] = datasets;
                    bool showLegend = Chart.IsPieChart || Chart.IsDoughnutChart ? true : false;
                    Options = Merge(Options, new {
                        responsive = false,
                        plugins = new {
                            legend = new { display = showLegend },
                            title = new { display = true, text = title }
                        }
                    });
                }

                // Set up indexAxis = y for horizontal bar charts
                if (Chart.IsBarChart)
                    Options["indexAxis"] = "y";

                // Set X / Y Axes
                Dictionary<string, dynamic?> scales = new ();
                if (x.Count > 0)
                    scales["x"] = x;
                if (y.Count > 0)
                    scales["y"] = y;
                if (yAxes.Count > 0)
                    scales = Merge(scales, yAxes);
                Options["scales"] = scales;

                // Set up trend lines
                var annotations = GetAnnotations();
                if (annotations != null)
                    Options = Merge(Options, new { plugins = new { annotation = annotations } });
            }

            // Chart Rendered event
            ChartRendered(this);
        }

        // Get annotations
        protected object? GetAnnotations()
        {
            if (Chart.Trends.Count > 0) {
                Dictionary<string, object> d = new ();
                for (int i = 0; i < Chart.Trends.Count; i++)
                    d.Add("annot" + (i + 1), GetAnnotation(Chart.Trends[i]));
                return new { annotations = d };
            }
            return null;
        }

        // Get annotation
        protected Annotation GetAnnotation(Annotation annot)
        {
            annot.BorderColor = GetRgbaColor(annot.BorderColor, GetOpacity(annot.Alpha)); // Color
            object label = annot.Label;
            bool display = !Empty(label);
            annot.Label = new Dictionary<string, object> {
                { "content", label },
                { "backgroundColor", annot.BorderColor },
                { "display", display },
                { "enabled", true },
                { "position", IsRTL ? "left" : "right" },
            }; // Display value (label)
            annot.ScaleID = (Chart.IsBarChart ? "x" : "y") + (annot.ParentYAxis == "S" ? "1" : ""); // Axis type + Secondary/Primary axis id
            annot.BorderDash ??= new int[] {};
            return annot;
        }

        // Get chart link
        protected Dictionary<string, string> GetChartLink(string src, Dictionary<string, object> row)
        {
            if (!Empty(src) && row != null) {
                int cntrow = row.Count;
                string lnk = src;
                var sdt = Chart.SeriesDateType;
                var xdt = Chart.XAxisDateFormat;
                if (!Empty(sdt))
                    xdt = sdt;
                var m = Regex.Match(lnk, "&t=([^&]+)&");
                string tblCaption = (m.Success) ? Language.TablePhrase(m.Groups[1].Value, "TblCaption") : "";
                var values = row.Values.ToList();
                for (int i = 0; i < cntrow; i++) { // Link format: %i:Parameter:FieldType%
                    m = Regex.Match(lnk, $@"%{i}:([^%:]*):([\d]+)%");
                    if (m.Success) {
                        DataType fldtype = FieldDataType(ConvertToInt(m.Groups[2].Value));
                        lnk = i switch {
                            0 => lnk.Replace(m.Groups[0].Value, Encrypt(Chart.GetXSql("@" + m.Groups[1].Value, fldtype, values[i], xdt))), // Format X SQL
                            1 => lnk.Replace(m.Groups[0].Value, Encrypt(Chart.GetSeriesSql("@" + m.Groups[1].Value, fldtype, values[i], sdt))), // Format Series SQL
                            _ => lnk.Replace(m.Groups[0].Value, Encrypt("@"  + m.Groups[1].Value + " = " + QuotedValue(values[i], fldtype, Chart.Table.DbId)))
                        };
                    }
                }
                return new () { {"url", lnk}, {"id", Chart.ID}, {"hdr", tblCaption} };
            }
            return new ();
        }

        // Get dataset
        protected Dictionary<string, dynamic?> GetDataset(object data, dynamic color, List<Dictionary<string, string>> links, object? seriesName = null, string renderAs = "", string yAxisId = "")
        {
            Dictionary<string, dynamic?> dataset = Merge(new Dictionary<string, dynamic?>(), Chart.Parameters.Get("dataset")); // Load default dataset options
            dataset["data"] = data; // Load data
            dataset["backgroundColor"] = color; // Background color
            dataset["borderWidth"] = 1;
            dataset["borderColor"] = color switch {
                List<string> colors => colors.Select(c => changeAlpha(c)),
                string str => changeAlpha(str),
                _ => null
            };
            bool hasLink = links.Any();
            dataset["links"] = hasLink ? links : null; // Drill down link
            if (seriesName != null) { // Multi series
                dataset["label"] = seriesName;
                if (Chart.IsCombinationChart) { // Combination chart, set render type / stack id / axis id
                    string renderType = GetRenderType(renderAs);
                    dataset["type"] = renderType;
                    if (renderType == "bar" && Chart.IsStackedChart) // Set up stack id
                        dataset["stack"] = Chart.ID;
                    if (Chart.IsDualAxisChart) // Set up axis id
                        dataset["yAxisID"] = yAxisId;
                } else if (Chart.IsStackedChart) { // Stacked chart, set up stack id
                    dataset["stack"] = Chart.ID;
                }
            }
            if (Chart.IsAreaChart || Chart.IsCombinationChart && SameText(renderAs, "area")) // Area chart, set fill
                dataset["fill"] = true;
            return dataset;

            // Change alpha
            string changeAlpha(string c) => Regex.Replace(c, @"[\d\.]+(?=\))", "1.0"); // Change alpha to 1.0
        }

        // Get render type for combination chart
        protected string GetRenderType(string renderAs) =>
            renderAs.ToLower() switch {
                "line" => "line",
                "area" => Chart.IsStackedChart ? "bar" : "line",
                _ => "bar"
            };
    }
} // End Partial class
