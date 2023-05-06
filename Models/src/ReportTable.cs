namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Report Table base class
    /// </summary>
    public class ReportTable : DbTableBase
    {
        public string ReportSourceTable = "";

        public string ReportSourceTableName = ""; // DN

        public RowSummary RowTotalType; // Row total type

        public RowTotal RowTotalSubType; // Row total subtype

        public int RowGroupLevel; // Row group level

        public bool ShowReport = true;

        // Constructor
        public ReportTable()
        {
            ShowDrillDownFilter = Config.ShowDrillDownFilter;
            UseDrillDownPanel = Config.UseDrillDownPanel;
        }

        // Group Per Page // DN
        public int GroupPerPage
        {
            get => Session.GetInt(Config.ProjectName + "_" + TableVar + "_" + Config.TableRecordsPerPage);
            set => Session.SetInt(Config.ProjectName + "_" + TableVar + "_" + Config.TableRecordsPerPage, value);
        }

        // Start Group // DN
        public int SessionStartGroup
        {
            get => Session.GetInt(Config.ProjectName + "_" + TableVar + "_" + TableReportType + "_" + Config.TableStartRec);
            set => Session.SetInt(Config.ProjectName + "_" + TableVar + "_" + TableReportType + "_" + Config.TableStartRec, value);
        }

        // Order By
        public string OrderBy
        {
            get => Session.GetString(Config.ProjectName + "_" + TableVar + "_" + Config.TableOrderBy);
            set => Session.SetString(Config.ProjectName + "_" + TableVar + "_" + Config.TableOrderBy, value);
        }

        // Session Order By (for non-grouping fields)
        public string DetailOrderBy
        {
            get => Session.GetString(Config.ProjectName + "_" + TableVar + "_" + Config.TableDetailOrderBy);
            set => Session.SetString(Config.ProjectName + "_" + TableVar + "_" + Config.TableDetailOrderBy, value);
        }

        #pragma warning disable 108
        // Get field object by name
        public ReportField? FieldByName(string name) => (ReportField)Fields.FirstOrDefault(kvp => kvp.Key == name).Value;

        // Get field object by parm
        public ReportField? FieldByParam(string parm) => (ReportField)Fields.FirstOrDefault(kvp => kvp.Value.Param == parm).Value;
        #pragma warning restore 108
    }
} // End Partial class
