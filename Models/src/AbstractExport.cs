namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Abstract class for export
    /// </summary>
    public abstract class AbstractExport : AbstractExportBase
    {
        public static string Selectors = "table.ew-table, table.ew-export-table, table.ew-chart, div.ew-chart, *.ew-export"; // Elements to be exported (table.ew-chart used by ExportPdf) // DN

        public string Line = "";

        public string Header = "";

        public string Style = "h"; // "v"(Vertical) or "h"(Horizontal)

        public bool Horizontal = true; // Horizontal

        public bool ExportCustom = false;

        public string StyleSheet = ""; // Style sheet path (relative to wwwroot folder)

        public virtual bool UseInlineStyles { get; set; } = false; // Use inline styles for page breaks

        public virtual bool ExportImages { get; set; } = true; // Allow exporting images

        public virtual bool ExportPageBreaks { get; set; } = true; // Page breaks when export

        public virtual bool ExportStyles { get; set; } = true; // CSS styles when export

        public int RowCount = 0;

        public int FieldCount = 0;

        // Constructor
        public AbstractExport(object? tbl = null) : base(tbl)
        {
            StyleSheet = Config.ProjectStylesheetFilename;
            ExportStyles = Config.ExportCssStyles;
        }

        /// <summary>
        /// Set style
        /// </summary>
        /// <param name="style">Export style (v = vertical, h = horizontal)</param>
        public virtual void SetStyle(string style)
        {
            style = style.ToLowerInvariant();
            if (SameString(style, "v") || SameString(style, "h"))
                Style = style;
            Horizontal = !SameString(Style, "v");
        }

        /// <summary>
        /// Set horizontal
        /// </summary>
        /// <param name="value">Whether export as horizontal</param>
        public virtual void SetHorizontal(bool value)
        {
            Horizontal = value;
            Style = Horizontal ? "h" : "v";
        }

        /// <summary>
        /// Export field caption
        /// </summary>
        /// <param name="fld">Field object</param>
        public virtual void ExportCaption(DbField fld)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            ExportValue(fld, fld.ExportCaption);
        }

        /// <summary>
        /// Export field value
        /// </summary>
        /// <param name="fld">Field object</param>
        public virtual void ExportValue(DbField fld) => ExportValue(fld, fld.ExportValue);

        /// <summary>
        /// Export field aggregate
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <param name="type">Aggregate type</param>
        public virtual void ExportAggregate(DbField fld, string type)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            if (Horizontal) {
                var val = "";
                if ((new[] {"TOTAL", "COUNT", "AVERAGE"}).Contains(type))
                    val = Language.Phrase(type) + ": " + fld.ExportValue;
                ExportValue(fld, val);
            }
        }

        /// <summary>
        /// Get meta tag for charset
        /// </summary>
        /// <returns>Meta tag for charset</returns>
        public virtual string CharsetMetaTag() => "<meta http-equiv=\"Content-Type\" content=\"text/html" + ((Config.Charset != "") ? "; charset=" + Config.Charset : "") + "\">\r\n";

        /// <summary>
        /// Export table header
        /// </summary>
        public virtual void ExportTableHeader() => Text.Append("<table class=\"ew-export-table\">");

        /// <summary>
        /// Cell styles
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <returns>Cell styles</returns>
        public virtual string CellStyles(DbField fld) => ExportStyles ? fld.CellStyles : "";

        /// <summary>
        /// Row styles
        /// </summary>
        /// <returns>Row styles</returns>
        public virtual string RowStyles() => ExportStyles ? Table?.RowStyles ?? "" : "";

        /// <summary>
        /// Export a value (caption, field value, or aggregate)
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <param name="val">Field value</param>
        public virtual void ExportValue(DbField fld, object val) =>
            Text.Append("<td" + CellStyles(fld) + ">" + ConvertToString(val) + "</td>");

        /// <summary>
        /// Begin a row
        /// </summary>
        /// <param name="rowCnt">Row count</param>
        public virtual void BeginExportRow(int rowCnt = 0)
        {
            RowCount++;
            FieldCount = 0;
            if (Horizontal) {
                string className = rowCnt switch {
                    -1 => "ew-export-table-footer",
                    0 => "ew-export-table-header",
                    _ => (rowCnt % 2 == 1) ? "ew-export-table-row" : "ew-export-table-alt-row"
                };
                Text.Append("<tr" + (ExportStyles ? " class=\"" + className + "\"" : "") + ">");
            }
        }

        /// <summary>
        /// End a row
        /// </summary>
        public virtual void EndExportRow(int rowCnt = 0)
        {
            if (Horizontal)
                Text.Append("</tr>");
        }

        /// <summary>
        /// Empty row
        /// </summary>
        public virtual void ExportEmptyRow()
        {
            RowCount++;
            Text.Append("<br>");
        }

        /// <summary>
        /// Page break
        /// </summary>
        public virtual void ExportPageBreak() {}

        /// <summary>
        /// Export field value
        /// </summary>
        /// <param name="fld">Field object</param>
        /// <returns>Field value</returns>
        public virtual object ExportFieldValue(DbField fld)
        {
            var exportValue = "";
            if (fld.ExportFieldImage && !Empty(fld.ExportHrefValue) && fld.Upload != null) { // Upload field
                // Note: Cannot show image, show empty content
                //if (!Empty(fld.Upload.DbValue))
                //    exportValue = GetFileATag(fld, fld.ExportHrefValue);
            } else {
                exportValue = fld.ExportValue;
            }
            return exportValue;
        }

        #pragma warning disable 1998
        /// <summary>
        /// Export a field
        /// </summary>
        /// <param name="fld">Field object</param>
        public virtual async Task ExportField(DbField fld)
        {
            if (!fld.Exportable)
                return;
            FieldCount++;
            var exportValue = ExportFieldValue(fld);
            if (Horizontal) {
                ExportValue(fld, exportValue);
            } else { // Vertical, export as a row
                RowCount++;
                Text.Append("<tr class=\"" + ((FieldCount % 2 == 1) ? "ew-export-table-row" : "ew-export-table-alt-row") + "\">" +
                    "<td" + CellStyles(fld) + ">" + fld.ExportCaption + "</td>");
                Text.Append("<td" + CellStyles(fld) + ">" + exportValue + "</td></tr>");
            }
        }
        #pragma warning restore 1998

        /// <summary>
        /// Table Footer
        /// </summary>
        public virtual void ExportTableFooter() => Text.Append("</table>");

        #pragma warning disable 1998
        /// <summary>
        /// Add HTML tags
        /// </summary>
        public virtual async Task ExportHeaderAndFooter()
        {
            string header = "<html><head>" + CharsetMetaTag() + "<style>" + await Styles() + "</style></head><body>";
            Text.Insert(0, header);
            Text.Append("</body></html>");
        }

        /// <summary>
        /// Get CSS styles
        /// </summary>
        public virtual async Task<string> Styles()
        {
            if (ExportStyles && !Empty(StyleSheet))
                return await LoadText(StyleSheet);
            return String.Empty;
        }
        #pragma warning restore 1998

        /// <summary>
        /// Adjust page break
        /// </summary>
        /// <param name="doc">Document object</param>
        protected virtual void AdjustPageBreak(ref dynamic doc)
        {
            // Remove empty charts
            foreach (var div in doc.QuerySelectorAll("div.ew-chart")) {
                var parent = div.ParentElement;
                var script = parent?.QuerySelector("script");
                if (script != null) // Remove script for chart
                    script.Remove();
                var img = div.QuerySelector("img");
                if (img == null) // No image inside => Remove
                    div.Remove();
            }

            // Remove empty cards
            foreach (var div in doc.QuerySelectorAll("div.card")) {
                var selector = div.QuerySelector(Selectors);
                if (selector == null) // Nothing to export => Remove
                    div.Remove();
            }

            // Find and process all elements to be exported
            var body = doc.QuerySelector("body");
            if (body != null) {
                bool pageBreak = Table?.ExportPageBreaks ?? ExportPageBreaks;
                var elements = body.QuerySelectorAll(Selectors);
                int i = 0;
                int cnt = 0;
                foreach (var element in elements)
                    cnt++;
                foreach (var element in elements) {
                    string classes = element.GetAttribute("class") ?? "";
                    string style = element.GetAttribute("style") ?? "";
                    Dictionary<string, string> styles = style.Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Split(':'))
                        .Where(p => p.Length == 2)
                        .ToDictionary(p => p[0].Trim(), p => p[1].Trim());
                    if (UseInlineStyles) // Use inline styles
                        classes = RemoveClass(classes, "break-before-page break-after-page"); // Remove classes
                    else { // Use classes
                        styles.Remove("page-break-before");
                        styles.Remove("page-break-after");
                    }
                    if (i == 0) { // First element, remove page break before content
                        if (UseInlineStyles)
                            styles.Remove("page-break-before");
                        else
                            classes = RemoveClass(classes, "break-before-page");
                    } else if (i == cnt - 1) { // Last element, remove page break after content
                        if (UseInlineStyles) {
                            if (pageBreak)
                                if (styles.ContainsKey("page-break-before"))
                                    styles["page-break-before"] = "always";
                                else
                                    styles.Add("page-break-before", "always");
                            styles.Remove("page-break-after");
                        } else {
                            classes = pageBreak ? AppendClass(classes, "break-before-page") : RemoveClass(classes, "break-before-page");
                            classes = RemoveClass(classes, "break-after-page");
                        }
                    } else {
                        if (UseInlineStyles) {
                            if (pageBreak)
                                if (styles.ContainsKey("page-break-before"))
                                    styles["page-break-before"] = "always";
                                else
                                    styles.Add("page-break-before", "always");
                            else
                                styles.Remove("page-break-before");
                            styles.Remove("page-break-after");
                        } else {
                            classes = pageBreak ? AppendClass(classes, "break-before-page") : RemoveClass(classes, "break-before-page");
                            classes = RemoveClass(classes, "break-after-page");
                        }
                    }
                    style = String.Join(';', styles.Select(p => p.Key + ":" + p.Value));
                    element.SetAttribute("class", classes);
                    element.SetAttribute("style", style);
                    i++;
                }
            }
        }

        /// <summary>
        /// Get Document
        /// </summary>
        /// <param name="html">Html</param>
        /// <returns>Document</returns>
        public virtual dynamic GetDocument(string? html = null) =>
            new AngleSharp.Html.Parser.HtmlParser().ParseDocument(html ?? Text.ToString());

        /// <summary>
        /// Set Document
        /// </summary>
        /// <param name="doc">Document</param>
        public virtual void SetDocument(dynamic doc)
        {
            Text.Clear();
            Text.Append(doc.DocumentElement.OuterHtml);
        }

        /// <summary>
        /// Adjust HTML before export (to be called in Export())
        /// </summary>
        protected virtual async Task AdjustHtml()
        {
            if (!Text.ToString().Contains("</body>"))
                await ExportHeaderAndFooter(); // Add header and footer to Text
            var doc = GetDocument(); // Load Text again

            // Remove Images
            if (!ExportImages) {
                foreach (var img in doc.QuerySelectorAll("img"))
                    img.Remove();
            }

            // Adjust page break
            AdjustPageBreak(ref doc);

            // Grid and table container
            foreach (var div in doc.QuerySelectorAll("div[class*='ew-grid'], div[class*='table-responsive']")) // div.ew-grid(-middle-panel), div.table-responsive(-sm|-md|-lg|-xl)
                div.RemoveAttribute("class");

            // Table
            foreach (AngleSharp.Dom.IElement table in doc.QuerySelectorAll(".ew-table, .ew-export-table")) {
                string classNames = table.GetAttribute("class") ?? "";
                bool noBorder = ContainsClass(classNames, "no-border");
                if (ContainsClass(classNames, "ew-table")) {
                    if (UseInlineStyles)
                        classNames = "ew-export-table"; // Use single class (for MS Word/Excel)
                    else
                        classNames = AppendClass(RemoveClass(classNames, "break-before-page break-after-page"), "ew-export-table");
                    table.SetAttribute("class", classNames);
                }
                string style = table.GetAttribute("style") ?? "";
                style += (Empty(style) ? "" : "; ") + "border-collapse: collapse;"; // Set border-collapse
                table.SetAttribute("style", style);
                Dictionary<string, string> cellStyles = Config.ExportTableCellSyles;
                if (noBorder)
                    cellStyles["border"] = "0";
                foreach (var row in table.QuerySelectorAll("tr")) {
                    foreach (var cell in row.QuerySelectorAll("td, th")) {
                        foreach (var cellStyle in cellStyles) // Add cell styles
                            cell.SetAttribute("style", String.Join(";", Config.ExportTableCellSyles.Select(kv => kv.Key + ":" + kv.Value)));
                    }
                }
            }
            SetDocument(doc);
        }

        /// <summary>
        /// Load HTML
        /// </summary>
        /// <param name="html">Html</param>
        public virtual void LoadHtml(string html)
        {
            Text.Append(html);
        }

        /// <summary>
        /// Add image (virtual)
        /// </summary>
        /// <param name="imageFile">Image file name</param>
        /// <param name="breakType">Page break type (before / after)</param>
        public virtual void AddImage(string imageFile, string breakType = "")
        {
            // To be implemented by subclass
        }

        // Output as string
        public override string ToString() => Text.ToString();
    }
} // End Partial class
