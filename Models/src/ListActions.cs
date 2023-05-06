namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// List actions class
    /// </summary>
    public class ListActions
    {
        public Dictionary<string, ListAction> Items = new ();

        // Add and return a new action
        public ListAction Add(string name, ListAction action)
        {
            Items[name] = action;
            return action;
        }

        // Add and return a new action
        public ListAction Add(string name, string caption, bool allow = true, string method = Config.ActionPostback, string select = Config.ActionMultiple, string confirmMsg = "", string icon = "", string success = "")
        {
            var item = new ListAction(name, caption, allow, method, select, confirmMsg, icon, success);
            Items[name] = item;
            return item;
        }

        // Add multiple actions by name and caption
        public void Add(Dictionary<string, dynamic> actions)
        {
            foreach (var (key, action) in actions)
                Add(key, action);
        }

        // Get item by name
        public ListAction? GetItem(string name) => Items.TryGetValue(name, out ListAction? action) ? action : null;

        // Indexer
        public ListAction? this[string index] => GetItem(index);
    }
} // End Partial class
