namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Menu item class
    /// </summary>
    public class MenuItem
    {
        public int Id;

        public string Name = "";

        public string Text = "";

        public string Url = "";

        public int ParentId = -1;

        public MenuBase? Submenu = null;

        public bool Allowed = true;

        public string Target = "";

        public bool IsHeader = false;

        public bool IsCustomUrl = false;

        public string Href = "";

        public string Icon = "";

        public Attributes Attributes = new ();

        public string Label = ""; // HTML (for vertical menu only)

        public bool IsNavbarItem = false;

        public bool IsSidebarItem = false;

        public int Level = 0;

        public MenuBase Menu;

        private bool? _active = null;

        // Constructor
        public MenuItem(MenuBase menu, int id, string name, string text, string url, int parentId = -1, bool allowed = true, bool isHeader = false, bool isCustomUrl = false, string icon = "", string label = "", bool isNavbarItem = false, bool isSidebarItem = false)
        {
            Menu = menu;
            Id = id;
            Name = name;
            Text = text;
            Url = !Empty(url) ? AppPath(url) : "";
            ParentId = parentId;
            Allowed = allowed;
            IsHeader = isHeader;
            IsCustomUrl = isCustomUrl;
            Icon = icon;
            Label = label;
            IsNavbarItem = isNavbarItem;
            IsSidebarItem = isSidebarItem;
        }

        // Can render
        public bool CanRender => Allowed && !Empty(Url) || (Submenu?.RenderMenu ?? false);

        // Is opened
        public bool IsOpened => Active || (Submenu?.IsOpened ?? false);

        // Is active // DN
        public bool Active
        {
            get {
                _active ??= IsCustomUrl
                    ? SameText(CurrentUrl(), Url)
                    : SameText(CurrentPageName(), GetPageName(Url));
                return _active.Value;
            }
        }

        // Add submenu item
        public void AddItem(MenuItem item)
        {
            Submenu ??= new (Id);
            Submenu.Level = Level + 1;
            Submenu.AddItem(item);
        }

        // Set attribute
        public void SetAttribute(string name, string value) => Attributes.Add(name, value);

        // Render
        public async Task<string> ToJson(bool deep = true)
        {
            if (Active || Submenu != null && Url != "#" && Menu.IsNavbar && Menu.IsRoot) { // Active or navbar root menu item with submenu // DN
                SetAttribute("data-url", Url); // Does not support URL for root menu item with submenu
                SetAttribute("data-ew-action", "none");
            }
            Url = !Empty(Url) ? GetUrl(Url) : ""; // DN
            if (IsMobile() && !IsCustomUrl && Url != "#")
                Url = Url.Replace("#", (Url.Contains("?") ? "&" : "?") + "hash=");
            if (Empty(Url))
                SetAttribute("data-ew-action", "none");
            if (!Empty(Icon)) {
                var list = ClassList(Icon);
            var faClassNames = new[] { "fa-solid", "fa-regular", "fa-light", "fa-thin", "fa-duotone", "fa-sharp", "fa-brands" };
            if (list.Any(cls => cls.StartsWith("fa-") && !list.Any(cls => faClassNames.Contains(cls))))
                list.Add("fa-solid");
                Icon = String.Join(" ", list);
            }
            bool hasItems = deep && Submenu != null;
            bool isOpened = hasItems && (Submenu?.IsOpened ?? false);
            string classNames = "";
            if (IsNavbarItem) {
                classNames = AppendClass(classNames, SameString(ParentId, "-1") || IsSidebarItem ? "nav-link" : "dropdown-item");
                if (Active)
                    classNames = AppendClass(classNames, "active");
                if (hasItems && !IsSidebarItem)
                    classNames = AppendClass(classNames, "dropdown-toggle ew-dropdown");
            } else {
                classNames = AppendClass(classNames, "nav-link");
                if (Active || isOpened)
                    classNames = AppendClass(classNames, "active");
            }
            Attributes.PrependClass(classNames); // Prepend classes
            string attrs = Attributes.ToString();
            var sw = new StringWriter();
            using var writer = new JsonTextWriter(sw);
            await writer.WriteStartObjectAsync();
            await writer.WritePropertyNameAsync("id");
            await writer.WriteValueAsync(Id);
            await writer.WritePropertyNameAsync("name");
            await writer.WriteValueAsync(Name);
            await writer.WritePropertyNameAsync("text");
            await writer.WriteValueAsync(Text);
            await writer.WritePropertyNameAsync("parentId");
            await writer.WriteValueAsync(ParentId);
            await writer.WritePropertyNameAsync("level");
            await writer.WriteValueAsync(Level);
            await writer.WritePropertyNameAsync("target");
            await writer.WriteValueAsync(Target);
            await writer.WritePropertyNameAsync("isHeader");
            await writer.WriteValueAsync(IsHeader);
            await writer.WritePropertyNameAsync("href");
            await writer.WriteValueAsync(Url);
            await writer.WritePropertyNameAsync("active");
            await writer.WriteValueAsync(Active);
            await writer.WritePropertyNameAsync("icon");
            await writer.WriteValueAsync(Icon);
            await writer.WritePropertyNameAsync("attrs");
            await writer.WriteValueAsync(attrs);
            await writer.WritePropertyNameAsync("label");
            await writer.WriteValueAsync(Label);
            await writer.WritePropertyNameAsync("isNavbarItem");
            await writer.WriteValueAsync(IsNavbarItem);
            await writer.WritePropertyNameAsync("open");
            await writer.WriteValueAsync((deep && Submenu != null) ? Submenu.IsOpened : false);
            await writer.WritePropertyNameAsync("items");
            if (deep && Submenu != null)
                await writer.WriteRawValueAsync(await Submenu.ToJson());
            else
                await writer.WriteNullAsync();
            await writer.WriteEndObjectAsync();
            return sw.ToString();
        }
    }
} // End Partial class
