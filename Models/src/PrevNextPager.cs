namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for PrevNext pager
    /// </summary>
    public class PrevNextPager : Pager
    {
        public int PageCount;

        public int CurrentPageNumber;

        public bool Modal;

        public string Url;

        // Constructor
        public PrevNextPager(object table, int fromIndex, int pageSize, int recordCount, string pageSizes = "", int range = 10, bool? autoHidePager = null, bool? autoHidePageSizeSelector = null, bool? usePageSizeSelector = null, bool isModal = false, string url = "") :
            base(table, fromIndex, pageSize, recordCount, pageSizes, range, autoHidePager, autoHidePageSizeSelector, usePageSizeSelector)
        {
            FirstButton = new PagerItem(ContextClass, pageSize);
            PrevButton = new PagerItem(ContextClass, pageSize);
            NextButton = new PagerItem(ContextClass, pageSize);
            LastButton = new PagerItem(ContextClass, pageSize);
            Modal = isModal;
            Url = url;
            Init();
        }

        // Method to init pager
        public void Init()
        {
            int tempIndex;
            if (PageSize > 0) {
                CurrentPageNumber = (FromIndex - 1) / PageSize + 1;
                if (CurrentPageNumber <= 0) // Make sure page number >= 1
                    CurrentPageNumber = 1;
                PageCount = (RecordCount - 1) / PageSize + 1;
                if (AutoHidePager && PageCount == 1)
                    Visible = false;
                if (FromIndex > RecordCount)
                    FromIndex = RecordCount;
                ToIndex = FromIndex + PageSize - 1;
                if (ToIndex > RecordCount)
                    ToIndex = RecordCount;
                // First Button
                tempIndex = 1;
                FirstButton.Start = tempIndex;
                FirstButton.Enabled = (tempIndex != FromIndex);
                // Prev Button
                tempIndex = FromIndex - PageSize;
                if (tempIndex < 1)
                    tempIndex = 1;
                PrevButton.Start = tempIndex;
                PrevButton.Enabled = (tempIndex != FromIndex);
                // Next Button
                tempIndex = FromIndex + PageSize;
                if (tempIndex > RecordCount)
                    tempIndex = FromIndex;
                NextButton.Start = tempIndex;
                NextButton.Enabled = (tempIndex != FromIndex);
                // Last Button
                tempIndex = ((RecordCount - 1) / PageSize) * PageSize + 1;
                LastButton.Start = tempIndex;
                LastButton.Enabled = (tempIndex != FromIndex);
            }
        }

        // Render
        public override HtmlString Render()
        {
            string html = "", href = AppPath(CurrentPageName());
            if (RecordCount > 0 && Visible) {
                string url = !Empty(Url) ? Url : CurrentDashboardPageUrl();
                string ajax = Table.UseAjaxActions ? "true" : "false";
                string action = Table.UseAjaxActions || Modal ? "refresh" : "redirect";
                string context = !Empty(ContextClass) ? " data-context=\"" + HtmlEncode(ContextClass) + "\"" : "";
                string firstBtn = $@"<button class=""btn btn-default{FirstButton.DisabledClass}"" data-value=""first"" data-table=""{Table.TableVar}"" title=""{Language.Phrase("PagerFirst")}"" {FirstButton.GetAttributes(url, action)}><i class=""fa-solid fa-angles-left ew-icon""></i></button>";
                string prevBtn = $@"<button class=""btn btn-default{PrevButton.DisabledClass}"" data-value=""prev"" data-table=""{Table.TableVar}"" title=""{Language.Phrase("PagerPrevious")}"" {PrevButton.GetAttributes(url, action)}><i class=""fa-solid fa-angle-left ew-icon""></i></button>";
                string nextBtn = $@"<button class=""btn btn-default{NextButton.DisabledClass}"" data-value=""next"" data-table=""{Table.TableVar}"" title=""{Language.Phrase("PagerNext")}"" {NextButton.GetAttributes(url, action)}><i class=""fa-solid fa-angle-right ew-icon""></i></button>";
                string lastBtn = $@"<button class=""btn btn-default{LastButton.DisabledClass}"" data-value=""last"" data-table=""{Table.TableVar}"" title=""{Language.Phrase("PagerLast")}"" {LastButton.GetAttributes(url, action)}><i class=""fa-solid fa-angles-right ew-icon""></i></button>";
                string disabled = Modal ? " disabled" : "";
                string pagePhrase = Language.Phrase(PagePhraseId);
                string pageNumber = $@"<!-- current page number --><input class=""form-control ew-page-number"" type=""text"" data-ew-action=""change-page"" data-ajax=""{ajax}"" data-url=""{url}"" data-pagesize=""{PageSize}"" data-pagecount=""{PageCount}"" {context} name=""{PageNumberName}"" value=""{FormatInteger(CurrentPageNumber)}""{disabled}>";
                html += $@"<div class=""ew-pager"">
<span>{pagePhrase}&nbsp;</span>
<div class=""ew-prev-next"">
    <div class=""input-group input-group-sm"">
        <!-- first page button -->
        {firstBtn}
        <!-- previous page button -->
        {prevBtn}
        {pageNumber}
        <!-- next page button -->
        {nextBtn}
        <!-- last page button -->
        {lastBtn}
    </div>
</div>
<span>&nbsp;{Language.Phrase("Of")}&nbsp;{FormatInteger(PageCount)}</span>
</div>";
            }
            return new HtmlString(html + base.Render().Value);
        }

        // Get start attribute
        private string GetStartAttribute(int start)
        {
            if (Modal)
                return $@" data-start=""{start}""";
            else
                return $@" href=""{AppPath(CurrentPageName())}?start={start}""";
        }
    }
} // End Partial class
