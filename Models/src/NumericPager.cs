namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Class for Numeric pager
    /// </summary>
    public class NumericPager : Pager
    {
        public List<PagerItem> Items = new ();

        public int ButtonCount = 0;

        // Constructor
        public NumericPager(object table, int fromIndex, int pageSize, int recordCount, string pageSizes = "", int range = 10, bool? autoHidePager = null, bool? autoHidePageSizeSelector = null, bool? usePageSizeSelector = null) :
            base(table, fromIndex, pageSize, recordCount, pageSizes, range, autoHidePager, autoHidePageSizeSelector, usePageSizeSelector)
        {
            if (AutoHidePager && fromIndex == 1 && recordCount <= pageSize)
                Visible = false;
            FirstButton = new PagerItem(ContextClass, pageSize);
            PrevButton = new PagerItem(ContextClass, pageSize);
            NextButton = new PagerItem(ContextClass, pageSize);
            LastButton = new PagerItem(ContextClass, pageSize);
            Init();
        }

        // Init pager
        public void Init()
        {
            if (FromIndex > RecordCount)
                FromIndex = RecordCount;
            ToIndex = FromIndex + PageSize - 1;
            if (ToIndex > RecordCount)
                ToIndex = RecordCount;
            SetupNumericPager();

            // Update button count
            if (FirstButton.Enabled)
                ButtonCount++;
            if (PrevButton.Enabled)
                ButtonCount++;
            if (NextButton.Enabled)
                ButtonCount++;
            if (LastButton.Enabled)
                ButtonCount++;
        }

        // Add pager item
        private void AddPagerItem(int startIndex, string text, bool enabled) => Items.Add(new (ContextClass, PageSize, startIndex, text, enabled));

        // Setup pager items
        private void SetupNumericPager()
        {
            bool hasPrev, noNext;
            int dy2, dx2, y, x, dx1, dy1, ny, tempIndex;
            if (RecordCount > PageSize) {
                noNext = (RecordCount < (FromIndex + PageSize));
                hasPrev = (FromIndex > 1);
                // First Button
                tempIndex = 1;
                FirstButton.Start = tempIndex;
                FirstButton.Enabled = (FromIndex > tempIndex);
                // Prev Button
                tempIndex = FromIndex - PageSize;
                if (tempIndex < 1)
                    tempIndex = 1;
                PrevButton.Start = tempIndex;
                PrevButton.Enabled = hasPrev;
                // Page links
                if (hasPrev | !noNext) {
                    x = 1;
                    y = 1;
                    dx1 = ((FromIndex - 1) / (PageSize * Range)) * PageSize * Range + 1;
                    dy1 = ((FromIndex - 1) / (PageSize * Range)) * Range + 1;
                    if ((dx1 + PageSize * Range - 1) > RecordCount) {
                        dx2 = (RecordCount / PageSize) * PageSize + 1;
                        dy2 = (RecordCount / PageSize) + 1;
                    } else {
                        dx2 = dx1 + PageSize * Range - 1;
                        dy2 = dy1 + Range - 1;
                    }
                    while (x <= RecordCount) {
                        if (x >= dx1 & x <= dx2) {
                            AddPagerItem(x, ConvertToString(y), FromIndex != x);
                            x = x + PageSize;
                            y = y + 1;
                        }
                        else if (x >= (dx1 - PageSize * Range) & x <= (dx2 + PageSize * Range)) {
                            if (x + Range * PageSize < RecordCount) {
                                AddPagerItem(x, y + "-" + (y + Range - 1), true);
                            } else {
                                ny = (RecordCount - 1) / PageSize + 1;
                                if (ny == y) {
                                    AddPagerItem(x, ConvertToString(y), true);
                                } else {
                                    AddPagerItem(x, y + "-" + ny, true);
                                }
                            }
                            x = x + Range * PageSize;
                            y = y + Range;
                        } else {
                            x = x + Range * PageSize;
                            y = y + Range;
                        }
                    }
                }
                // Next Button
                NextButton.Start = FromIndex + PageSize;
                tempIndex = FromIndex + PageSize;
                NextButton.Start = tempIndex;
                NextButton.Enabled = !noNext;
                // Last Button
                tempIndex = ((RecordCount - 1) / PageSize) * PageSize + 1;
                LastButton.Start = tempIndex;
                LastButton.Enabled = (FromIndex < tempIndex);
            }
        }

        // Render
        public override HtmlString Render()
        {
            string html = "";
            string url = CurrentDashboardPageUrl();
            string action = Table.UseAjaxActions ? "refresh" : "redirect";
            if (RecordCount > 0 && Visible) {
                if (FirstButton.Enabled)
                    html += $@"<li class=""page-item{FirstButton.DisabledClass}""><a class=""page-link"" data-value=""first"" {FirstButton.GetAttributes(url, action)} aria-label=""{Language.Phrase("PagerFirst")}""><i class=""fa-solid fa-angles-left""></i></a></li>";
                if (PrevButton.Enabled)
                    html += $@"<li class=""page-item{PrevButton.DisabledClass}""><a class=""page-link"" data-value=""prev"" {PrevButton.GetAttributes(url, action)} aria-label=""{Language.Phrase("PagerPrevious")}""><i class=""fa-solid fa-angle-left""></i></a></li>";
                foreach (var PagerItem in Items)
                    html += $@"<li class=""page-item{PagerItem.ActiveClass}""><a class=""page-link"" {PagerItem.GetAttributes(url, action)}>{FormatInteger(PagerItem.Text)}</a></li>";
                if (NextButton.Enabled)
                    html += $@"<li class=""page-item{NextButton.DisabledClass}""><a class=""page-link"" data-value=""next"" {NextButton.GetAttributes(url, action)} aria-label=""{Language.Phrase("PagerNext")}""><i class=""fa-solid fa-angle-right""></i></a></li>";
                if (LastButton.Enabled)
                    html += $@"<li class=""page-item{LastButton.DisabledClass}""><a class=""page-link"" data-value=""last"" {LastButton.GetAttributes(url, action)} aria-label=""{Language.Phrase("PagerLast")}""><i class=""fa-solid fa-angles-right""></i></a></li>";
                html = $@"<div class=""ew-pager"">
<div class=""ew-numeric-page"">
    <ul class=""pagination pagination-sm"">
        {html}
    </ul>
</div>
</div>";
            }
            return new HtmlString(html + base.Render().Value);
        }
    }
} // End Partial class
