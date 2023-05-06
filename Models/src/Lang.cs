namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Language class
    /// </summary>
    public class Lang
    {
        public string LanguageId;

        [AllowNull]
        public Dictionary<string, dynamic> Phrases;

        [AllowNull]
        public DotAccessData PhraseData; // DN

        public string LanguageFolder = Config.LanguageFolder; // DN

        public string Template = ""; // JsRender template

        public string Method = "prependTo"; // JsRender template method

        public string Target = ".navbar-nav.ms-auto"; // JsRender template target

        public string Type = "LI"; // LI/DROPDOWN (for used with top Navbar) or SELECT/RADIO (NOT for used with top Navbar)

        private JsonTextWriter _writer; // DN

        private StringWriter _stringWriter = new (); // DN

        // Constructor
        public Lang()
        {
            _writer = new JsonTextWriter(_stringWriter);

            // Set up file list
            LoadFileList().GetAwaiter().GetResult();

            // Set up language id
            string language = Param("language");
            if (!Empty(language) && !Empty(GetFileName(language))) {
                LanguageId = language;
                Session[Config.SessionLanguageId] = LanguageId;
            } else if (!Empty(Session[Config.SessionLanguageId])) {
                LanguageId = Session.GetString(Config.SessionLanguageId);
            } else {
                LanguageId = Config.LanguageDefaultId;
            }
            if (!IsValidLocaleId(LanguageId)) { // Locale not supported by server
                Log("Locale " + LanguageId + " not supported by server.");
                LanguageId = "en-US"; // Fallback to "en-US"
            }
            if (HttpContext != null)
                CurrentLanguage = LanguageId;
            LoadLanguage(LanguageId);

            // Call Language Load event
            LanguageLoad();
            SetClientVar("languages", new { languages = GetLanguages() });
        }

        #pragma warning disable 1998
        // Load language file list
        private async Task LoadFileList()
        {
            Config.LanguageFile = Config.LanguageFile.Select(lang => (dynamic)new {
                Id = lang.Id,
                File = lang.File,
                Desc = LoadFileDesc(MapPath(LanguageFolder + lang.File))
            }).ToList();
        }
        #pragma warning restore 1998

        // Load language file description
        private string LoadFileDesc(string file)
        {
            using var reader = new XmlTextReader(file);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            while (!reader.EOF && reader.Read()) {
                if (reader.IsStartElement() && reader.Name == "ew-language")
                    return reader.GetAttribute("desc") ?? "";
            }
            return "";
        }

        // Load language file
        private void LoadLanguage(string id)
        {
            string fileName = GetFileName(id);
            if (Empty(fileName))
                fileName = GetFileName(Config.LanguageDefaultId);
            if (Empty(fileName))
                return;
            var key = Config.ProjectName + "_" + fileName.Replace(WebRootPath, "").Replace(".xml", "").Replace(Config.PathDelimiter, "_");
            if (Session.GetValue<Dictionary<string, dynamic>>(key) is Dictionary<string, dynamic> d) {
                Phrases = d;
            } else {
                Phrases = XmlToDictionary(fileName);
                Session.SetValue(key, Phrases);
            }
            PhraseData = new (Phrases.ToDictionary(kvp => kvp.Key, kvp => (dynamic?)kvp.Value)); // DN

            // Set up locale settings
            JObject locale = LocaleConvert(id).GetAwaiter().GetResult(); // Sync
            CultureInfo ci = CultureInfo.CreateSpecificCulture(id);
            NumberFormat = locale["number"]?.Value<string>() ?? NumberFormat;
            CurrencyFormat = locale["currency"]?.Value<string>() ?? CurrencyFormat;
            PercentFormat = locale["percent"]?.Value<string>() ?? PercentFormat;
            NumberingSystem = locale["numbering_system"]?.Value<string>() ?? "";
            CurrentNumberFormat = ci.NumberFormat;
            CurrentNumberFormat.NumberDecimalSeparator = locale["decimal_separator"]?.Value<string>() ?? CurrentNumberFormat.NumberDecimalSeparator;
            CurrentNumberFormat.CurrencyDecimalSeparator = locale["decimal_separator"]?.Value<string>() ?? CurrentNumberFormat.CurrencyDecimalSeparator;
            CurrentNumberFormat.PercentDecimalSeparator = locale["decimal_separator"]?.Value<string>() ?? CurrentNumberFormat.PercentDecimalSeparator;
            CurrentNumberFormat.NumberGroupSeparator = locale["grouping_separator"]?.Value<string>() ?? CurrentNumberFormat.CurrencyGroupSeparator;
            CurrentNumberFormat.CurrencyGroupSeparator = locale["grouping_separator"]?.Value<string>() ?? CurrentNumberFormat.NumberGroupSeparator;
            CurrentNumberFormat.PercentGroupSeparator = locale["grouping_separator"]?.Value<string>() ?? CurrentNumberFormat.PercentGroupSeparator;
            CurrentNumberFormat.CurrencySymbol = locale["currency_symbol"]?.Value<string>() ?? CurrentNumberFormat.CurrencySymbol;
            CurrentNumberFormat.CurrencyDecimalSeparator = locale["decimal_separator"]?.Value<string>() ?? CurrentNumberFormat.CurrencyDecimalSeparator;
            CurrentNumberFormat.CurrencyGroupSeparator = locale["grouping_separator"]?.Value<string>() ?? CurrentNumberFormat.CurrencyGroupSeparator;
            CurrentNumberFormat.PercentSymbol = locale["percent_symbol"]?.Value<string>() ?? CurrentNumberFormat.PercentSymbol;
            CurrentNumberFormat.PercentDecimalSeparator = locale["decimal_separator"]?.Value<string>() ?? CurrentNumberFormat.PercentDecimalSeparator;
            CurrentNumberFormat.PercentGroupSeparator = locale["grouping_separator"]?.Value<string>() ?? CurrentNumberFormat.PercentGroupSeparator;
            CurrentDateTimeFormat = ci.DateTimeFormat;
            CurrentDateTimeFormat.DateSeparator = locale["date_separator"]?.Value<string>() ?? CurrentDateTimeFormat.DateSeparator;
            CurrentDateTimeFormat.TimeSeparator = locale["time_separator"]?.Value<string>() ?? CurrentDateTimeFormat.TimeSeparator;
            CurrentDateTimeFormat.ShortDatePattern = Regex.Replace(locale["date"]?.Value<string>() ?? CurrentDateTimeFormat.ShortDatePattern, @"\b(G+)\b", m => m.Value.ToLower()); // Make sure "g" in C# format
            CurrentDateTimeFormat.ShortTimePattern = locale["time"]?.Value<string>() ?? CurrentDateTimeFormat.ShortTimePattern;
        }

        // Convert XML to dictionary
        private Dictionary<string, dynamic> XmlToDictionary(string file) => XElementToDictionary(XElement.Load(file));

        // Convert XElement to dictionary
        private Dictionary<string, dynamic> XElementToDictionary(XElement el)
        {
            var dict = new Dictionary<string, dynamic>(StringComparer.InvariantCultureIgnoreCase);
            if (el.HasElements) {
                foreach (var e in el.Elements()) {
                    var name = e.Name.LocalName;
                    var id = e.Attribute("id")?.Value;
                    if (!dict.ContainsKey(name))
                        dict.Add(name, new Dictionary<string, dynamic>(StringComparer.InvariantCultureIgnoreCase));
                    if (id != null && !e.HasElements && e.Name.LocalName == "phrase") { // phrase
                        var d = e.Attributes().Where(attr => attr.Name.LocalName != "id").ToDictionary(attr => attr.Name.LocalName, attr => (object)attr.Value);
                        dict[name].Add(id, d);
                    } else if (id != null && e.HasElements) { // table, field, menu
                        dict[name].Add(id, XElementToDictionary(e));
                    } else if (id == null && e.HasElements) { // global, project, datetimepicker, etc.
                        dict[name] = XElementToDictionary(e);
                    }
                }
            }
            return dict;
        }

        // Get language file name
        private string GetFileName(string id)
        {
            var file = Config.LanguageFile.FirstOrDefault(lang => lang.Id == id)?.File;
            if (file != null)
                return MapPath(LanguageFolder + file);
            return "";
        }

        // Get phrase
        public string Phrase(string id, bool? useText = false)
        {
            if (Empty(id))
                return "";
            var attrs = PhraseAttrs(id);
            if (attrs == null)
                return id;
            attrs.TryGetValue("class", out object? phraseClass);
            if (!attrs.TryGetValue("value", out object? text))
                text = id; // Return the id if phrase not found
            string res = ConvertToString(text);
            string className = ConvertToString(phraseClass);
            if (useText != true && !Empty(className)) {
                if (useText == null && !Empty(text)) // Use both icon and text
                    className = AppendClass(className, "me-2");
                if (Regex.IsMatch(className, @"\bspinner\b")) // Spinner
                    res = "<div class=\"" + className + "\" role=\"status\"><span class=\"visually-hidden\">" + HtmlEncode(text) + "</span></div>";
                else // Icon
                    res = "<i data-phrase=\"" + id + "\" class=\"" + className + "\"><span class=\"visually-hidden\">" + HtmlEncode(text) + "</span></i>";
                if (useText == null && !Empty(text)) // Use both icon and text
                    res += ConvertToString(text);
            }
            return res;
        }

        // Set phrase
        public void SetPhrase(string id, string value) =>  SetPhraseAttr(id, "value", value);

        // Get data
        public string GetData(string id) => PhraseData.Get(id.ToLowerInvariant()) ?? "";

        // Set data
        public void SetData(string id, string value) => PhraseData.Set(id.ToLowerInvariant(), value);

        // Get project phrase
        public string ProjectPhrase(string id) => GetData($"project.phrase.{id}.value");

        // Set project phrase
        public void SetProjectPhrase(string id, string value) => SetData($"project.phrase.{id}.value", value);

        // Get menu phrase
        public string MenuPhrase(string menuId, string id) => GetData($"project.menu.{menuId}.phrase.{id}.value");

        // Set menu phrase
        public void SetMenuPhrase(string menuId, string id, string value) => SetData($"project.menu.{menuId}.phrase.{id}.value", value);

        // Get table phrase
        public string TablePhrase(string tblVar, string id) => GetData($"project.table.{tblVar}.phrase.{id}.value");

        // Set table phrase
        public void SetTablePhrase(string tblVar, string id, string value) => SetData($"project.table.{tblVar}.phrase.{id}.value", value);

        // Get field phrase
        public string FieldPhrase(string tblVar, string fldVar, string id) => GetData($"project.table.{tblVar}.field.{fldVar}.phrase.{id}.value");

        // Set field phrase
        public void SetFieldPhrase(string tblVar, string fldVar, string id, string value) => SetData($"project.table.{tblVar}.field.{fldVar}.phrase.{id}.value", value);

        // Get chart phrase // DN
        public string ChartPhrase(string tblVar, string fldVar, string id) => GetData($"project.table.{tblVar}.chart.{fldVar}.phrase.{id}.value");

        // Set chart phrase // DN
        public void SetChartPhrase(string tblVar, string fldVar, string id, string value) => SetData($"project.table.{tblVar}.chart.{fldVar}.phrase.{id}.value", value);

        // Get phrase attributes
        public dynamic? PhraseAttrs(string id) => PhraseData.Get($"global.phrase.{id.ToLowerInvariant()}");

        // Get phrase attribute
        public string PhraseAttr(string id, string name = "value") =>
            PhraseAttrs(id) is Dictionary<string, dynamic> d && d.TryGetValue(name, out dynamic? value) ? ConvertToString(value) : "";

        // Set phrase attribute
        public void SetPhraseAttr(string id, string name, string value) {
            if (PhraseAttrs(id) is Dictionary<string, dynamic> d)
                d[name.ToLowerInvariant()] = value;
            else
                SetData($"global.phrase.{id}.{name}", value);
        }

        // Get phrase class
        public string PhraseClass(string id) => PhraseAttr(id, "class");

        // Set phrase attribute
        public void SetPhraseClass(string id, string value) => SetPhraseAttr(id, "class", value);

        // Output phrases as JSON
        public async Task PhrasesToJson(dynamic phrases)
        {
            Dictionary<string, string> dict = ((Dictionary<string, dynamic>)phrases)
                .ToDictionary(kvp => kvp.Key, kvp => (string)kvp.Value["value"]);
            foreach (var (key, value) in dict) {
                await _writer.WritePropertyNameAsync(key);
                await _writer.WriteValueAsync(value);
            }
        }

        // Output dictionary as JSON
        public async Task DictinoaryToJson(dynamic value)
        {
            var dict = (Dictionary<string, dynamic>)(value);
            await _writer.WriteStartObjectAsync();
            foreach (var (key, val) in dict) {
                if (key == "phrase") {
                    await PhrasesToJson(val);
                } else {
                    await _writer.WritePropertyNameAsync(key);
                    await DictinoaryToJson(val);
                }
            }
            await _writer.WriteEndObjectAsync();
        }

        // Output phrases as Json (Async)
        public async Task<string> ToJsonAsync()
        {
            await DictinoaryToJson(PhraseData.Get("global")!);
            return "ew.language.phrases = " + _stringWriter.ToString() + ";";
        }

        // Output phrases as Json
        public string ToJson() => ToJsonAsync().GetAwaiter().GetResult();

        // Get language info
        public List<Dictionary<string, object>> GetLanguages()
        {
            return (Config.LanguageFile.Count <= 1) ? new () :
                Config.LanguageFile.Select(lang => {
                    string id = lang.Id;
                    string desc = Phrase(id);
                    if (desc == id && !Empty(lang.Desc))
                        desc = lang.Desc;
                    bool selected = (id == CurrentLanguage);
                    return new Dictionary<string, object> {
                        {"id", id},
                        {"desc", desc},
                        {"selected", selected}
                    };
                }).ToList();
        }

        // Get template
        public string GetTemplate() =>
            Type.ToUpper() switch {
                "LI" => // LI template (for used with top Navbar)
                    "{{for languages}}<li class=\"nav-item\"><a class=\"nav-link{{if selected}} active{{/if}} ew-tooltip\" title=\"{{>desc}}\" data-ew-action=\"language\" data-language=\"{{:id}}\">{{:id}}</a></li>{{/for}}",
                "DROPDOWN" => // DROPDOWN template (for used with top Navbar)
                    "<li class=\"nav-item dropdown\"><a class=\"nav-link\" data-bs-toggle=\"dropdown\"><i class=\"fa-solid fa-globe ew-icon\"></i></span></a><div class=\"dropdown-menu dropdown-menu-lg dropdown-menu-end\">{{for languages}}<a class=\"dropdown-item{{if selected}} active{{/if}}\" data-ew-action=\"language\" data-language=\"{{:id}}\">{{>desc}}</a>{{/for}}</div></li>",
                "SELECT" => // SELECT template (NOT for used with top Navbar)
                    "<div class=\"ew-language-option\"><select class=\"form-select\" id=\"ew-language\" name=\"ew-language\" data-ew-action=\"language\">{{for languages}}<option value=\"{{:id}}\"{{if selected}} selected{{/if}}>{{:desc}}</option>{{/for}}</select></div>",
                "RADIO" => // RADIO template (NOT for used with top Navbar)
                    "<div class=\"ew-language-option\"><div class=\"btn-group\" data-bs-toggle=\"buttons\">{{for languages}}<input type=\"radio\" name=\"ew-language\" id=\"ew-Language-{{:id}}\" autocomplete=\"off\" data-ew-action=\"language\"{{if selected}} checked{{/if}} value=\"{{:id}}\"><label class=\"btn btn-default ew-tooltip\" for=\"ew-language-{{:id}}\" data-container=\"body\" data-bs-placement=\"bottom\" title=\"{{>desc}}\">{{:id}}</label>{{/for}}</div></div>",
                _ => Template
            };

        // Language Load event
        public void LanguageLoad() {
            // Example:
            //SetPhrase("SaveBtn", "Save Me"); // Refer to language XML file for phrase IDs
        }
    }
} // End Partial class
