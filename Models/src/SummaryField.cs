namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// SummaryField class
    /// </summary>
    public class SummaryField : ReportField
    {
        public string SummaryType = "";

        public string SummaryCaption = "";

        public List<Attributes> SummaryViewAttrs = new ();

        public List<Attributes> SummaryLinkAttrs = new ();

        public object?[]? SummaryCurrentValues;

        public string?[]? SummaryViewValues;

        public object?[]? SummaryValues;

        public int[]? SummaryValueCounts;

        public object?[]? SummaryGroupValues;

        public int[]? SummaryGroupValueCounts;

        public object? SummaryInitialValue;

        public object? SummaryRowSummary;

        public int SummaryRowCount;

        // Constructor
        public SummaryField(string tableVar, string fieldVar, int type) : base(tableVar, fieldVar, type)
        {
        }

        // Summary view attributes
        public string SummaryViewAttributes(int i)
        {
            if (IsList(SummaryViewAttrs)) {
                if (i >= 0 && i < SummaryViewAttrs.Count) {
                    var attrs = SummaryViewAttrs[i];
                    return attrs.ToString();
                }
            }
            return "";
        }

        // Summary link attributes
        public string SummaryLinkAttributes(int i)
        {
            if (IsList(SummaryViewAttrs)) {
                if (i >= 0 && i < SummaryViewAttrs.Count) {
                    var attrs = SummaryViewAttrs[i];
                    attrs.CheckLinkAttributes();
                    return attrs.ToString();
                }
            }
            return "";
        }
    }
} // End Partial class
