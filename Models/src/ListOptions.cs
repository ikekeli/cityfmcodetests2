namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// List Options class
    /// </summary>
    public class ListOptions
    {
        public List<ListOption> Items = new ();

        public string CustomItem = "";

        public string Tag = "div";

        public string TagClassName = "";

        public string TableVar = "";

        public string RowCount = "";

        public string TemplateType = "block";

        public string TemplateId = "";

        public string TemplateClassName = "";

        public int RowSpan = 1;

        public bool UseDropDownButton = false;

        public bool UseButtonGroup = false;

        public string ButtonClass = "";

        public string ButtonGroupClass = "";

        public string GroupOptionName = "button";

        public string DropDownButtonPhrase = "";

        public string DropDownAutoClose = "true"; // true/inside/outside/false (see https://getbootstrap.com/docs/5.2/components/dropdowns/#auto-close-behavior)

        // Check visible
        public bool Visible => Items.Any(item => item.Visible);

        // Check group option visible
        public bool GroupOptionVisible
        {
            get {
                int cnt = 0;
                foreach (var item in Items) {
                    if (item.Name != GroupOptionName &&
                        (item.Visible && item.ShowInDropDown && UseDropDownButton ||
                        item.Visible && item.ShowInButtonGroup && UseButtonGroup)) {
                        cnt++;
                        if (UseDropDownButton && cnt > 1 || UseButtonGroup)
                            return true;
                    }
                }
                return false;
            }
        }

        // Add and return a new option
        public ListOption Add(string name)
        {
            ListOption item = new (name, this);
            Items.Add(item);
            return item;
        }

        // Add group option and return the new option
        public ListOption AddGroupOption() => Add(GroupOptionName);

        // Load default settings
        public void LoadDefault()
        {
            CustomItem = "";
            Items.ForEach(item => item.Body = "");
        }

        // Hide all options
        public void HideAllOptions(List<string>? list = null) => Items.ForEach(item => {
                if (list == null || !list.Contains(item.Name))
                    item.Visible = false;
            });

        // Show all options
        public void ShowAllOptions() => Items.ForEach(item => item.Visible = true);

        /// <summary>
        /// Get item by name
        /// </summary>
        /// <param name="name">Name of item. Predefined names: view/edit/copy/delete/detail_DetailTable/userpermission/checkbox</param>
        /// <returns>ListOption</returns>
        public ListOption? GetItem(string name) => Items.Find(item => item.Name == name);

        // Get item by name
        public ListOption? this[string name] => GetItem(name);

        // Get item by index
        public ListOption this[int index] => Items[index];

        // Get item index by name (predefined names: view/edit/copy/delete/detail_<DetailTable>/userpermission/checkbox)
        public int GetItemIndex(string name) => Items.FindIndex(item => item.Name == name);

        // Items count
        public int Count => Items.Count;

        // Visible items count
        public int VisibleCount => Items.Where(item => item.Visible).Count();

        // Move item to position
        public void MoveItem(string name, int pos)
        {
            int newpos = pos;
            if (newpos < 0) // If negative, count from the end
                newpos = Items.Count + newpos;
            if (newpos < 0) {
                newpos = 0;
            } else if (newpos > Items.Count) {
                newpos = Items.Count;
            }
            var curitem = GetItem(name);
            int curpos = GetItemIndex(name);
            if (curitem != null && curpos > -1 && newpos != curpos) {
                Items.RemoveAt(curpos); // Remove old item
                if (curpos < newpos)
                    newpos--; // Adjust new position
                Items.Insert(newpos, curitem); // Insert new item
            }
        }

        // Render list options
        public void Render(string part, string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "") =>
            Write(RenderHtml(part, pos, rowCnt, templateType, templateId, templateClass));

        // Render list options body
        public IHtmlContent RenderBody(string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "") =>
            new HtmlString(RenderHtml("body", pos, rowCnt, templateType, templateId, templateClass));

        // Render list options header
        public IHtmlContent RenderHeader(string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "") =>
            new HtmlString(RenderHtml("header", pos, rowCnt, templateType, templateId, templateClass));

        // Render list options footer
        public IHtmlContent RenderFooter(string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "") =>
            new HtmlString(RenderHtml("footer", pos, rowCnt, templateType, templateId, templateClass));

        // Render list options as HTML
        public string RenderHtml(string part, string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "")
        {
            var groupitem = GetItem(GroupOptionName);
            if (Empty(CustomItem) && groupitem != null && ShowPosition(groupitem.OnLeft, pos)) {
                bool useDropDownButton = UseDropDownButton;
                bool useButtonGroup = UseButtonGroup;
                if (useDropDownButton) { // Render dropdown
                    var buttonValue = "";
                    int cnt = 0;
                    foreach (var item in Items) {
                        if (item.Name != GroupOptionName && item.Visible) {
                            if (item.ShowInDropDown) {
                                buttonValue += item.Body;
                                cnt++;
                            } else if (item.Name == "listactions") { // Show listactions as button group
                                item.Body = RenderButtonGroup(item.Body, pos);
                            }
                        }
                    }
                    if (cnt < 1 || cnt == 1 && !buttonValue.Contains("dropdown-menu")) { // No item to show in dropdown or only one item without dropdown menu
                        useDropDownButton = false; // No need to use drop down button
                    } else {
                        string dropdownButtonClass = !TagClassName.Contains("ew-multi-column-list-option-card") ? "btn-default" : "";
                        dropdownButtonClass = AppendClass(dropdownButtonClass, "btn dropdown-toggle");
                        groupitem.Body = RenderDropDownButton(buttonValue, pos, dropdownButtonClass);
                        groupitem.Visible = true;
                    }
                }
                if (!useDropDownButton) {
                    if (useButtonGroup) { // Render button group
                        var visible = false;
                        var buttonGroups = new Dictionary<string, string>();
                        foreach (var item in Items) {
                            if (item.Name != GroupOptionName && item.Visible && !Empty(item.Body)) {
                                if (item.ShowInButtonGroup) {
                                    visible = true;
                                    var buttonValue = item.Body;
                                    if (!buttonGroups.ContainsKey(item.ButtonGroupName))
                                        buttonGroups[item.ButtonGroupName] = "";
                                    buttonGroups[item.ButtonGroupName] += buttonValue;
                                } else if (item.Name == "listactions") { // Show listactions as button group
                                    item.Body = RenderButtonGroup(item.Body, pos);
                                }
                            }
                        }
                        groupitem.Body = "";
                        foreach (var (key, buttonGroup) in buttonGroups)
                            groupitem.Body += RenderButtonGroup(buttonGroup, pos);
                        if (visible)
                            groupitem.Visible = true;
                    } else { // Render links as button links
                        foreach (var item in Items) {
                            if ((new [] { "view", "edit", "copy", "delete" }).Contains(item.Name)) // Show actions as button links
                                item.Body = RenderButtonLinks(item.Body);
                        }
                    }
                }
            }
            if (!Empty(templateId)) {
                if (pos == "right" || pos.StartsWith("bottom", StringComparison.InvariantCultureIgnoreCase)) { // Show all options script tags on the right/bottom (ignore left to avoid duplicate)
                    return Output(part, "", rowCnt, "block", templateId, templateClass) + // Original block for ew.showTemplates
                        Output(part, "", rowCnt, "inline", templateId) +
                        Output(part, "", rowCnt, "single", templateId);
                }
            }
            return Output(part, pos, rowCnt, templateType, templateId, templateClass);
        }

        // Get custom template script tag
        protected string CustomTemplateTag(string templateId, string templateType, string templateClass, string rowCnt = "") {
            var id = "_" + templateId;
            if (!Empty(rowCnt))
                id = rowCnt + id;
            id = "tp" + templateType + id;
            return "<template id=\"" + id + "\"" + (!Empty(templateClass) ? " class=\"" + templateClass + "\"" : "") + ">";
        }

        // Output list options
        protected string Output(string part, string pos = "", object? rowCnt = null, string templateType = "block", string templateId = "", string templateClass = "") {
            RowCount = ConvertToString(rowCnt);
            TemplateType = templateType;
            TemplateId = templateId;
            TemplateClassName = templateClass;
            string result = "";
            string tag = Tag; // Save Tag
            if (!Empty(templateId)) {
                if (templateType != "block")
                    TagClassName = AppendClass(TagClassName, "d-inline-block");
                else
                    TagClassName = RemoveClass(TagClassName, "d-inline-block");
                if (templateType == "block") {
                    if (part == "header")
                        result += CustomTemplateTag(templateId, "oh", templateClass);
                    else if (part == "body")
                        result += CustomTemplateTag(templateId, "ob", templateClass, RowCount);
                    else if (part == "footer")
                        result += CustomTemplateTag(templateId, "of", templateClass);
                } else if (templateType == "inline") {
                    Tag = "div"; // Use div
                    if (part == "header")
                        result += CustomTemplateTag(templateId, "o2h", templateClass);
                    else if (part == "body")
                        result += CustomTemplateTag(templateId, "o2b", templateClass, RowCount);
                    else if (part == "footer")
                        result += CustomTemplateTag(templateId, "o2f", templateClass);
                }
            } else {
                if (Empty(pos) || pos.StartsWith("top", StringComparison.InvariantCultureIgnoreCase) || pos.StartsWith("bottom", StringComparison.InvariantCultureIgnoreCase) || templateType != "block") // Use inline tag for multi-column
                    TagClassName = AppendClass(TagClassName, "d-inline-block");
            }
            if (!Empty(CustomItem)) {
                ListOption? opt = null;
                int cnt = 0;
                foreach (var item in Items) {
                    if (ShowItem(item, templateId, pos))
                        cnt++;
                    if (item.Name == CustomItem)
                        opt = item;
                }
                var useButtonGroup = UseButtonGroup; // Backup options
                UseButtonGroup = true; // Show button group for custom item
                if (opt != null && cnt > 0) {
                    if (!Empty(templateId) || ShowPosition(opt.OnLeft, pos)) {
                        result += opt.Render(part, cnt, pos);
                    } else {
                        result += opt.Render("", cnt, pos);
                    }
                }
                UseButtonGroup = useButtonGroup; // Restore options
            } else {
                foreach (var item in Items) {
                    if (ShowItem(item, templateId, pos))
                        result += item.Render(part, 1, pos);
                }
            }
            if ((templateType == "block" || templateType == "inline") && !Empty(templateId)) {
                result += "</template>";
                Tag = tag; // Restore Tag
            }
            return result;
        }

        // Show item
        private bool ShowItem(ListOption item, string templateId, string pos)
        {
            bool show = item.Visible && (!Empty(templateId) || ShowPosition(item.OnLeft, pos));
            if (show) {
                bool groupItemVisible = GetItem(GroupOptionName)?.Visible ?? false;
                if (UseDropDownButton) // Group item / Item not in dropdown / Item in dropdown + Group item not visible
                    show = item.Name == GroupOptionName && groupItemVisible || !item.ShowInDropDown || item.ShowInDropDown && !groupItemVisible;
                else if (UseButtonGroup)
                    show = item.Name == GroupOptionName && groupItemVisible || !item.ShowInButtonGroup || item.ShowInButtonGroup && !groupItemVisible;
            }
            return show;
        }

        // Show position
        private bool ShowPosition(bool onLeft, string pos) => onLeft && pos == "left" || !onLeft && pos == "right" || pos == "" || pos.StartsWith("top", StringComparison.InvariantCultureIgnoreCase) || pos.StartsWith("bottom", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Concat options
        /// </summary>
        /// <param name="pattern">Regular expression pattern for matching the option names, e.g. '/^detail_/'</param>
        /// <param name="separator">Separator</param>
        /// <returns>The concatenated HTML</returns>
        public string Concat(string pattern, string separator = "") => String.Join(separator, Items.Where(item => Regex.IsMatch(item.Name, pattern) && !Empty(item.Body)));

        /// <summary>
        /// Merge options to the first option
        /// </summary>
        /// <param name="pattern">Regular expression pattern for matching the option names, e.g. '/^detail_/'</param>
        /// <param name="separator">Separator</param>
        /// <returns>The first option</returns>
        public ListOption? Merge(string pattern, string separator = "")
        {
            var items = Items.Where(item => Regex.IsMatch(item.Name, pattern));
            var first = items.FirstOrDefault();
            first?.SetBody(Concat(pattern, separator));
            items.Skip(1).ToList().ForEach(item => item.Visible = false);
            return first;
        }

        /// <summary>
        /// Get button links
        /// </summary>
        /// <param name="body">Body</param>
        /// <returns>Button links</returns>
        public string RenderButtonLinks(string body)
        {
            if (Empty(body))
                return body;
            using var doc = new AngleSharp.Html.Parser.HtmlParser().ParseDocument(body);

            // Get and remove <input type="hidden"> and <div class="btn-group">
            string html = "";
            foreach (var el in doc.QuerySelectorAll("div.btn-group, input[type=hidden]")) {
                html += el.OuterHtml;
                el.Remove();
            }

            // Get <a> and <button>
            string btnClass = ButtonClass;
            string links = "";
            foreach (var button in doc.QuerySelectorAll("a, button")) {
                string classes = button.GetAttribute("class") ?? "";
                classes = PrependClass(classes, "btn btn-xs btn-link");
                button.SetAttribute("class", AppendClass(classes, btnClass)); // Add button classes
                string link = button.OuterHtml;
                if (SameText(button.TagName, "a")) {
                    string action = button.GetAttribute("data-ew-action") ?? "";
                    string href = button.GetAttribute("href") ?? "";
                    if (Empty(action) && !Empty(href)) {
                        button.SetAttribute("data-ew-action", "redirect");
                        button.SetAttribute("data-url", href);
                        button.RemoveAttribute("href");
                        link = Regex.Replace(Regex.Replace(button.OuterHtml, @"^<a\s*", "<button "), @"</a>", "</button>"); // Change link to button
                    }
                }
                links += link;
            }
            return links + html;
        }

        // Get button group link
        public string RenderButtonGroup(string body, string pos)
        {
            if (Empty(body))
                return body;
            using var doc = new AngleSharp.Html.Parser.HtmlParser().ParseDocument(body);

            // Get and remove <input type="hidden"> and <div class="btn-group">
            string html = "";
            foreach (var el in doc.QuerySelectorAll("div.btn-group, input[type=hidden]")) {
                html += el.OuterHtml;
                el.Remove();
            }

            // Get <a> and <button>
            string btnClass = ButtonClass;
            string links = "";
            foreach (var button in doc.QuerySelectorAll("a, button")) {
                string className = button.GetAttribute("class") ?? "";
                className = PrependClass(className, "btn btn-default");
                button.SetAttribute("class", AppendClass(className, btnClass)); // Add button classes
                links += button.OuterHtml;
            }
            string btngroupClass = "btn-group btn-group-sm ew-btn-group ew-list-options" + (pos.StartsWith("bottom", StringComparison.InvariantCultureIgnoreCase) ? " dropup" : "");
            string btngroup = !Empty(links) ? "<div class=\"" + btngroupClass + "\">" + links + "</div>" : "";
            return btngroup + html;
        }

        // Render drop down button
        public string RenderDropDownButton(string body, string pos, string dropdownButtonClass)
        {
            if (Empty(body))
                return body;
            using var doc = new AngleSharp.Html.Parser.HtmlParser().ParseDocument(body);

            // Get and remove <div class="d-none"> and <input type="hidden">
            string html = "";
            foreach (var el in doc.QuerySelectorAll("div.d-none, input[type=hidden]")) {
                html += el.OuterHtml;
                el.Remove();
            }

            // Get <a> and <button> without data-bs-toggle attribute
            string links = "";
            bool submenu = false;
            string submenulink = "";
            string submenulinks = "";
            foreach (var button in doc.QuerySelectorAll("a:not([data-bs-toggle]), button:not([data-bs-toggle])")) {
                var action = button.GetAttribute("data-action");
                string classes = button.GetAttribute("class") ?? "";
                if (!Regex.IsMatch(classes, @"\bdropdown-item\b")) { // Skip if already dropdown-item
                    classes = Regex.Replace(classes, @"btn[\S]*\s+", "", RegexOptions.IgnoreCase); // Remove btn classes
                    string htmlTitle = button.GetAttribute("title") ?? "";
                    button.RemoveAttribute("title"); // Remove title
                    string caption = button.InnerHtml;
                    caption = !Empty(htmlTitle) && htmlTitle != caption ? htmlTitle : caption;
                    button.SetAttribute("class", AppendClass(classes, "dropdown-item"));
                    if (SameText(button.TagName, "a") && Empty(button.GetAttribute("href"))) // Add href for <a>
                        button.SetAttribute("href", "#");
                    var icon = button.QuerySelector("i.ew-icon"); // Icon classes contains 'ew-icon'
                    var badge = button.QuerySelector("span.badge");
                    if (badge == null) { // Skip span.badge
                        if (!Empty(caption) && !Empty(icon)) { // Has both caption and icon
                            classes = icon?.GetAttribute("class") ?? "";
                            icon?.SetAttribute("class", AppendClass(classes, "me-2")); // Add margin-right to icon
                        }
                        //var children = button.Children;
                        //foreach (var child in children)
                        //    child.Remove();
                        button.InnerHtml = ""; // Clear InnerHtml // DN
                        if (!Empty(icon))
                            button.AppendChild(icon);
                        if (!Empty(caption)) // Has caption
                            button.AppendChild(doc.CreateTextNode(caption));
                    }
                }
                string link = button.OuterHtml;
                if (action == "list") { // Start new submenu
                    if (submenu) { // End previous submenu
                        if (!Empty(submenulinks)) // Set up submenu
                            links += "<li class=\"dropdown-submenu dropdown-hover\">" + submenulink.Replace("dropdown-item", "dropdown-item dropdown-toggle") + "<ul class=\"dropdown-menu\">" + submenulinks + "</ul></li>";
                        else
                            links += "<li>" + submenulink + "</li>";
                    }
                    submenu = true;
                    submenulink = link;
                    submenulinks = "";
                } else {
                    if (Empty(action) && submenu) { // End previous submenu
                        if (!Empty(submenulinks)) // Set up submenu
                            links += "<li class=\"dropdown-submenu dropdown-hover\">" + submenulink + "<ul class=\"dropdown-menu\">" + submenulinks + "</ul></li>";
                        else
                            links += "<li>" + submenulink + "</li>";
                        submenu = false;
                    }
                    if (submenu)
                        submenulinks += "<li>" + link + "</li>";
                    else
                        links += "<li>" + link + "</li>";
                }
            }
            string btndropdown = "";
            if (!Empty(links)) {
                if (submenu) { // End previous submenu
                    if (!Empty(submenulinks)) // Set up submenu
                        links += "<li class=\"dropdown-submenu dropdown-hover\">" + submenulink + "<ul class=\"dropdown-menu\">" + submenulinks + "</ul></li>";
                    else
                        links += "<li>" + submenulink + "</li>";
                }
                string btnclass = dropdownButtonClass;
                btnclass = AppendClass(btnclass, ButtonClass);
                string btngrpclass = "btn-group btn-group-sm ew-btn-dropdown" + (pos.StartsWith("bottom", StringComparison.InvariantCultureIgnoreCase) ? " dropup" : "");
                btngrpclass = AppendClass(btngrpclass, ButtonGroupClass);
                string buttontitle = Language.Phrase(DropDownButtonPhrase, true);
                string button = "<button type=\"button\" class=\"" + btnclass + "\" data-title=\"" + buttontitle + "\" data-bs-toggle=\"dropdown\" data-bs-auto-close=\"" + DropDownAutoClose + "\">" + Language.Phrase(DropDownButtonPhrase) + "</button>" +
                    "<ul class=\"dropdown-menu " + ((pos == "right" || pos.EndsWith("end", StringComparison.InvariantCultureIgnoreCase)) ? "dropdown-menu-end " : "") + "ew-dropdown-menu ew-list-options\">" + links + "</ul>";
                btndropdown = "<div class=\"" + btngrpclass + "\" data-table=\"" + TableVar + "\">" + button + "</div>"; // Use dropup for bottom
            }
            return btndropdown + html;
        }

        // Hide detail items for dropdown
        public void HideDetailItemsForDropDown()
        {
            var showDetail = false;
            if (UseDropDownButton)
                showDetail = Items.Any(item => item.Name != GroupOptionName && item.Visible && item.ShowInDropDown && !item.Name.StartsWith("detail_"));
            if (!showDetail)
                HideDetailItems();
        }

        // Hide detail items
        public void HideDetailItems() => Items.Where(item => item.Name.StartsWith("detail_")).ToList().ForEach(item => item.Visible = false);

        // Detail items is visible
        public bool DetailVisible() => Items.Any(item => item.Name.StartsWith("detail_") && item.Visible);
    }
} // End Partial class
