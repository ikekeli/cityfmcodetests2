namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Menu class
    /// </summary>
    public class MenuBase
    {
        public object Id;

        public bool IsRoot;

        public bool IsNavbar;

        public bool Accordion = true; // For sidebar menu only

        public bool Compact = false; // For sidebar menu only

        public List<MenuItem> Items = new ();

        public bool UseSubmenu;

        public static bool Cache = false;

        public int Level = 0;

        private bool? _renderMenu = null;

        private bool? _isOpened = null;

        // Constructor
        public MenuBase(object id, bool isRoot = false, bool isNavbar = false, string? languageFolder = null)
        {
            Id = id;
            IsRoot = isRoot;
            IsNavbar = isNavbar;
            if (isNavbar) {
                UseSubmenu = true;
                Accordion = false;
            }
            Language = ResolveLanguage();
        }

        // Add a menu item
        public void AddMenuItem(MenuItem item)
        {
            if (!MenuItemAdding(item))
                return;
            if (item.ParentId < 0)
                AddItem(item);
            else if (FindItem(item.ParentId, out MenuItem? parentMenu))
                parentMenu?.AddItem(item);
        }

        // Add a menu item
        public void AddMenuItem(int id, string name, string text, string url, int parentId = -1, string src = "", bool allowed = true, bool isHeader = false, bool isCustomUrl = false, string icon = "", string label = "", bool isNavbarItem = false, bool isSidebarItem = false) =>
            AddMenuItem(new MenuItem(this, id, name, text, url, parentId, allowed, isHeader, isCustomUrl, icon, label, isNavbarItem, isSidebarItem));

        // Add a menu item // DN
        public void AddMenuItem(object[] item) =>
            AddMenuItem(
                (int)item[0], // id
                (string)item[1], // name
                Language.MenuPhrase((string)item[2], "MenuText"), // text
                (string)item[3], // url
                (int)item[4], // parentId
                (string)item[5], // src
                item[6] is string s
                    ? (s.EndsWith("?") ? (IsLoggedIn() || AllowList(s.TrimEnd('?'))) : AllowList(s))
                    : (item[6] is bool b ? (b ? true : IsLoggedIn()) : false), // allowed
                (bool)item[7], // isHeader
                (bool)item[8], // isCustomUrl
                (string)item[9], // icon,
                (string)item[10], // label,
                (bool)item[11], // isNavbarItem
                (bool)item[12] // isSidebarItem
            );

        // Add menu items // DN
        public void AddMenuItems(List<object[]> items) => items.ForEach(item => AddMenuItem(item));

        // Add a menu item (for backward compatibility) // DN
        public void AddMenuItem(int id, string text, string url, int parentId = -1, string src = "", bool allowed = true, bool isHeader = false, bool isCustomUrl = false) =>
            AddMenuItem(id, "mi_" + ConvertToString(id), text, url, parentId, src, allowed, isHeader, isCustomUrl);

        public string Phrase(string menuId, string id) => Language.MenuPhrase(menuId, id);

        // Get menu item count
        public int Count => Items.Count;

        // Add item
        public void AddItem(MenuItem item)
        {
            item.Level = Level;
            item.Menu = this;
            Items.Add(item);
        }

        // Clear all menu items
        public void Clear() => Items.Clear();

        // Find item
        public bool FindItem(int id, out MenuItem? outitem)
        {
            outitem = null;
            foreach (var item in Items) {
                if (item.Id == id) {
                    outitem = item;
                    return true;
                } else if (item.Submenu != null) {
                    if (item.Submenu.FindItem(id, out outitem))
                        return true;
                }
            }
            return false;
        }

        // Find item by menu text
        public bool FindItemByText(string txt, out MenuItem? outitem)
        {
            outitem = null;
            foreach (var item in Items) {
                if (item.Text == txt) {
                    outitem = item;
                    return true;
                } else if (item.Submenu != null) {
                    if (item.Submenu.FindItemByText(txt, out outitem))
                        return true;
                }
            }
            return false;
        }

        // Move item to position
        public void MoveItem(string text, int pos)
        {
            int oldpos = 0;
            int newpos = pos;
            pos = pos < 0 ? 0 : pos > Items.Count ? Items.Count : pos;
            if (Items.Find(item => SameString(item.Text, text)) is MenuItem currentItem) {
                oldpos = Items.IndexOf(currentItem);
                if (pos != oldpos) {
                    Items.RemoveAt(oldpos); // Remove old item
                    if (oldpos < pos)
                        newpos--; // Adjust new position
                    Items.Insert(newpos, currentItem); // Insert new item
                }
            }
        }

        // Check if this menu should be rendered
        public bool RenderMenu
        {
            get {
                _renderMenu ??= Items.Any(item => item.CanRender);
                return _renderMenu.Value;
            }
        }

        // Check if this menu should be opened
        public bool IsOpened
        {
            get {
                _isOpened ??= Items.Any(item => item.IsOpened);
                return _isOpened.Value;
            }
        }

        // Render the menu as JSON // DN
        public virtual async Task<string> ToJson()
        {
            if (Cache && IsLoggedIn() && Session.TryGetValue("__menu__" + Id, out string? _json))
                return _json;
            if (!RenderMenu || Count == 0)
                return "null";
            // Write JSON
            var sw = new StringWriter();
            using var writer = new JsonTextWriter(sw);
            if (IsRoot) {
                await writer.WriteStartObjectAsync();
                await writer.WritePropertyNameAsync("accordion");
                await writer.WriteValueAsync(Accordion);
                await writer.WritePropertyNameAsync("compact");
                await writer.WriteValueAsync(Compact);
                await writer.WritePropertyNameAsync("items");
            }
            await writer.WriteStartArrayAsync();
            foreach (MenuItem item in Items) {
                if (item.CanRender) {
                    if (item.IsHeader && (!IsRoot || !UseSubmenu)) { // Group title (Header)
                        await writer.WriteRawValueAsync(await item.ToJson(false));
                        if (item.Submenu != null) {
                            foreach (MenuItem subitem in item.Submenu.Items) {
                                if (subitem.CanRender)
                                    await writer.WriteRawValueAsync(await subitem.ToJson());
                            }
                        }
                    } else {
                        await writer.WriteRawValueAsync(await item.ToJson());
                    }
                }
            }
            await writer.WriteEndArrayAsync();
            if (IsRoot)
                await writer.WriteEndObjectAsync();
            string json = sw.ToString();
            MenuRendered(ref json);
            if (Cache && IsLoggedIn())
                Session["__menu__" + Id] = json;
            return json;
        }

        // Clear cache
        public void ClearCache() => Session.Remove("__menu__" + Id);

        // Returns the menu as script tag
        public async Task<string> ToScript() => "<script>ew.vars." + Id + " = " + await ToJson() + ";</script>";

        // MenuItem Adding
        public virtual bool MenuItemAdding(MenuItem item) => true;

        // Menu Rendering
        public virtual void MenuRendering() {}

        // Menu Rendered
        public virtual void MenuRendered(ref string json) {}
    }
} // End Partial class
