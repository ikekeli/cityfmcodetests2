namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Menu class
    /// </summary>
    public class Menu : MenuBase
    {
        // Constructor
        public Menu(object menuId, bool isRoot = false, bool isNavbar = false, string? languageFolder = null) : base(menuId, isRoot, isNavbar, languageFolder)
        {
        }

        // Render
        public override async Task<string> ToJson()
        {
            MenuRendering();
            return await base.ToJson();
        }
    }
} // End Partial class
