namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// XML document class
    /// </summary>
    public class XmlDoc : XmlDocument
    {
        public string RootTagName = "table";

        public string DetailTagName = "";

        public string RowTagName = "row";

        public XmlElement? Table;

        public XmlElement? SubTable;

        public XmlElement? Row;

        // Get XML tag name
        public string XmlTagName(string name)
        {
            name = name.Replace(" ", "_");
            if (!Regex.IsMatch(name, @"^(?!XML)[a-z][\w-]*$", RegexOptions.IgnoreCase))
                name = "_" + name;
            return name;
        }

        // Add root
        public void AddRoot(string rootname)
        {
            RootTagName = XmlTagName(rootname);
            Table = CreateElement(RootTagName);
            AppendChild(Table);
        }

        // Add row
        public void AddRow(string tagName = "", string rowname = "")
        {
            if (!Empty(rowname))
                RowTagName = XmlTagName(rowname);
            Row = CreateElement(RowTagName);
            if (Empty(tagName)) {
                Table?.AppendChild(Row);
            } else {
                if (Empty(DetailTagName) || !SameString(DetailTagName, tagName)) {
                    DetailTagName = XmlTagName(tagName);
                    SubTable = CreateElement(DetailTagName);
                    Table?.AppendChild(SubTable);
                }
                SubTable?.AppendChild(Row);
            }
        }

        // Add field
        public void AddField(string name, object value)
        {
            var field = CreateElement(XmlTagName(name));
            field.AppendChild(CreateTextNode(ConvertToString(value)));
            Row?.AppendChild(field);
        }
    }
} // End Partial class
