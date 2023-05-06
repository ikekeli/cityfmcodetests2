namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// List option class
    /// </summary>
    public class ListOption
    {
        public string Name = "";

        public bool OnLeft = false;

        public string CssStyle = "";

        public string CssClass = "";

        public bool Visible = true;

        public string Header = "";

        public string Body = "";

        public string Footer = "";

        public ListOptions Parent;

        public bool ShowInButtonGroup = true;

        public bool ShowInDropDown = true;

        public string ButtonGroupName = "_default";

        // Constructor
        public ListOption(string name, ListOptions parent)
        {
            Name = name;
            Parent = parent;
        }

        // Add a link
        public void AddLink(Attributes attrs, string phraseId) => Body += GetLinkHtml(attrs, phraseId);

        // Set body
        public void SetVisible(bool value) => Visible = value;

        // Set body
        public void SetBody(string value) => Body = value;

        // Add to body
        public void AddBody(string value) => Body += value;

        // Clear
        public void Clear() => Body = "";

        // Move
        public void MoveTo(int pos) => Parent.MoveItem(Name, pos);

        // Render
        public string Render(string part, int colspan, string pos)
        {
            string tagclass = Parent.TagClassName, value = part;
            bool td = SameText(Parent.Tag, "td");
            if (part == "header") {
                tagclass = Empty(tagclass) ? "ew-list-option-header" : tagclass;
                value = Header;
            } else if (part == "body") {
                tagclass = Empty(tagclass) ? "ew-list-option-body" : tagclass;
                value = Body;
            } else if (part == "footer") {
                tagclass = Empty(tagclass) ? "ew-list-option-footer" : tagclass;
                value = Footer;
            }
            if (Empty(value) && Regex.IsMatch(Parent.TagClassName, "inline") && Empty(Parent.TemplateId)) // Skip for multi-column inline tag
                return "";
            var res = value;
            tagclass = AppendClass(tagclass, CssClass);
            var attrs = new Attributes(new { @class = tagclass, style = CssStyle, data_name = Name }); // Note: data_name => "data-name" // DN
            attrs.AppendClass(CssClass);
            if (td && (Name == Parent.GroupOptionName || Name == "checkbox")) // "button" and "checkbox" columns
                attrs.AppendClass("w-1");
            if (td && Parent.RowSpan > 1)
                attrs["rowspan"] = ConvertToString(Parent.RowSpan);
            if (td && colspan > 1)
                attrs["colspan"] = ConvertToString(colspan);
            var name = Parent.TableVar + "_" + Name;
            if (Name != Parent.GroupOptionName) {
                if (!(new[] {"checkbox", "rowcnt"}).Contains(Name)) {
                    if (Parent.UseButtonGroup && ShowInButtonGroup) {
                        res = Parent.RenderButtonGroup(res, pos);
                        if (OnLeft && td && colspan > 1)
                            res = "<div class=\"text-end\">" + res + "</div>";
                    }
                }
                if (part == "header")
                    res = "<span id=\"elh_" + name + "\" class=\"" + name + "\">" + res + "</span>";
                else if (part == "body")
                    res = "<span id=\"el" + Parent.RowCount + "_" + name + "\" class=\"" + name + "\">" + res + "</span>";
                else if (part == "footer")
                    res = "<span id=\"elf_" + name + "\" class=\"" + name + "\">" + res + "</span>";
            }
            var tag = (td && part == "header") ? "th" : Parent.Tag;
            if (Parent.UseButtonGroup && ShowInButtonGroup)
                attrs.AppendClass("text-nowrap");
            res = !Empty(tag) ? HtmlElement(tag, attrs, res) : res;
            if (!Empty(Parent.TemplateId) && Parent.TemplateType == "single") {
                if (part == "header")
                    res = "<template id=\"tpoh_" + Parent.TemplateId + "_" + Name + "\">" + res + "</template>";
                else if (part == "body")
                    res = "<template id=\"tpob" + Parent.RowCount + "_" + Parent.TemplateId + "_" + Name + "\">" + res + "</template>";
                else if (part == "footer")
                    res = "<template id=\"tpof_" + Parent.TemplateId + "_" + Name + "\">" + res + "</template>";
            }
            return res;
        }
    }
} // End Partial class
