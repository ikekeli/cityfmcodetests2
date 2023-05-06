namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Basic Search class
    /// </summary>
    public class BasicSearch
    {
        public DbTableBase Table;

        public bool BasicSearchAnyFields = Config.BasicSearchAnyFields;

        public string KeywordDefault = "";

        public string TypeDefault = "";

        public bool Raw = false;

        private string _prefix = "";

        private string _keyword = "";

        private string _type = "";

        // Constructor
        public BasicSearch(DbTableBase table)
        {
            Table = table;
            _prefix = Config.ProjectName + "_" + Table.TableVar + "_";
            Raw = !Config.RemoveXss;
        }

        // Session variable name
        protected string GetSessionName(string suffix) => _prefix + suffix;

        // Load default
        public void LoadDefault()
        {
            _keyword = KeywordDefault;
            _type = TypeDefault;
            if (Session[GetSessionName(Config.TableBasicSearchType)] == null && !Empty(TypeDefault)) // Save default to session
                SessionType = TypeDefault;
        }

        // Unset session
        public void UnsetSession()
        {
            Session.Remove(GetSessionName(Config.TableBasicSearchType));
            Session.Remove(GetSessionName(Config.TableBasicSearch));
        }

        // Isset session
        public bool IssetSession => Session[GetSessionName(Config.TableBasicSearch)] != null;

        // Session Keyword
        public string SessionKeyword
        {
            get => ConvertToString(Session[GetSessionName(Config.TableBasicSearch)]);
            set {
                _keyword = value;
                Session[GetSessionName(Config.TableBasicSearch)] = value;
            }
        }

        // Set Keyword
        public void SetKeyword(string value, bool save = true)
        {
            value = Raw ? value : RemoveXss(value);
            _keyword = value;
            if (save)
                SessionKeyword = value;
        }

        // Keyword
        public string Keyword
        {
            get => _keyword;
            set => SetKeyword(value, false);
        }

        // Type
        public string SessionType
        {
            get => ConvertToString(Session[GetSessionName(Config.TableBasicSearchType)]);
            set {
                _type = value;
                Session[GetSessionName(Config.TableBasicSearchType)] = value;
            }
        }

        // Set type
        public void SetType(string value, bool save = true)
        {
            value = Raw ? value : RemoveXss(value);
            _type = value;
            if (save)
                SessionType = value;
        }

        // Type
        public string Type
        {
            get => _type;
            set => SetType(value, false);
        }

        // Get type name
        public string TypeName =>
            SessionType switch {
                "=" => Language.Phrase("QuickSearchExact"),
                "AND" => Language.Phrase("QuickSearchAll"),
                "OR" => Language.Phrase("QuickSearchAny"),
                _ => Language.Phrase("QuickSearchAuto")
            };

        // Get short type name
        public string TypeNameShort
        {
            get {
                string typname = SessionType switch {
                    "=" => Language.Phrase("QuickSearchExactShort"),
                    "AND" => Language.Phrase("QuickSearchAllShort"),
                    "OR" => Language.Phrase("QuickSearchAnyShort"),
                    _ => Language.Phrase("QuickSearchAutoShort")
                };
                if (!Empty(typname))
                    typname += "&nbsp;";
                return typname;
            }
        }

        // Get keyword list
        public List<string> KeywordList(bool def = false)
        {
            string searchKeyword = def ? KeywordDefault : Keyword;
            string searchType = def ? TypeDefault : Type;
            List<string> list = new ();
            if (!Empty(searchKeyword)) {
                string search = searchKeyword.Trim();
                list = GetQuickSearchKeywords(search, searchType);
            }
            return list;
        }

        // save
        public void Save()
        {
            SessionKeyword = _keyword;
            SessionType = _type;
        }

        // Load
        public void Load()
        {
            _keyword = SessionKeyword;
            _type = SessionType;
        }

        // Convert to JSON
        public string ToJson()
        {
            var d = new Dictionary<string, string>();
            if (!Empty(_keyword)) {
                d.Add(Config.TableBasicSearch, _keyword);
                d.Add(Config.TableBasicSearchType, _type);
            }
            return ConvertToJson(d);
        }
    }
} // End Partial class
