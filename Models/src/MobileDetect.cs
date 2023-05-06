namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Mobile Detect class
    /// Based on https://github.com/serbanghita/Mobile-Detect
    /// </summary>
    public class MobileDetect
    {
        public Dictionary<string, List<string>> Properties = new ()
        {
            // Build
            { "Mobile", new () { "Mobile/[VER]" } },
            { "Build", new () { "Build/[VER]" }  },
            { "Version", new () { "Version/[VER]" } },
            { "VendorID", new () { "VendorID/[VER]" } },

            // Devices
            { "iPad", new () { "iPad.*CPU[a-z ]+[VER]" } },
            { "iPhone", new () { "iPhone.*CPU[a-z ]+[VER]" } },
            { "iPod", new () { "iPod.*CPU[a-z ]+[VER]" } },
            { "Kindle", new () { "Kindle/[VER]" } },

            // Browser
            { "Chrome", new () { "Chrome/[VER]", "CriOS/[VER]", "CrMo/[VER]" } },
            { "Coast", new () { "Coast/[VER]" } },
            { "Dolfin", new () { "Dolfin/[VER]" } },
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent/Firefox
            { "Firefox", new () { "Firefox/[VER]", "FxiOS/[VER]" } },
            { "Fennec", new () { "Fennec/[VER]" } },
            // http://msdn.microsoft.com/en-us/library/ms537503(v=vs.85).aspx
            // https://msdn.microsoft.com/en-us/library/ie/hh869301(v=vs.85).aspx
            { "Edge", new () { "Edge/[VER]" } },
            { "IE", new () { "IEMobile/[VER];", "IEMobile [VER]", "MSIE [VER];", "Trident/[0-9.]+;.*rv:[VER]" } },
            // http://en.wikipedia.org/wiki/NetFront
            { "PaleMoon", new () { "PaleMoon/[VER]" } },

            // Engine
            { "Gecko", new () { "Gecko/[VER]" } },
            { "Trident", new () { "Trident/[VER]" } },
            { "Presto", new () { "Presto/[VER]" } },
            { "Goanna", new () { "Goanna/[VER]" } },

            // OS
            { "iOS", new () { @" \bi?OS\b [VER][ ;]{1}" } },
            { "Android", new () { "Android [VER]" } },
            { "BlackBerry", new () { @"BlackBerry[\w]+/[VER]", "BlackBerry.*Version/[VER]", "Version/[VER]" } },
            { "BREW", new () { "BREW [VER]" } },
            { "Java", new () { "Java/[VER]" } },
            // http://windowsteamblog.com/windows_phone/b/wpdev/archive/2011/08/29/introducing-the-ie9-on-windows-phone-mango-user-agent-string.aspx
            // http://en.wikipedia.org/wiki/Windows_NT#Releases
            { "Windows Phone OS", new () { "Windows Phone OS [VER]", "Windows Phone [VER]" } },
            { "Windows Phone", new () { "Windows Phone [VER]" } },
            { "Windows CE", new () { "Windows CE/[VER]" } },
            // http://social.msdn.microsoft.com/Forums/en-US/windowsdeveloperpreviewgeneral/thread/6be392da-4d2f-41b4-8354-8dcee20c85cd
            { "Windows NT", new () { "Windows NT [VER]" } },
            { "Symbian", new () { "SymbianOS/[VER]", "Symbian/[VER]" } },
            { "webOS", new () { "webOS/[VER]", "hpwOS/[VER];" } }
        };

        public JObject Data;

        public string UserAgent;

        public Dictionary<string, StringValues> HttpHeaders;

        public List<JToken> Rules = new ();

        // Constructor
        public MobileDetect()
        {
            Data = LoadJsonData().GetAwaiter().GetResult();
            HttpHeaders = Request?.Headers.ToDictionary(kvp => "HTTP_" + kvp.Key.Replace("-", "_").ToUpper(), kvp => kvp.Value) ?? new (); // Convert header to HTTP_*
            UserAgent = ParseHeadersForUserAgent();
            if (Data["uaMatch"]?["phones"] is IEnumerable<JToken> phones)
                Rules.Concat(phones);
            if (Data["uaMatch"]?["tablets"] is IEnumerable<JToken> tablets)
                Rules.Concat(tablets);
            if (Data["uaMatch"]?["browsers"] is IEnumerable<JToken> browsers)
                Rules.Concat(browsers);
            if (Data["uaMatch"]?["os"] is IEnumerable<JToken> os)
                Rules.Concat(os);
        }

        // Check if the device is mobile
        public bool IsMobile => CheckHttpHeadersForMobile() || MatchDetectionRulesAgainstUa();

        // Check if the device is a tablet
        public bool IsTablet => MatchDetectionRulesAgainstUa(Data["uaMatch"]?["tablets"]);

        // Checks if the device is conforming to the provided key
        // e.g .Is("ios") / .Is("androidos") / .Is("iphone")
        public bool Is(string key) => Rules.Where(rule => SameText(((JProperty)rule).Name, key)) is var rules && rules.Count() > 0
            ? Match(rules.First().Value<string>())
            : false;

        // Get version number
        public double Version(string propertyName)
        {
            // Check if the property exists in the properties array.
            if (Properties.ContainsKey(propertyName)) {
                foreach (string propertyMatchString in Properties[propertyName]) {
                    string propertyPattern = propertyMatchString.Replace("[VER]", @"([\w._\+]+)");
                    // Identify and extract the version
                    var m = Regex.Match(UserAgent, propertyPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (m.Success) {
                        string[] ver = m.Groups[1].Value.Split('.');
                        return ConvertToDouble(ver.Length == 1 ? ver[0] : ver[0] + "." + ver[1]);
                    }
                }
            }
            return -1;
        }

        // Load JSON data
        private async Task<JObject> LoadJsonData() => JsonConvert.DeserializeObject(await FileReadAllText(ServerMapPath("js/Mobile_Detect.json"))) is JObject o
            ? o
            : throw new Exception("Failed to parse Mobile_Detect.json");

        // UA HTTP headers
        private List<string> UaHttpHeaders => Data["uaHttpHeaders"]?.Select(h => h.Value<string>() ?? "").ToList() ?? new ();

        // Parse the headers for the user agent - uses a list of possible keys as provided by upstream
        // returns a concatenated list of possible user agents, should be just 1
        private string ParseHeadersForUserAgent() =>
            String.Join(" ", HttpHeaders.Where(kvp => UaHttpHeaders.Contains(kvp.Key)).Select(kvp => kvp.Value)).Trim();

        // Check the HTTP headers for signs of mobile
        private bool CheckHttpHeadersForMobile() => Data["headerMatch"] is JToken headerMatch && headerMatch.Any(token =>
            token is JProperty jp &&
            HttpHeaders.TryGetValue(jp.Name, out StringValues sv) &&
            jp.Value?["matches"] is JArray ja &&
            ja.Select(m => m.Value<string>()).Any(match => match != null && sv.ToString().Contains(match)));

        // Check custom regexes against the User-Agent string
        private bool Match(JToken? keyRegex, string? uaString = null) => keyRegex?.Value<string>() is string s && Regex.IsMatch(
                uaString ??= UserAgent,
                s.Replace("/", "\\/"), // Escape the special character which is the delimiter
                RegexOptions.IgnoreCase
            );

        // Find a detection rule that matches the current User-agent
        private bool MatchDetectionRulesAgainstUa(IEnumerable<JToken>? rules = null) =>
            (rules ?? Rules).Any(regex => Match(regex));

        // Check if browser disallows SameSite=None
        public bool DisallowSameSiteNone()
        {
            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking
            // stack.
            if (UserAgent.Contains("CPU iPhone OS 12") || UserAgent.Contains("iPad; CPU OS 12"))
                return true;

            // Cover Mac OS X based browsers that use the Mac OS networking stack.
            // This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (UserAgent.Contains("Macintosh; Intel Mac OS X 10_14") && UserAgent.Contains("Version/") && UserAgent.Contains("Safari"))
                return true;

            // Cover Chrome 50-69, because some versions are broken by SameSite=None,
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions,
            // but pre-Chromium Edge does not require SameSite=None.
            if (UserAgent.Contains("Chrome/5") || UserAgent.Contains("Chrome/6"))
                return true;
            return false;
        }
    }
} // End Partial class
