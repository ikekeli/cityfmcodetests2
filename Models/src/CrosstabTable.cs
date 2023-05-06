namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for crosstab
    /// </summary>
    public class CrosstabTable : ReportTable
    {
        // Column field related
        public string ColumnFieldName = "";

        public bool ColumnDateSelection = false;

        // Summary fields
        public List<SummaryField> SummaryFields = new ();

        public string SummarySeparatorStyle = "unstyled";

        // Summary cells
        public List<Attributes> SummaryCellAttrs = new ();

        public List<Attributes> SummaryViewAttrs = new ();

        public List<Attributes> SummaryLinkAttrs = new ();

        public object?[]? SummaryCurrentValues;

        public string?[]? SummaryViewValues;

        public int CurrentIndex = -1;

        // Summary cell attributes
        public string SummaryCellAttributes(int i)
        {
            if (i >= 0 && i < SummaryCellAttrs.Count) {
                var attrs = SummaryCellAttrs[i];
                return attrs.ToString();
            }
            return "";
        }

        // Summary view attributes
        public string SummaryViewAttributes(int i)
        {
            if (i >= 0 && i < SummaryViewAttrs.Count) {
                var attrs = SummaryViewAttrs[i];
                return attrs.ToString();
            }
            return "";
        }

        // Summary link attributes
        public string SummaryLinkAttributes(int i)
        {
            if (i >= 0 && i < SummaryLinkAttrs.Count) {
                var attrs = SummaryLinkAttrs[i];
                attrs.CheckLinkAttributes();
                return attrs.ToString();
            }
            return "";
        }

        // Render summary fields
        public string RenderSummaryFields(int idx)
        {
            string html = "";
            int cnt = SummaryFields.Count;
            foreach (SummaryField smry in SummaryFields) {
                var vv = smry.SummaryViewValues?[idx];
                // if (!IsFormatted(vv))
                //     vv = IsNullOrNotNumeric(vv)
                //         ? "&nbsp;" // Avoid empty li
                //         : FormatNumber(vv, Config.DefaultNumberFormat); // DN
                if (!Empty(smry.SummaryLinkAttrs[idx]["onclick"]) || !Empty(smry.SummaryLinkAttrs[idx]["href"]) || !Empty(smry.SummaryLinkAttrs[idx]["data-ew-action"])) // DN
                    vv = "<a" + smry.SummaryLinkAttributes(idx) + ">" + vv + "</a>";
                vv = "<span" + smry.SummaryViewAttributes(idx) + ">" + vv + "</span>";
                if (cnt > 0) {
                    if (!IsExport() || SameText(ExportType, "print"))
                        vv = "<li class=\"ew-value-" + smry.SummaryType.ToLowerInvariant() + "\">" + vv + "</li>";
                    else if (SameText(ExportType, "excel") && Config.UseExcelExtension || SameText(ExportType, "word") && Config.UseWordExtension) // DN
                        vv += " ";
                    else
                        vv += "<br>";
                }
                html += vv;
            }
            if (cnt > 0 && (!IsExport() || SameText(ExportType, "print")))
                html = "<ul class=\"list-" + SummarySeparatorStyle + " ew-crosstab-values\">" + html + "</ul>";
            return html;
        }

        // Render summary types
        public string RenderSummaryCaptions(string typ = "")
        {
            string html = "";
            int cnt = SummaryFields.Count;
            if (typ == "page") {
                return Language.Phrase("RptPageSummary");
            } else if (typ == "grand") {
                return Language.Phrase("RptGrandSummary");
            } else {
                foreach (SummaryField smry in SummaryFields) {
                    string st = smry.SummaryCaption;
                    var fld = FieldByName(smry.Name);
                    string caption = fld?.Caption ?? "";
                    if (caption != "")
                        st = caption + " (" + st + ")";
                    if (cnt > 0) {
                        if (!IsExport() || SameText(ExportType, "print"))
                            st = "<li>" + st + "</li>";
                        else if (SameText(ExportType, "excel") && Config.UseExcelExtension || SameText(ExportType, "word") && Config.UseWordExtension) // DN
                            st += "    ";
                        else
                            st += "<br>";
                    }
                    html += st;
                }
                if (cnt > 0 && (!IsExport() || SameText(ExportType, "print")))
                    html = "<ul class=\"list-" + SummarySeparatorStyle + " ew-crosstab-values\">" + html + "</ul>";
                return html;
            }
        }
    }
} // End Partial class
