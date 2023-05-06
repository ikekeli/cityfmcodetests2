namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Pager class
    /// </summary>
    public abstract class Pager
    {
        public PagerItem NextButton;

        public PagerItem FirstButton;

        public PagerItem PrevButton;

        public PagerItem LastButton;

        public int PageSize;

        public int FromIndex;

        public int ToIndex;

        public int RecordCount;

        public int Range;

        public bool Visible = true;

        public bool AutoHidePager = true;

        public bool AutoHidePageSizeSelector = true;

        public bool UsePageSizeSelector = true;

        public string PageSizes = "";

        public string ItemPhraseId = "Record";

        public dynamic Table;

        public string PageNumberName;

        public string PagePhraseId = "Page";

        public string ContextClass = "";

        private bool PageSizeAll = false;

        // Constructor
        public Pager(object table, int fromIndex, int pageSize, int recordCount, string pageSizes = "", int range = 10, bool? autoHidePager = null, bool? autoHidePageSizeSelector = null, bool? usePageSizeSelector = null)
        {
            Table = table;
            ContextClass = CheckClassName(Table.TableVar);
            AutoHidePager = autoHidePager ?? Config.AutoHidePager;
            AutoHidePageSizeSelector = autoHidePageSizeSelector ?? Config.AutoHidePageSizeSelector;
            UsePageSizeSelector = usePageSizeSelector ?? true;
            FromIndex = fromIndex;
            PageSize = pageSize;
            RecordCount = recordCount;
            Range = range;
            PageSizes = pageSizes;
            // Handle page size = 0
            if (PageSize == 0)
                PageSize = RecordCount;
            // Handle page size = -1 (ALL)
            if (PageSize == -1 || PageSize == RecordCount) {
                PageSizeAll = true;
                PageSize = RecordCount;
            }
            PageNumberName = Config.TablePageNumber;
            FirstButton = new PagerItem(ContextClass, pageSize);
            PrevButton = new PagerItem(ContextClass, pageSize);
            NextButton = new PagerItem(ContextClass, pageSize);
            LastButton = new PagerItem(ContextClass, pageSize);
        }

        // Render
        public virtual HtmlString Render()
        {
            string html = "";
            if (Visible && RecordCount > 0) {
                // Do not show record numbers for View/Edit page
                if (PagePhraseId != "Record") {
                    html += $@"<div class=""ew-pager ew-rec"">
                        <div class=""d-inline-flex"">
                            <div class=""ew-pager-rec me-1"">{Language.Phrase(ItemPhraseId)}</div>
                            <div class=""ew-pager-start me-1"">{FormatInteger(FromIndex)}</div>
                            <div class=""ew-pager-to me-1"">{Language.Phrase("To")}</div>
                            <div class=""ew-pager-end me-1"">{FormatInteger(ToIndex)}</div>
                            <div class=""ew-pager-of me-1"">{Language.Phrase("Of")}</div>
                            <div class=""ew-pager-count me-1"">{FormatInteger(RecordCount)}</div>
                        </div>
                    </div>";
                }
                // Page size selector
                if (UsePageSizeSelector && !Empty(PageSizes) && !(AutoHidePageSizeSelector && RecordCount <= PageSize)) {
                    var pageSizes = PageSizes.Split(',');
                    var options = pageSizes.Select(pageSize => {
                        if (Int32.TryParse(pageSize, out int ps) && ps > 0) {
                            return $@"<option value=""{pageSize}""" + (PageSize == ps ? " selected" : "") + $">{FormatInteger(pageSize)}</option>";
                        } else {
                            return @"<option value=""ALL""" + (PageSizeAll ? " selected" : "") + $">{Language.Phrase("AllRecords")}</option>";
                        }
                    });
                    string url = CurrentDashboardPageUrl();
                    string ajax = Table.UseAjaxActions ? "true" : "false";
                    html += $@"<div class=""ew-pager"">
                        <select name=""{Config.TableRecordsPerPage}"" class=""form-select form-select-sm ew-tooltip"" title=""{Language.Phrase("RecordsPerPage")}"" data-ew-action=""change-page-size"" data-ajax=""{ajax}"" data-url=""{url}"">{String.Join("\t\t", options)}</select>
                    </div>";
                }
            }
            return new HtmlString(html);
        }
    }
} // End Partial class
