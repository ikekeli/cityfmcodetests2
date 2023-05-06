namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Report Field class
    /// </summary>
    public class ReportField : DbField
    {
        public object? SumValue; // Sum

        public object? AvgValue; // Average

        public object? MinValue; // Minimum

        public object? MaxValue; // Maximum

        public object? CntValue; // Count

        public object? SumViewValue; // Sum

        public object? AvgViewValue; // Average

        public object? MinViewValue; // Minimum

        public object? MaxViewValue; // Maximum

        public object? CntViewValue; // Count

        public string DrillDownTable = ""; // Drill down table name

        public string DrillDownUrl = ""; // Drill down URL

        public string CurrentFilter = ""; // Current filter in use

        public int GroupingFieldId = 0; // Grouping field id

        public bool ShowGroupHeaderAsRow = false; // Show grouping level as row

        public bool ShowCompactSummaryFooter = false; // Show compact summary footer

        public string GroupByType = ""; // Group By Type

        public string GroupInterval = ""; // Group Interval

        public string GroupSql = ""; // Group SQL

        public object? GroupViewValue; // Group View Value

        public string DateFilter = ""; // Date Filter

        public string Delimiter = ""; // Delimiter (e.g. comma) for delimiter separated value

        public List<object> DistinctValues = new ();

        public List<Dictionary<string, object>> Records = new ();

        public bool LevelBreak = false;

        private object? _groupValue;

        public ReportField(object table, string fieldVar, int type) : base(table, fieldVar, type)
        {
        }

        // Group value
        public object? GroupValue
        {
            get => _groupValue;
            set {
                SetDbValue(value);
                _groupValue = DbValue;
            }
        }

        // Get select options HTML (override)
        public override IHtmlContent SelectOptionListHtml(string name = "", bool? multiple = null) =>
            new HtmlString(((HtmlString)base.SelectOptionListHtml(name, multiple))?.Value?.Replace(">" + Config.InitValue + "</option>", "></option>")); // Do not show the init value as value

        // Get distinct values
        public void GetDistinctValues(List<Dictionary<string, object>> records, string sort = "ASC") =>
            DistinctValues = SameText(sort, "DESC")
                ? records.Select(row => row[GroupName]).Distinct().OrderByDescending(value => value).ToList()
                : records.Select(row => row[GroupName]).Distinct().OrderBy(value => value).ToList();

        // Get distinct records
        public void GetDistinctRecords(List<Dictionary<string, object>> records, object val) =>
            Records = records.Where(row => SameString(row[GroupName], val)).ToList();

        // Get sum
        public void GetSum(List<Dictionary<string, object>> records) =>
            SumValue = records.Any() ? records.Sum(row => ConvertToDecimal(row[GroupName])) : 0;

        // Get average
        public void GetAvg(List<Dictionary<string, object>> records) =>
            AvgValue = records.Any() ? records.Average(row => ConvertToDecimal(row[GroupName])) : 0;

        // Get min
        public void GetMin(List<Dictionary<string, object>> records) =>
            MinValue = records.Any() ? records.Min(row => row[GroupName]) : null;

        // Get max
        public void GetMax(List<Dictionary<string, object>> records) =>
            MaxValue = records.Any() ? records.Max(row => row[GroupName]) : null;

        // Get count
        public void GetCnt(List<Dictionary<string, object>> records)
        {
            int cnt = records.Any() ? records.Count(row => !Empty(row[GroupName])) : 0;
            CntValue = cnt;
            Count = cnt;
        }

        // Get group name // DN
        public string GroupName => !Empty(GroupSql) ? "EW_GROUP_VALUE_" + GroupingFieldId : Name;

        /// <summary>
        /// FormatAdvancedFilters
        /// </summary>
        /// <param name="ar">Advanced filter</param>
        /// <returns>Formatted advanced filter</returns>
        public List<Dictionary<string, object>> FormatAdvancedFilters(dynamic ar) { // DN
            if (IsList(ar) && IsList(AdvancedFilters)) {
                foreach (var arwrk in ar) {
                    if (arwrk.ContainsKey("lf") && arwrk.ContainsKey("df")) {
                        string lf = arwrk["lf"];
                        string df = arwrk["df"];
                        if (lf.StartsWith("@@") && SameString(lf, df) && IsList(AdvancedFilters)) {
                            string key = lf.Substring(2);
                            foreach (var filter in AdvancedFilters) {
                                if (filter.Enabled && SameString(key, filter.ID))
                                    arwrk["df"] = filter.Name;
                            }
                        }
                    }
                }
            }
            return ar;
        }
    }
} // End Partial class
