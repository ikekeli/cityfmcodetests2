namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    // Static constructor
    static cityfmcodetests()
    {
        var provider = new FileExtensionContentTypeProvider();
        StaticFileSettings = new ()
        {
            ContentTypeProvider = provider
        };
        Session = new ();
        Cookie = new ();
        Inspector = new ContentInspectorBuilder() {
            Definitions = MimeDetective.Definitions.Default.All()
        }.Build();

        // ContentType Mapping event
        ContentTypeMapping(provider.Mappings);

        // Class Init event
        ClassInit();
    }

    // Null value for database field
    public const object DbNullValue = null; // DN

    // MobileDetect
    private static MobileDetect? _mobileDetect = null;

    // Current HttpContext
    public static HttpContext? HttpContext => HttpContextAccessor.HttpContext;

    // Hosting environment
    [AllowNull]
    public static IWebHostEnvironment HostingEnvironment = null;

    // HttpContext accessor
    [AllowNull]
    private static IHttpContextAccessor HttpContextAccessor = null;

    // Configuration
    [AllowNull]
    private static IConfiguration _config = null;

    // Antiforgery
    [AllowNull]
    public static IAntiforgery Antiforgery = null;

    // Link generator
    [AllowNull]
    public static LinkGenerator LinkGenerator = null;

    // HttpClient factory
    [AllowNull]
    public static IHttpClientFactory HttpClientFactory = null;

    // Root container
    [AllowNull]
    public static ILifetimeScope RootContainer = null;

    // Content inspector
    public static ContentInspector Inspector;

    // Configuration
    public static IConfiguration Configuration
    {
        get => _config;
        set {
            _config = value;
            Config.Init();
        }
    }

    // Web application
    public static WebApplication Application
    {
        set {
            HttpContextAccessor = value.Services.GetRequiredService<IHttpContextAccessor>();
            HostingEnvironment = value.Environment;
            Antiforgery = value.Services.GetRequiredService<IAntiforgery>();
            LinkGenerator = value.Services.GetRequiredService<LinkGenerator>();
            HttpClientFactory = value.Services.GetRequiredService<IHttpClientFactory>();
            RootContainer = value.Services.GetAutofacRoot();
        }
    }

    /// <summary>
    /// Web root path
    /// </summary>
    public static string WebRootPath => HostingEnvironment.WebRootPath;

    /// <summary>
    /// Is development
    /// </summary>
    /// <returns></returns>
    public static bool IsDevelopment() => HostingEnvironment.IsDevelopment();

    /// <summary>
    /// Is production
    /// </summary>
    /// <returns></returns>
    public static bool IsProduction() => HostingEnvironment.IsProduction();

    /// <summary>
    /// Is debug mode
    /// </summary>
    /// <returns></returns>
    public static bool IsDebug() => Config.Debug;

    /// <summary>
    /// Convert string to array
    /// </summary>
    /// <param name="str">Input string</param>
    /// <param name="wildcard">Wildcard, e.g. "*"</param>
    /// <returns>Array of string</returns>
    public static string[] ConvertToArray(string str, string wildcard = "")
    {
        if (Empty(str) || wildcard != "" && SameString(str, wildcard))
            return new string[] { wildcard };
        var strs = str.Split(',').Select(x => x.Trim()).Where(x => x != "");
        if (!Empty(wildcard) && (strs.Count() == 0 || strs.Any(x => SameString(x, wildcard))))
            return new string[] { wildcard };
        else
            return strs.ToArray();
    }

    /// <summary>
    /// Is API request
    /// </summary>
    /// <returns>Whether or not the request is an API request</returns>
    public static bool IsApi(HttpContext? context = null) => (Controller != null) ?
        SameString(Controller.GetType().BaseType?.ToString(), Config.ProjectNamespace + ".Controllers.ApiController") :
        (context ?? HttpContext)?.Request.Path.StartsWithSegments(new PathString(AppPath("api"))) ?? false;

    /// <summary>
    /// Is Json response
    /// </summary>
    /// <returns>Whether or not the request requires a Json response</returns>
    public static bool IsJsonResponse(HttpContext? context = null) => IsApi(context) || Param<bool>("json") || Regex.IsMatch((context ?? HttpContext)?.Request?.Headers["Accept"].ToString() ?? "", @"\bapplication/json\b");

    /// <summary>
    /// Get JWT token
    /// </summary>
    /// <param name="userName">User name</param>
    /// <param name="userID">User ID</param>
    /// <param name="parentUserID">Parent user ID</param>
    /// <param name="userLevelID">User level ID</param>
    /// <param name="minExpiry">Minimum expiry time (seconds)</param>
    /// <param name="permission">Permission</param>
    /// <returns>Token</returns>
    public static string GetJwtToken(string userName, string userID, string parentUserID, string userLevelID, int minExpiry = 0, int permission = 0)
    {
        var claims = new Claim[] {
            new (ClaimTypes.Name, userName),
            new ("Id", userID ?? String.Empty, ClaimValueTypes.String),
            new ("ParentUserId", parentUserID ?? String.Empty, ClaimValueTypes.String),
            new ("UserLevelId", userLevelID ?? String.Empty, ClaimValueTypes.String),
            new ("Permission", ConvertToString(permission), ClaimValueTypes.Integer)
        };
        int expires = minExpiry > 0 ? minExpiry : ConvertToInt(Configuration["Jwt:ExpireTimeAfterLogin"]);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"] ?? ""));
        var fi = typeof(SecurityAlgorithms).GetField(Configuration["Jwt:Algorithm"] ?? "");
        string algorithm = fi?.GetRawConstantValue()?.ToString() ?? SecurityAlgorithms.HmacSha256;
        var creds = new SigningCredentials(key, algorithm);
        var token = new JwtSecurityToken(issuer: Configuration["Jwt:Issuer"],
            audience: Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddSeconds(expires),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Get JWT token
    /// </summary>
    /// <returns>Token</returns>
    public static string GetJwtToken() => ResolveSecurity().JwtToken;

    /// <summary>
    /// Get value from Form or Query
    /// </summary>
    /// <param name="name">Name of parameter</param>
    /// <returns>The parameter value or empty string</returns>
    public static string Param(string name) => Post(name, out StringValues sv)
        ? sv.ToString()
        : Get(name, out StringValues qv)
        ? qv.ToString()
        : "";

    /// <summary>
    /// Get value from Form or Query
    /// </summary>
    /// <param name="name">Name of parameter</param>
    /// <typeparam name="T">Type of the returned value</typeparam>
    /// <returns>The parameter value or default of <c>T</c></returns>
    public static T? Param<T>(string name) => Post(name, out StringValues sv)
        ? ChangeType<T>(sv.ToString())
        : Get(name, out StringValues qv)
        ? ChangeType<T>(qv.ToString())
        : default(T);

    // HttpContext data
    public static HttpDataDictionary HttpData => Container.Resolve<HttpDataDictionary>();

    /// <summary>
    /// Current view (RazorPage)
    /// </summary>
    /// <value>The currrent view (RazorPage)</value>
    public static RazorPage View
    {
        get => HttpData.Get<RazorPage>("VIEW")!;
        set => HttpData["VIEW"] = value;
    }

    /// <summary>
    /// Get current view output
    /// </summary>
    /// <param name="viewName">View name</param>
    /// <param name="page">Page object</param>
    /// <param name="clear">Clear view output or not</param>
    /// <returns>View output</returns>
    public static async Task<string> GetViewOutput(string viewName = "", dynamic? page = null, bool clear = true)
    {
        if (Controller == null)
            return "";
        string result = "";
        // Get ViewOutput from page // DN
        if (page != null) {
            HttpData[page.PageObjName] = page; // Set up page object for View
            CurrentPage = page; // Set up current page
            viewName = "~/Views/" + Config.ControllerName + "/" + viewName + ".cshtml"; // Get correct View
        }
        if (HttpContext != null && Response != null) {
            var context = new ActionContext(HttpContext, Controller.RouteData, new ());
            var originalBody = Response.Body;
            try {
                using var memoryStream = new MemoryStream();
                Response.Body = memoryStream;
                if (!Empty(viewName))
                    await Controller.View(viewName).ExecuteResultAsync(context);
                else
                    await Controller.View().ExecuteResultAsync(context);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(memoryStream);
                result = await reader.ReadToEndAsync();
            } finally {
                Response.Body = originalBody;
            }
            if (clear && !Empty(result))
                Response.Clear();
        }
        return result;
    }

    /// <summary>
    /// Current container lifetime scope
    /// Note: The container should be obtained from HttpContext, the ?? operator is only used to make the property non-nullable.
    /// </summary>
    /// <value>Lifetime scope</value>
    public static ILifetimeScope Container
    {
        get {
            if (HttpContext != null)
                HttpContext.Items["CONTAINER"] ??= HttpContext.RequestServices.GetService<ILifetimeScope>()!;
            return HttpContext?.Items["CONTAINER"] is ILifetimeScope scope ? scope : null!;
        }
    }

    /// <summary>
    /// Current controller
    /// </summary>
    /// <value>The current controller</value>
    public static Controller Controller
    {
        get => HttpData.Get<Controller>("CONTROLLER")!;
        set => HttpData["CONTROLLER"] = value;
    }

    /// <summary>
    /// Temp data
    /// </summary>
    public static ITempDataDictionary TempData => Controller.TempData;

    /// <summary>
    /// Current logger
    /// </summary>
    /// <value>Logger of the current controller</value>
    public static ILogger Logger
    {
        get => HttpData.Get<ILogger>("LOGGER")!;
        set => HttpData["LOGGER"] = value;
    }

    /// <summary>
    /// Personal data file name
    /// </summary>
    /// <value>File name</value>
    public static string PersonalDataFileName
    {
        get => HttpData.Get<string>("PERSONAL_DATA_FILE_NAME") ?? "PersonalData.json";
        set => HttpData["PERSONAL_DATA_FILE_NAME"] = value;
    }

    /// <summary>
    /// Use session
    /// </summary>
    /// <value>Use session or not</value>
    public static bool UseSession
    {
        get {
            if (HttpData.TryGetValue("USE_SESSION", out object? useSession))
                return (bool)useSession;
            bool res = IsApi() ? !Empty(Param(Config.TokenName)) || !Empty(Request?.Headers[Config.TokenName.HeaderCase()].ToString()) : true;
            HttpData["USE_SESSION"] = res;
            return res;
        }
        set => HttpData["USE_SESSION"] = value;
    }

    /// <summary>
    /// Log (debug)
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="args">Arguments for the message</param>
    public static void Log(string message, params object[] args) => LogDebug(message, args);

    /// <summary>
    /// Log objects (debug)
    /// </summary>
    /// <param name="args">Objects to log</param>
    public static void Log(params object[] args)
    {
        foreach (object value in args)
            LogDebug(JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }

    // Log debug
    public static void LogDebug(string message, params object[] args) => Logger?.LogDebug(message, args);

    // Log trace
    public static void LogTrace(string message, params object[] args) => Logger?.LogTrace(message, args);

    // Log info
    public static void LogInformation(string message, params object[] args) => Logger?.LogInformation(message, args);

    // Log warning
    public static void LogWarning(string message, params object[] args) => Logger?.LogWarning(message, args);

    // Log error
    public static void LogError(string message, params object[] args) => Logger?.LogError(message, args);

    // Log critical
    public static void LogCritical(string message, params object[] args) => Logger?.LogCritical(message, args);

    // Log SQL
    public static string LogSql(string sql, params object[] args)
    {
        if (Config.Debug)
            SetDebugMessage(sql);
        if (Config.LogSql)
            LogDebug(sql, args);
        return sql;
    }

    /// <summary>
    /// Route data values
    /// </summary>
    public static IDictionary<string, object?> RouteValues => Controller?.RouteData.Values ?? new RouteValueDictionary();

    /// <summary>
    /// Current route name
    /// </summary>
    public static string RouteName => HttpContext?.GetEndpoint()?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName ?? "";

    /// <summary>
    /// Get route name by partial route names
    /// </summary>
    /// <param name="names">Partial route names, e.g. GetRouteName("carslist", "cars", "list")</param>
    /// <returns></returns>
    public static string GetRouteName(params string[] names) => String.Join('-', names);

    /// <summary>
    /// Get path by route name
    /// </summary>
    /// <param name="endpointName">Route name</param>
    /// <param name="values">Values, e.g. new { p = "Index", query = "somequery", }</param>
    /// <param name="pathBase">URI path base, e.g. new PathString("/Foo/Bar")</param>
    /// <param name="fragment">URI fragment, e.g. new FragmentString("#Fragment")</param>
    /// <param name="options">LinkOptions, e.g. new LinkOptions() { AppendTrailingSlash = true, }). See https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.linkoptions?view=aspnetcore-6.0</param>
    /// <returns>URI with an absolute path based on the provided values</returns>
    public static string? GetPathByName(string endpointName, object? values, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null) =>
        LinkGenerator?.GetPathByName(endpointName, values, !Empty(pathBase) ? pathBase : Request?.PathBase ?? PathString.Empty, fragment, options);

    /// <summary>
    /// Get path by partial route names
    /// </summary>
    /// <param name="names">Partial route names, e.g. "carslist", "cars", "list"</param>
    /// <returns>URI with an absolute path based on the provided values</returns>
    public static string? GetPathByName(params string[] names) => GetPathByName(GetRouteName(names), null, Request?.PathBase ?? PathString.Empty);

    /// <summary>
    /// Current user
    /// </summary>
    public static ClaimsPrincipal? User => HttpContext?.User;

    /// <summary>
    /// Is authenticated
    /// </summary>
    /// <returns>Whether the user is authenticated</returns>
    public static bool IsAuthenticated() => User?.Identities.Any(identity => identity.IsAuthenticated) ?? false;

    /// <summary>
    /// Is authenticated
    /// </summary>
    /// <param name="provider">Provider name, e.g. "Google"</param>
    /// <returns></returns>
    public static bool IsAuthenticated(string provider) => User?.Identities.Any(identity => identity.AuthenticationType == provider) ?? false;

    /// <summary>
    /// Is SAML authenticated
    /// </summary>
    /// <returns>Whether the user is SAML authenticated</returns>
    public static bool IsSamlAuthenticated() => IsAuthenticated(Config.SamlAuthenticationType);

    /// <summary>
    /// Request
    /// </summary>
    public static HttpRequest? Request => HttpContext?.Request;

    /// <summary>
    /// Response
    /// </summary>
    public static HttpResponse? Response => HttpContext?.Response;

    /// <summary>
    /// Form collection
    /// </summary>
    public static IFormCollection Form => (Request?.HasFormContentType ?? false) ? Request.Form : FormCollection.Empty;

    /// <summary>
    /// Query collection
    /// </summary>
    public static IQueryCollection Query => Request?.Query ?? QueryCollection.Empty;

    /// <summary>
    /// Form file collection
    /// </summary>
    public static IFormFileCollection Files => Form.Files;

    /// <summary>
    /// Is HTTP POST
    /// </summary>
    /// <returns>Whether request is HTTP POST</returns>
    public static bool IsPost() => SameText(Request?.Method, "POST");

    /// <summary>
    /// Is HTTP GET
    /// </summary>
    /// <returns>Whether request is HTTP GET</returns>
    public static bool IsGet() => SameText(Request?.Method, "GET");

    /// <summary>
    /// Compare objects as string (case-senstive)
    /// </summary>
    /// <param name="v1">Object</param>
    /// <param name="v2">Object</param>
    /// <returns>Whether the two objects are same string or not</returns>
    public static bool SameString(object? v1, object? v2) =>
        String.Equals(ConvertToString(v1).Trim(), ConvertToString(v2).Trim());

    /// <summary>
    /// Compare objects as string (case insensitive)
    /// </summary>
    /// <param name="v1">Object</param>
    /// <param name="v2">Object</param>
    /// <returns>Whether the two objects are same string or not</returns>
    public static bool SameText(object? v1, object? v2) =>
        String.Equals(ConvertToString(v1).Trim(), ConvertToString(v2).Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Compare objects as integer
    /// </summary>
    /// <param name="v1">Object</param>
    /// <param name="v2">Object</param>
    /// <returns>Whether the two objects are same integer</returns>
    public static bool SameInteger(object? v1, object? v2)
    {
        try {
            long? l1 = v1 is string s1 ? ParseInteger(s1) : ConvertToNullableInt64(v1);
            long? l2 = v2 is string s2 ? ParseInteger(s2) : ConvertToNullableInt64(v2);
            return l1 != null && l2 != null && l1 == l2;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Compare objects as DateTime
    /// </summary>
    /// <param name="v1">Object</param>
    /// <param name="v2">Object</param>
    /// <returns>Whether the two objects are same DateTime</returns>
    public static bool SameDate(object? v1, object? v2)
    {
        try {
            return ConvertToDateTime(v1) is DateTime dt1 && ConvertToDateTime(v2) is DateTime dt2 && dt1 == dt2;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Check if empty string/object
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Whether the value is empty</returns>
    public static bool Empty([NotNullWhen(false)] object? value) => value switch {
            null => true,
            DBNull => true,
            string s => String.IsNullOrWhiteSpace(s),
            string[] ar => ar.Length == 0 || ar.Length == 1 && String.IsNullOrWhiteSpace(ar[0]),
            StringValues sv => StringValues.IsNullOrEmpty(sv),
            _ => String.IsNullOrWhiteSpace(ConvertToString(value))
        };

    /// <summary>
    /// Check if null Or DBNull
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Whether the value is null</returns>
    public static bool IsNull([NotNullWhen(false)] object? value) => value == null || Convert.IsDBNull(value);

    /// <summary>
    /// Check if DBNull
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Whether the value is DBNull</returns>
    public static bool IsDBNull([NotNullWhen(true)] object? value) => Convert.IsDBNull(value);

    /// <summary>
    /// Convert to string
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>string</returns>
    public static string ConvertToString(object? value) => Convert.ToString(value) ?? "";

    /// <summary>
    /// Convert to hex string
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Hex string</returns>
    public static string ConvertToHexString(object? value)
    {
        if (value is byte[] bytes) {
            StringBuilder str = new ();
            StringBuilder nullstr = new ();
            foreach (byte b in bytes) {
                if (b == 0) {
                    nullstr.Append(b.ToString("x2")); // Get null before any non-null data
                } else {
                    str.Append(nullstr.ToString() + b.ToString("x2"));
                    nullstr.Clear();
                }
            }
            return str.ToString();
        }
        return ConvertToString(value);
    }

    /// <summary>
    /// Convert from hex string
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>byte array</returns>
    public static byte[] ConvertFromHexString(string? value)
    {
        try {
            if (value != null && value.Length > 0) {
                int l = value.Length / 2;
                var b = new byte[l];
                for (int i = 0; i < l; ++i)
                    b[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
                return b;
            }
        } catch {
            return new byte[0];
        }
        return new byte[0];
    }

    /// <summary>
    /// Convert object to 32-bit integer
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Integer value or 0 if failure</returns>
    public static int ConvertToInt(object? value)
    {
        try {
            return Convert.ToInt32(value);
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Convert object to 64-bit integer
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Integer value or 0 if failure</returns>
    public static long ConvertToInt64(object? value)
    {
        try {
            return Convert.ToInt64(value);
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Convert object to double
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Double value or 0 if failure</returns>
    public static double ConvertToDouble(object? value)
    {
        try {
            return Convert.ToDouble(value);
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Convert object to double?
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Double value or null if failure</returns>
    public static double? ConvertToNullableDouble(object? value)
    {
        try {
            return Convert.ToDouble(value);
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to int?
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Int32 value or null if failure</returns>
    public static long? ConvertToNullableInt(object? value)
    {
        try {
            return Convert.ToInt32(value);
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to long?
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Int64 value or null if failure</returns>
    public static long? ConvertToNullableInt64(object? value)
    {
        try {
            return Convert.ToInt64(value);
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to decimal
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Decimal value or 0 if failure</returns>
    public static decimal ConvertToDecimal(object? value)
    {
        try {
            return Convert.ToDecimal(value);
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Convert object to DateTime
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="dateTimeFormat">DateTime format</param>
    /// <returns>DateTime value or null</returns>
    public static DateTime? ConvertToDateTime(object? value, string dateTimeFormat = "")
    {
        try {
            return value switch {
                null => null,
                TimeSpan ts => DateTime.Parse(ts.ToString()),
                string s => ParseDateTime(s, dateTimeFormat) is DateTime dt ? dt : null,
                _ => Convert.ToDateTime(value)
            };
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to DateTimeOffset
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="styles">DateTime styles</param>
    /// <returns>DateTimeOffset value or null</returns>
    public static DateTimeOffset? ConvertToDateTimeOffset(object? value, DateTimeStyles styles = DateTimeStyles.AdjustToUniversal)
    {
        try {
            return value != null ? DateTimeOffset.Parse(ConvertToString(value), null, styles) : null;
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to TimeSpan
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="timeFormat">Time format</param>
    /// <returns>TimeSpan value or null</returns>
    public static TimeSpan? ConvertToTimeSpan(object? value, string timeFormat = "")
    {
        try {
            return value switch {
                null => null,
                TimeSpan ts => ts,
                string s => ParseTimeSpan(s, timeFormat) is TimeSpan ts ? ts : null,
                long lng => TimeSpan.FromTicks(lng),
                double dbl => TimeSpan.FromSeconds(dbl),
                _ => null
            };
        } catch {
            return null;
        }
    }

    /// <summary>
    /// Convert object to bool
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Boolean value or false if failure</returns>
    public static bool ConvertToBool(object? value)
    {
        try {
            if (IsNumeric(value))
                return ParseNumber(ConvertToString(value)) is double d && d != 0;
            else if (value is string)
                return SameText(value, "y") || SameText(value, "t") || SameText(value, "true");
            return Convert.ToBoolean(value);
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Convert object to bool
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Boolean value or false if failure</returns>
    public static bool? ConvertToNullableBool(object? value) => value != null ? ConvertToBool(value) : null;

    /// <summary>
    /// Convert input value to boolean value for SQL paramater
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="trueValue">True value</param>
    /// <param name="falseValue">False value</param>
    /// <returns>Object with value convertible to boolean value</returns>
    public static object? ConvertToBool(object? value, string trueValue, string falseValue)
    {
        object? res = value;
        if (!SameString(value, trueValue) && !SameString(value, falseValue))
            res = !Empty(value) ? trueValue : falseValue;
        if (IsNumeric(res)) // Convert to int so it can be converted to bool if necessary
            res = ConvertToInt(res);
        return res;
    }

    /// <summary>
    /// Convert input value to specific type for SQL paramater
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="trueValue">True value</param>
    /// <param name="falseValue">False value</param>
    /// <returns>Object with value convertible to boolean value</returns>
    public static T ConvertToBool<T>(object? value, T trueValue, T falseValue) =>  ConvertToBool(value) ? trueValue : falseValue;

    /// <summary>
    /// Convert object to Guid
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>Guid value or Empty Guid if failure</returns>
    public static Guid? ConvertToGuid(object? value) => value switch
    {
        Guid guid => guid,
        string str => Guid.TryParse(ConvertToString(str), out Guid g) ? g : null,
        _ => null
    };

    /// <summary>
    /// Check if a type is anonymous type
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>The type is anonymous type or not</returns>
    public static bool IsAnonymousType([NotNullWhen(true)] Type? type) =>
        type != null && Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains("AnonymousType")
            && type.Attributes.HasFlag(TypeAttributes.NotPublic);

    /// <summary>
    /// Check if an object is anonymous type
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>The type is anonymous type or not</returns>
    public static bool IsAnonymousType([NotNullWhen(true)] object? obj) => obj != null && IsAnonymousType(obj.GetType());

    /// <summary>
    /// Convert (anonymous) object to IDictionary&lt;string, T&gt;
    /// </summary>
    /// <param name="data">Object to be converted</param>
    /// <typeparam name="T">Type</typeparam>
    /// <returns>Dictionary</returns>
    public static IDictionary<string, T> ConvertToDictionary<T>(object? data)
    {
        if (data == null)
            return new Dictionary<string, T>();
        if (data is IDictionary<string, T> d)
            return d.ToDictionary(kvp => Empty(kvp.Key) ? kvp.Key.ToString() : kvp.Key, kvp => kvp.Value); // Calculated field may not have field names
        if (IsAnonymousType(data))
            return data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => {
                    var v = p.GetValue(data, null);
                    return IsAnonymousType(v)
                        ? (T)ConvertToDictionary<T>(v) // Note: Only supports T = object or dynamic
                        : ChangeType<T>(v);
                });
        return new Dictionary<string, T>();
    }

    /// <summary>
    /// Create and fill an array
    /// </summary>
    /// <param name="count">Number of items in array</param>
    /// <param name="val">Initial value</param>
    /// <typeparam name="T">Data type of the initial value</typeparam>
    /// <returns>Array filled with intial value</returns>
    public static T[] InitArray<T>(int count, T val)
    {
        var ar = new T[count];
        for (int i = 0; i < count; i++)
            ar[i] = val;
        return ar;
    }

    /// <summary>
    /// Get class name(s) as list
    /// </summary>
    /// <param name="attr">The "class" attribute value</param>
    /// <returns>The result "class" attribute value</returns>
    public static List<string> ClassList(string attr)
    {
        return !Empty(attr)
        ? attr.Trim().Split(' ').Distinct().Where(x => !Empty(x)).ToList() // Remove empty and duplicate values
        : new List<string>();
    }

    /// <summary>
    /// Contains CSS class name
    /// </summary>
    /// <param name="attr">Class name(s)</param>
    /// <param name="className">Class name to search</param>
    /// <returns>Whether class name exists</returns>
    public static bool ContainsClass(string attr, string className) => ClassList(attr).Contains(className);

    /// <summary>
    /// Prepend CSS class name
    /// </summary>
    /// <param name="attr">The "class" attribute value</param>
    /// <param name="classname">The class name(s) to prepend</param>
    /// <returns>The result "class" attribute value</returns>
    public static string PrependClass(string attr, string classname) => !Empty(classname) && !Empty(attr)
        ? String.Join(' ', ClassList(classname + " " + attr).Distinct())
        : !Empty(attr) ? attr : classname;

    /// <summary>
    /// Append CSS class name
    /// </summary>
    /// <param name="attr">The "class" attribute value</param>
    /// <param name="classname">The class name(s) to append</param>
    /// <returns>The result "class" attribute value</returns>
    public static string AppendClass(string attr, string classname) => !Empty(classname) && !Empty(attr)
        ? String.Join(' ', ClassList(attr + " " + classname).Distinct())
        : !Empty(attr) ? attr : classname;

    /// <summary>
    /// Remove CSS class name(s) from a "class" attribute value
    /// </summary>
    /// <param name="attr">The "class" attribute value</param>
    /// <param name="classname">The class name(s) to remove</param>
    /// <returns>The result "class" attribute value</returns>
    public static string RemoveClass(string attr, string classname) => !Empty(classname) && !Empty(attr)
        ? String.Join(" ", ClassList(attr).Except(ClassList(classname)))
        : attr;

    /// <summary>
    /// Check CSS class name and convert to lowercase with dashes between words
    /// </summary>
    /// <param name="name">Class name</param>
    /// <returns>Valid class name</returns>
    public static string CheckClassName(string name)
    {
        name = Regex.Replace(name.Replace(new char[] { '_' }, ' '), @"\b([a-zA-Z])", m => m.ToString().ToUpperInvariant()).Replace(" ", ""); // Replace "_a"/"_A" to "A" // DN
        string prefix = Config.ClassPrefix;
        var m = Regex.Match(name, @"^(\d+)(-*)([\-\w]+)");
        if (m.Success) // Cannot start with a digit
            return prefix + m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value.ParamCase();
        m = Regex.Match(name, @"^(-{2,}|-\d+)(-*)([\-\w]+)");
        if (m.Success) // Cannot start with two hyphens or a hyphen followed by a digit
            return prefix + m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value.ParamCase();
        m = Regex.Match(name, @"^(_+)?(-*)([\-\w]+)");
        if (m.Success) // Keep leading underscores
            return m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value.ParamCase();
        return name.ParamCase();
    }

    /// <summary>
    /// Check if culture is supported
    /// </summary>
    /// <param name="id">Locale ID</param>
    /// <returns>Supported</returns>
    public static bool IsValidLocaleId(string id) => CultureInfo.GetCultures(CultureTypes.AllCultures).Any(ci => SameText(ci.Name, id.Replace("_", "-")));

    /// <summary>
    ///  Get locale information
    /// </summary>
    /// <returns>Locale settings</returns>
    public static async Task<JObject> LocaleConvert(string langid)
    {
        string localefile = langid + ".json";
        string localepath = ServerMapPath(Config.LocaleFolder) + localefile;
        string localesfile = "js/locales.json";
        JObject locale;
        JObject? locale2 = null;
        CultureInfo ci = CultureInfo.CreateSpecificCulture(langid);
        if (FileExists(localepath) && JsonConvert.DeserializeObject(await FileReadAllText(localepath)) is JObject j1) { // Load from locale file
            locale = j1;
            locale["id"] = langid;
        } else if (JsonConvert.DeserializeObject(await FileReadAllText(ServerMapPath(localesfile))) is JObject locales && locales[langid] is JObject j2) { // Load from CultureInfo and locales.json
            locale = new JObject();
            locale["id"] = langid;
            locale["desc"] = ci.DisplayName;
            locale2 = j2;
        } else {
            throw new Exception($"Locale '{langid}' not found");
        }
        var fmt = ci.NumberFormat;
        var dtfmt = ci.DateTimeFormat;
        locale["number"] ??= locale2?["number"];
        locale["currency"] ??= locale2?["currency"];
        locale["percent"] ??= locale2?["percent"];
        locale["number_system"] ??= locale2?["number_system"];
        locale["date"] ??= locale2?["date"] != null ? Regex.Replace(locale2?["date"]?.Value<string>() ?? "", @"\by\b", "yyyy") : dtfmt.ShortDatePattern; // Use ICU pattern and "yyyy"
        locale["time"] ??= locale2?["time"] ?? Regex.Replace(dtfmt.ShortTimePattern, "t+", "a"); // Use ICU pattern
        locale["percent_symbol"] ??= fmt.PercentSymbol;
        locale["currency_symbol"] ??= fmt.CurrencySymbol;
        locale["decimal_separator"] ??= fmt.NumberDecimalSeparator;
        locale["grouping_separator"] ??= fmt.NumberGroupSeparator;
        locale["date_separator"] ??= dtfmt.DateSeparator;
        locale["time_separator"] ??= dtfmt.TimeSeparator;
        return locale;
    }

    /// <summary>
    /// Get ICU date/time format pattern
    /// </summary>
    /// <param name="dateFormat">Date format</param>
    /// <returns>ICU date format</returns>
    public static string DateFormat(object dateFormat)
    {
        if (ParseInteger(ConvertToString(dateFormat)) is var id && id.HasValue) { // Predefined format
            return id.Value switch {
                0 or 2 => CurrentDateTimeFormat.ShortDatePattern, // Date
                1 => CurrentDateTimeFormat.ShortDatePattern + " " + CurrentDateTimeFormat.ShortTimePattern, // DateTime
                3 => CurrentDateTimeFormat.ShortTimePattern, // Time
                _ => Config.DateFormats.TryGetValue((int)id.Value, out string? format) ? format : "" // Predefined formats
            };
        } else if (dateFormat is string s && !Empty(s)) { // User defined format
            return s;
        }
        return ""; // Unknown
    }

    /// <summary>
    /// Get database date/time format pattern
    /// </summary>
    /// <param name="dateFormat">Date format</param>
    /// <param name="dbtype">Database type</param>
    /// <returns>Database date format</returns>
    public static string DbDateFormat(object dateFormat, string dbtype)
    {
        string df = DateFormat(dateFormat);
        var symbols = Config.DbDateFormats[dbtype];
        string[] tokens = Regex.Split(df, @"[_\W]").Where(s => s != "").Reverse().ToArray();
        string[] replacements = tokens.Select(t => symbols.TryGetValue(t, out string? v) ? v : t).ToArray();
        return df.Replace(tokens, replacements).Replace("/", CurrentDateTimeFormat.DateSeparator).Replace(":", CurrentDateTimeFormat.TimeSeparator);
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="msg">Message</param>
    /// <param name="newmsg">New message</param>
    public static void AddMessage(ref string msg, string newmsg)
    {
        if (!Empty(newmsg)) {
            if (!Empty(msg))
                msg += "<br>";
            msg += newmsg;
        }
    }

    /// <summary>
    /// Add messages and return the combined message // DN
    /// </summary>
    /// <param name="msg">Message</param>
    /// <param name="newmsg">New message</param>
    /// <returns>Combined message</returns>
    public static string AddMessage(string msg, string newmsg)
    {
        AddMessage(ref msg, newmsg);
        return msg;
    }

    /// <summary>
    /// Add filter by condition
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="newfilter">New filter</param>
    /// <param name="cond">Condition (AND/OR)</param>
    public static void AddFilter(ref string filter, string? newfilter, string cond = "AND")
    {
        if (Empty(newfilter))
            return;
        filter = !Empty(filter)
            ? AddBracketsForFilter(filter, cond) + " " + cond + " " + AddBracketsForFilter(newfilter, cond)
            : newfilter;
    }

    /// <summary>
    /// Add filter by condition and return the combined filter
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="newfilter">New filter</param>
    /// <param name="cond">Condition (AND/OR)</param>
    /// <returns>Combined filter</returns>
    public static string AddFilter(string filter, string? newfilter, string cond = "AND")
    {
        AddFilter(ref filter, newfilter, cond);
        return filter;
    }

    /// <summary>
    /// Add brackets to filter based on condition
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="cond">Condition (AND/OR)</param>
    /// <returns>Filter</returns>
    public static string AddBracketsForFilter(string filter, string cond = "AND")
    {
        if (!Empty(filter)) {
            string filterWrk = filter;
            string pattern = @"\([^()]+?\)";
            while (Regex.IsMatch(filterWrk, pattern)) // Remove nested brackets (...)
                filterWrk = Regex.Replace(filterWrk, pattern, "");
            if (Regex.IsMatch(filterWrk, @"\sOR\s", RegexOptions.IgnoreCase) && SameText(cond, "AND")) // Check for any OR without brackets
                filter = "(" + filter + ")";
        }
        return filter;
    }

    /// <summary>
    /// Get current user IP
    /// </summary>
    /// <returns>IP4 address</returns>
    public static string CurrentUserIpAddress()
    {
        var ipaddr = HttpContext?.Connection.RemoteIpAddress?.ToString() ?? HttpContext?.Connection.LocalIpAddress?.ToString() ?? "";
        if (Empty(ipaddr) || ipaddr == "::1") { // No remote or local IP address or IPv6 enabled machine, check if localhost
            ipaddr = GetIP4Address(Request?.Host.ToString().Split(':')[0]);
            if (ipaddr == "127.0.0.1")
                return ipaddr;
        }
        return ipaddr; // Unknown
    }

    /// <summary>
    /// Is local
    /// </summary>
    /// <returns>Whether current user is local (e.g. 127.0.0.1)</returns>
    public static bool IsLocal() => HttpContext != null && HttpContext.Connection.LocalIpAddress == HttpContext.Connection.RemoteIpAddress ||
        CurrentUserIpAddress() == "127.0.0.1";

    /// <summary>
    /// Get IPv4 Address
    /// </summary>
    /// <param name="host">Host</param>
    /// <returns>IP4 address</returns>
    public static string GetIP4Address(string? host)
    {
        string ipaddr = String.Empty;
        if (host != null) {
            try {
                foreach (IPAddress ipa in Dns.GetHostAddresses(host)) {
                    if (ipa.AddressFamily.ToString() == "InterNetwork")
                        return ipa.ToString();
                }
            } catch {}
        }
        return ipaddr;
    }

    /// <summary>
    /// Get current date in specified format
    /// </summary>
    /// <param name="namedformat">Named format</param>
    /// <returns>Current date</returns>
    public static string CurrentDate(int namedformat) =>
        namedformat switch {
            6 or 10 or 13 or 16 => FormatDateTime(DateTime.Today, 6),
            7 or 11 or 14 or 17=> FormatDateTime(DateTime.Today, 7),
            _ => FormatDateTime(DateTime.Today, 5)
        };

    /// <summary>
    /// Get current date in default date format
    /// </summary>
    /// <returns>Current date</returns>
    public static string CurrentDate() => FormatDateTime(DateTime.Today, 0); // DN

    /// <summary>
    /// Get current time in hh:mm:ss format
    /// </summary>
    /// <returns>Current time</returns>
    public static string CurrentTime() => FormatDateTime(DateTime.Now, 3); // DN

    /// <summary>
    /// Get current date with current time in hh:mm(:ss) format
    /// </summary>
    /// <param name="namedformat">Date format</param>
    /// <returns>Current date/time</returns>
    public static string CurrentDateTime(int namedformat) =>
        namedformat switch {
            6 or 10 or 13 or 16 => FormatDateTime(DateTime.Now, 10),
            7 or 11 or 14 or 17=> FormatDateTime(DateTime.Now, 11),
            _ => FormatDateTime(DateTime.Now, 9)
        };

    /// <summary>
    /// Get current date/time in default date/time format
    /// </summary>
    /// <returns>Current date/time</returns>
    public static string CurrentDateTime() => FormatDateTime(DateTime.Now, 1); // DN

    // HTML Sanitizer
    public static HtmlSanitizer Sanitizer = new ();

    /// <summary>
    /// Remove XSS
    /// </summary>
    /// <param name="html">Input HTML</param>
    /// <returns>Sanitized HTML</returns>
    public static string RemoveXss(object? html)
    {
        if (Empty(html))
            return "";
        if (Config.RemoveXss)
            return Sanitizer.Sanitize(ConvertToString(html));
        return ConvertToString(html);
    }

    // Get session timeout time (seconds)
    public static int SessionTimeoutTime()
    {
        int mlt = 0;
        if (Config.SessionTimeout > 0) // User specified timeout time
            mlt = Config.SessionTimeout * 60;
        if (mlt <= 0)
            mlt = 1200; // Default (1200s = 20min)
        return mlt - 30; // Add some safety margin
    }

    /// <summary>
    /// Get client variable
    /// </summary>
    /// <param name="key">Key name</param>
    /// <param name="subkey">Subkey name</param>
    /// <returns>Value of the variable</returns>
    public static object? GetClientVar(string key = "", string subkey = "")
    {
        if (Empty(key))
            return ClientVariables;
        object? value = null;
        if (!Empty(key) && ClientVariables.TryGetValue(key, out object? obj))
            value = obj;
        if (!Empty(subkey) && value is Dictionary<string, dynamic> dict) {
            if (SameText(key, "tables") && !dict.ContainsKey(subkey)) { // If key is "tables", subkey must be table var.
                if (Resolve(subkey) is DbTableBase tbl)
                    tbl.ToClientVar(new () { "Caption" }, new () { "Caption", "Required", "IsInvalid", "Raw" });
            }
            value = dict.TryGetValue(subkey, out dynamic? obj2) ? obj2 : null;
        }
        return value;
    }

    /// <summary>
    /// Set client variable
    /// </summary>
    /// <param name="key">Key name</param>
    /// <param name="value">Value of the variable</param>
    public static void SetClientVar(string key, object value)
    {
        if (!Empty(key)) {
            if (ClientVariables.TryGetValue(key, out object? obj) && obj != null &&
                (obj is IDictionary<string, dynamic?> || IsAnonymousType(obj)) &&
                (value is IDictionary<string, dynamic?> || IsAnonymousType(value)))
                ClientVariables[key] = Merge(obj, value);
            else
                ClientVariables[key] = value;
        }
    }

    /// <summary>
    /// Convert to Title Case (CAPITAL_CASE => CapitalCase, lowercase => Lowercase)
    /// </summary>
    /// <returns>Title Case</returns>
    public static string TitleCase(string name) => String.Join("", name.Split('_').Select(x => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.ToLowerInvariant())));

    /// <summary>
    /// Title case words (hello world => Hello World)
    /// </summary>
    /// <returns>Title case words</returns>
    public static string TitleCaseWords(string name) => String.Join(" ", name.Split(' ').Where(x => x.Trim() != "").Select(x => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Trim().ToLower())));

    /// <summary>
    /// Get Config client variables
    /// </summary>
    /// <returns>Config client variables</returns>
    public static Dictionary<string, object> ConfigClientVars()
    {
        Dictionary<string, object> values = new ();
        foreach (string name in Config.ConfigClientVars) {
            string configName = String.Join("", name.Split('_').Select(x => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(x.ToLowerInvariant())));
            if (Config.Get(configName) is object obj) // Get Config value
                values.Add(name, obj);
        }
        // Update PROJECT_STYLESHEET_FILENAME
        values["PROJECT_STYLESHEET_FILENAME"] = CssFile(Config.ProjectStylesheetFilename);
        return values;
    }

    /// <summary>
    /// Get Global client variables
    /// </summary>
    /// <returns>Global client variables</returns>
    public static Dictionary<string, object> GlobalClientVars()
    {
        return new () {
            // Global client vars // DN
            { "ROWTYPE_VIEW", RowType.View }, // View RowType
            { "ROWTYPE_ADD", RowType.Add }, // Add RowType
            { "ROWTYPE_EDIT", RowType.Edit }, // Edit RowType
            { "DATE_FORMAT", CurrentDateTimeFormat.ShortDatePattern }, // Date format
            { "TIME_FORMAT", CurrentDateTimeFormat.ShortTimePattern }, // Time format
            { "DATE_SEPARATOR", CurrentDateTimeFormat.DateSeparator }, // Date separator
            { "TIME_SEPARATOR", CurrentDateTimeFormat.TimeSeparator }, // Time separator
            { "DECIMAL_SEPARATOR", CurrentNumberFormat.NumberDecimalSeparator }, // Decimal separator
            { "GROUPING_SEPARATOR", CurrentNumberFormat.NumberGroupSeparator }, // Grouping separator
            { "CURRENCY_SYMBOL", CurrentNumberFormat.CurrencySymbol }, // Currency symbol
            { "NUMBER_FORMAT", NumberFormat }, // Number format
            { "PERCENT_FORMAT", PercentFormat }, // Percent format
            { "CURRENCY_FORMAT", CurrencyFormat }, // Currency format
            { "NUMBERING_SYSTEM", NumberingSystem }, // Numbering system
            { "TOKEN_NAME_KEY", Config.TokenNameKey }, // Token name key
            { "TOKEN_NAME", Config.TokenName }, // Token name

            // Others
            { "CURRENT_USER_NAME", CurrentUserName() },
            { "IS_LOGGEDIN", IsLoggedIn() },
            { "IS_SYS_ADMIN", IsSysAdmin() },
            { "IS_RTL", IsRTL },
            { "IS_AUTOLOGIN", IsAutoLogin() },
            { "LANGUAGE_ID", CurrentLanguageID.Replace("_", "-") },
            { "PATH_BASE", AppPath() }, // Path base
            { "PROJECT_NAME", Config.ProjectName },
            { "SESSION_ID", Encrypt(Session.SessionId) }, // Session ID // DN
            { "ANTIFORGERY_TOKEN_KEY", Config.TokenValueKey }, // "csrf_value" // DN
            { "ANTIFORGERY_TOKEN", CurrentToken }, // CSRF token // DN
            { "API_JWT_AUTHORIZATION_HEADER", "Authorization" }, // API JWT authorization header
            { "API_JWT_TOKEN", GetJwtToken() }, // API JWT token
            { "IMAGE_FOLDER", "wwwroot/images/" }, // Image folder
            { "SESSION_TIMEOUT", Config.SessionTimeout > 0 ? SessionTimeoutTime() : 0 }, // Session timeout time (seconds)
            { "TIMEOUT_URL", AppPath("index") }, // Timeout URL // DN
            { "SERVER_SEARCH_FILTER", Config.SearchFilterOption == "Server" },
            { "CLIENT_SEARCH_FILTER", Config.SearchFilterOption == "Client" }
        };
    }

    /// <summary>
    /// Is remote path
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Whether path is remote</returns>
    public static bool IsRemote(string? path) => Regex.IsMatch(path ?? "", Config.RemoteFilePattern);

    /// <summary>
    /// Get current user name
    /// </summary>
    /// <returns>Current user name</returns>
    public static string CurrentUserName() => ResolveSecurity().CurrentUserName;

    /// <summary>
    /// Get current user ID
    /// </summary>
    /// <returns>Current User ID</returns>
    public static string CurrentUserID() => ResolveSecurity().CurrentUserID;

    /// <summary>
    /// Get current user ID or user name as per Config.LogUserId
    /// </summary>
    /// <returns>Current User ID or name</returns>
    public static string CurrentUser()
    {
        string user;
        if (Config.LogUserId) {
            user = CurrentUserID();
            if (Empty(user)) // Assume Administrator or Anonymous user
                user = IsSysAdmin() ? "-1" : "-2";
        } else {
            user = CurrentUserName();
            if (Empty(user)) // Assume Administrator or Anonymous user
                user = IsSysAdmin() ? Language.Phrase("UserAdministrator") : Language.Phrase("UserAnonymous");
        }
        return user;
    }

    /// <summary>
    /// Get current parent user ID
    /// </summary>
    /// <returns>Current parent user ID</returns>
    public static string CurrentParentUserID() => ResolveSecurity().CurrentParentUserID;

    /// <summary>
    /// Get current user level
    /// </summary>
    /// <returns>Current user level ID</returns>
    public static string CurrentUserLevel() => ResolveSecurity().CurrentUserLevelID;

    /// <summary>
    /// Get current user level name
    /// </summary>
    /// <returns>Current user level name</returns>
    public static string CurrentUserLevelName() => ResolveSecurity().CurrentUserLevelName;

    /// <summary>
    /// Get current user level list
    /// </summary>
    /// <returns>Current user level ID list as comma separated values</returns>
    public static string CurrentUserLevelList() => ResolveSecurity().UserLevelList();

    /// <summary>
    /// Get current user image
    /// </summary>
    /// <returns>Current user image as base 64 string</returns>
    public static string CurrentUserImageBase64() => ResolveProfile()?.GetValue(Config.UserProfileImage) ?? "";

    /// <summary>
    /// Get Current user info
    /// </summary>
    /// <param name="fldname">Field name</param>
    /// <returns>Field value</returns>
    public static object? CurrentUserInfo(string fldname)
    {
        return null;
    }

    /// <summary>
    /// Get user info
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="row">Dictionary</param>
    /// <returns>Field value</returns>
    public static object? GetUserInfo(string fieldName, IDictionary<string, object>? row)
    {
        object? info = null;
        if (row != null && row.TryGetValue(fieldName, out object? value)) {
            info = value;
            if (fieldName == Config.LoginPasswordFieldName && !Config.EncryptedPassword) // Password is saved HTML-encoded
                info = HtmlDecode(info);
        }
        return info;
    }

    /// <summary>
    /// Get user info
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="dr">Data reader</param>
    /// <returns>Field value</returns>
    public static object? GetUserInfo(string fieldName, DbDataReader dr) => GetUserInfo(fieldName, GetDictionary(dr));

    /// <summary>
    /// Get user filter
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <param name="val">Value</param>
    /// <returns>Filter</returns>
    public static string GetUserFilter(string fieldName, object? val)
    {
        return "(0=1)"; // Show no records
    }

    /// <summary>
    /// Get current page ID
    /// </summary>
    /// <returns>Current page ID</returns>
    public static string CurrentPageID() => CurrentPage?.PageID ?? "";

    // Page title
    private static string? _title = null;

    /// <summary>
    /// Get/Set current page title
    /// </summary>
    /// <returns>Current page title</returns>
    //
    public static string CurrentPageTitle
    {
        get => CurrentPage?.Title ?? _title ?? Language.ProjectPhrase("BodyTitle");
        set {
            if (GetProperty(CurrentPage, "Title") != null)
                CurrentPage!.Title = value;
            else
                _title = value;
        }
    }

    // Check if user password expired
    public static bool IsPasswordExpired() => ResolveSecurity().IsPasswordExpired;

    // Check if user password reset
    public static bool IsPasswordReset() => ResolveSecurity().IsPasswordReset;

    // Set session password expired
    public void SetSessionPasswordExpired() => ResolveSecurity().SetSessionPasswordExpired();

    // Check if user is logging in (after changing password)
    public static bool IsLoggingIn() => ResolveSecurity().IsLoggingIn;

    // Check if user is logging in (2FA)
    public static bool IsLoggingIn2FA() => ResolveSecurity().IsLoggingIn2FA;

    /// <summary>
    /// Is logged in
    /// </summary>
    /// <returns>Whether current user is logged in</returns>
    public static bool IsLoggedIn() => ResolveSecurity().IsLoggedIn;

    // Is auto login
    public static bool IsAutoLogin() => SameString(Session[Config.SessionUserLoginType], "a");

    // Get current page heading
    public static string CurrentPageHeading()
    {
        if (Config.PageTitleStyle != "Title") {
            string heading = CurrentPage?.PageHeading ?? "";
            if (!Empty(heading))
                return heading;
        }
        return Language.ProjectPhrase("BodyTitle");
    }

    // Get current page subheading
    public static string CurrentPageSubheading()
    {
        string heading = "";
        if (Config.PageTitleStyle != "Title")
            heading = CurrentPage?.PageSubheading ?? "";
        return heading;
    }

    // Set up menus
    public static void SetupMenus()
    {
        // Navbar menu
        var topMenu = new Menu("navbar", true, true);
        topMenu.AddMenuItems(Config.TopMenuItems);
        TopMenu = topMenu.ToScript();

        // Sidebar menu
        var sideMenu = new Menu("menu", true, false);
        sideMenu.AddMenuItems(Config.MenuItems);
        SideMenu = sideMenu.ToScript();
    }

    // Set up login status
    public static void SetupLoginStatus()
    {
        LoginStatus["isLoggedIn"] = IsLoggedIn();
        LoginStatus["currentUserName"] = CurrentUserName();
        string currentPage = CurrentPageName();
        string logoutPage = "logout";
        string logoutUrl = GetUrl(logoutPage);
        LoginStatus["logout"] = new Dictionary<string, object>() {
            { "ew-action", "redirect" },
            { "url", logoutUrl }
        };
        LoginStatus["logoutUrl"] = logoutUrl;
        LoginStatus["logoutText"] =  Language.Phrase("Logout", null);
        LoginStatus["canLogout"] = !Empty(logoutPage) && IsLoggedIn();
        string loginPage = "login";
        string loginUrl = GetUrl(loginPage);
        if (currentPage != loginPage) {
            if (Config.UseModalLogin && !IsMobile()) {
                LoginStatus["login"] = new Dictionary<string, object>() {
                    { "ew-action", "modal" },
                    { "footer", false },
                    { "caption", Language.Phrase("Login", true) },
                    { "size", "modal-md" },
                    { "url", loginUrl }
                };
            } else {
                LoginStatus["login"] = new Dictionary<string, object>() {
                    { "ew-action", "redirect" },
                    { "url", loginUrl }
                };
            }
        } else {
            LoginStatus["login"] = new Dictionary<string, object>() {
                { "url", loginUrl }
            };
        }
        LoginStatus["loginTitle"] = Language.Phrase("Login", true);
        LoginStatus["loginText"] = Language.Phrase("Login");
        LoginStatus["canLogin"] = !(bool)LoginStatus["canLogout"] && currentPage != loginPage && !Empty(loginPage) && !Empty(loginUrl) && !IsLoggedIn() && !IsLoggingIn2FA();
    }

    /// <summary>
    /// Is Export
    /// </summary>
    /// <param name="format">Export format. If unspecified, any format.</param>
    /// <returns>Whether the page is in export mode</returns>
    public static bool IsExport(string format = "")
    {
        string exportType = !Empty(ExportType) ? ExportType : Param("export");
        return Empty(format) ? !Empty(exportType) : SameText(exportType, format);
    }

    /// <summary>
    /// Is System Admin
    /// </summary>
    /// <returns>Whether the current user is hard-coded system administrator</returns>
    public static bool IsSysAdmin() => ResolveSecurity().IsSysAdmin;

    /// <summary>
    /// Is Admin
    /// </summary>
    /// <returns>Whether the current user is system administrator (hard-coded or of Administrator User Level)</returns>
    public static bool IsAdmin() => ResolveSecurity().IsAdmin;

    /// <summary>
    /// Get current master table object
    /// </summary>
    /// <value>The master table object</value>
    public static dynamic? CurrentMasterTable
    {
        get {
            string masterTableName = CurrentPage?.CurrentMasterTable ?? "";
            return !Empty(masterTableName) && HttpData.TryGetValue(masterTableName, out object? table) && table != null ? (dynamic)table : null;
        }
    }

    /// <summary>
    /// Get foreign key URL
    /// </summary>
    /// <param name="name">Key name</param>
    /// <param name="val">Key value</param>
    /// <param name="dateFormat">Date format</param>
    /// <returns>Key URL</returns>
    public static string ForeignKeyUrl(string name, object? val, int? dateFormat = null)
    {
        if (IsNull(val)) // DN
            val = Config.NullValue;
        else if (Empty(val))
            val = Config.EmptyValue;
        else if (dateFormat is int df)
            val = UnformatDateTime(val, ConvertToString(df));
        return name + "=" + UrlEncode(val);
    }

    /// <summary>
    /// Get filter for a primary/foreign key field
    /// </summary>
    /// <param name="fld">Field object</param>
    /// <param name="val">Key value</param>
    /// <param name="dataType">DataType* of value</param>
    /// <param name="dbid">Database id</param>
    /// <returns>Key SQL</returns>
    public static string KeyFilter(DbField fld, object? val, DataType dataType, string dbid)
    {
        string expression = fld.Expression;
        if (SameText(val, Config.NullValue))
            return expression + " IS NULL";
        string fldVal = val is string s ? s : ConvertToString(val);
        if (SameText(fldVal, Config.EmptyValue) || val == null) // DN
            fldVal = "";
        string dbType = GetConnectionType(dbid);
        string fldOpr = "=";
        if (fld.DataType == DataType.Number && (dataType == DataType.String || dataType == DataType.Memo)) { // Find field value (number) in input value (string)
            if (dbType == "MYSQL") // MySQL, use FIND_IN_SET(expr, val)
                fldOpr = "FIND_IN_SET";
            else { // Other database type, use expr IN (val)
                fldOpr = "IN";
                fldVal = fldVal.Replace(Config.MultipleOptionSeparator.ToString(), Config.InOperatorValueSeparator);
            }
            return SearchFilter(expression, fldOpr, fldVal, dataType, dbid);
        } else if ((fld.DataType == DataType.String || fld.DataType == DataType.Memo) && dataType == DataType.Number) { // Find input value (number) in field value (string)
            return MultiValueFilter(expression, fldVal, dbid);
        } else { // Assume same data type
            return SearchFilter(expression, "=", fldVal, dataType, dbid);
        }
    }

    /// <summary>
    /// Search field for multi-value
    /// </summary>
    /// <param name="expression">Search expression</param>
    /// <param name="val">Value</param>
    /// <param name="dbid">Database id</param>
    /// <param name="opr">Operator</param>
    /// <returns>Multi value filter</returns>
    public static string MultiValueFilter(string expression, object val, string dbid, string opr = "=")
    {
        string dbtype = GetConnectionType(dbid);
        List<string> list = val is List<string> l ? l : new List<string> { ConvertToString(val) };
        return String.Join(opr == "=" ? " OR " : " AND ",
            list.Select(v =>
                dbtype == "MYSQL"
                ? (opr == "=" ? "" : "NOT ") + "FIND_IN_SET('" + AdjustSql(v, dbid) + "', " + expression + ")" // MySQL, use FIND_IN_SET(val, expr)
                : MultiSearchSqlFilter(expression, opr, v, dbid, Config.MultipleOptionSeparator.ToString())) // Other database type, use (expr = 'val' OR expr LIKE 'val,%' OR expr LIKE '%,val,%' OR expr LIKE '%,val')
            .ToList());
    }

    /// <summary>
    /// Get foreign key value
    /// </summary>
    /// <param name="val">Key value</param>
    /// <returns>Key value</returns>
    public static string? ForeignKeyValue(object? val)
    {
        if (SameText(val, Config.NullValue))
            return null;
        if (SameText(val, Config.EmptyValue))
            return "";
        return ConvertToString(val);
    }

    /// <summary>
    /// Current language ID
    /// </summary>
    public static string CurrentLanguageID => CurrentLanguage;

    /// <summary>
    /// Current language ID
    /// </summary>
    public static bool IsRTL => Config.RtlLanguages.Contains(CurrentLanguage.Split('-').First(), StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Current project ID
    /// </summary>
    public static string CurrentProjectID => CurrentPage?.ProjectID ?? Config.ProjectId;

    /// <summary>
    /// Encrypt with key (TripleDES)
    /// </summary>
    /// <param name="data">Data to encrypt</param>
    /// <param name="key">key</param>
    /// <returns>Encrypted data</returns>
    public static string Encrypt(object data, string key)
    {
        if (Empty(data))
            return "";
        byte[] results = {};
        try {
            var hashProvider = MD5.Create();
            var bytes = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            var algorithm = TripleDES.Create();
            algorithm.Key = bytes;
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;
            try {
                var dataToEncrypt = Encoding.UTF8.GetBytes(ConvertToString(data));
                var encryptor = algorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            } finally {
                algorithm.Clear();
                hashProvider.Clear();
            }
        } catch {}
        return Convert.ToBase64String(results).Replace("+", "-").Replace("/", "_").Replace("=", ""); // Remove padding
    }

    /// <summary>
    /// Encrypt with random key (TripleDES)
    /// </summary>
    /// <param name="data">Data to encrypt</param>
    /// <returns>Encrypted data</returns>
    public static string Encrypt(object data) => Encrypt(data, Config.RandomKey);

    /// <summary>
    /// Decrypt with key (TripleDES)
    /// </summary>
    /// <param name="data">Data to decrypt</param>
    /// <param name="key">key</param>
    /// <returns>Decrypted data</returns>
    public static string Decrypt(object data, string key)
    {
        if (Empty(data))
            return "";
        byte[] result = {};
        try {
            var hashProvider = MD5.Create();
            var bytes = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            var algorithm = TripleDES.Create();
            algorithm.Key = bytes;
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;
            var str = ConvertToString(data).Replace("-", "+").Replace("_", "/");
            var len = str.Length;
            if (len % 4 != 0)
                str = str.PadRight(len + 4 - len % 4, '='); // Add padding
            try {
                var dataToDecrypt = Convert.FromBase64String(str);
                var decryptor = algorithm.CreateDecryptor();
                result = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            } finally {
                algorithm.Clear();
                hashProvider.Clear();
            }
        } catch {}
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// Decrypt with random key (TripleDES)
    /// </summary>
    /// <param name="data">Data to decrypt</param>
    /// <returns>Decrypted data</returns>
    public static string Decrypt(object data) => Decrypt(data, Config.RandomKey);

    /// <summary>
    /// Encrypt with key (AES)
    /// </summary>
    /// <param name="input">String to encrypt</param>
    /// <param name="password">Password</param>
    /// <returns>Encrypted string</returns>
    public static string AesEncrypt(string input, string password) => Aes.Encrypt(input, password);

    /// <summary>
    /// Encrypt with random key as password (AES)
    /// </summary>
    /// <param name="input">String to encrypt</param>
    /// <returns>Encrypted string</returns>
    public static string AesEncrypt(string input) => Aes.Encrypt(input, Config.EncryptionKey);

    /// <summary>
    /// Decrypt with key (AES)
    /// </summary>
    /// <param name="input">String to decrypt</param>
    /// <param name="password">Password</param>
    /// <returns>Decrypted string</returns>
    public static string AesDecrypt(string input, string password) => Aes.Decrypt(input, password);

    /// <summary>
    /// Decrypt with random key as password (AES)
    /// </summary>
    /// <param name="input">String to decrypt</param>
    /// <returns>Decrypted string</returns>
    public static string AesDecrypt(string input) => Aes.Decrypt(input, Config.EncryptionKey);

    /// <summary>
    /// Save byte array to file
    /// </summary>
    /// <param name="folder">Folder</param>
    /// <param name="fn">File name</param>
    /// <param name="filedata">File data</param>
    /// <returns>Whether the action is successful</returns>
    public static async Task<bool> SaveFile(string folder, string fn, byte[] filedata)
    {
        if (CreateFolder(folder)) {
            try {
                await FileWriteAllBytes(IncludeTrailingDelimiter(folder, true) + fn, filedata);
                return true;
            } catch {
                if (Config.Debug)
                    throw;
            }
        }
        return false;
    }

    /// <summary>
    /// Save string to file
    /// </summary>
    /// <param name="folder">Folder</param>
    /// <param name="fn">File name</param>
    /// <param name="filedata">File data</param>
    /// <returns>Whether the action is successful</returns>
    public static async Task<bool> SaveFile(string folder, string fn, string filedata)
    {
        if (CreateFolder(folder)) {
            try {
                await FileWriteAllText(IncludeTrailingDelimiter(folder, true) + fn, filedata);
                return true;
            } catch {
                if (Config.Debug)
                    throw;
            }
        }
        return false;
    }

    // Read global debug message
    public static string GetDebugMessage()
    {
        var msg = DebugMessage;
        DebugMessage = "";
        return (!IsExport() && !Empty(msg)) ? Config.DebugMessageTemplate.Replace("%t", Language.Phrase("Debug")).Replace("%s", msg) : "";
    }

    // Write global debug message // DN
    public static void SetDebugMessage(string v)
    {
        string[] ar = Regex.Split(v.Trim(), @"<(hr|br)>");
        v = String.Join("; ", ar.ToList().Where(s => !Empty(s)).Select(s => s.Trim()));
        DebugMessage = AddMessage(DebugMessage, "<p><samp>" + GetElapsedTime() + ": " + v + "</samp></p>");
    }

    // Save global debug message
    public static void SaveDebugMessage()
    {
        if (Config.Debug)
            Session[Config.SessionDebugMessage] = DebugMessage;
    }

    // Load global debug message
    public static void LoadDebugMessage()
    {
        if (Config.Debug) {
            DebugMessage = Session.GetString(Config.SessionDebugMessage);
            Session[Config.SessionDebugMessage] = "";
        }
    }

    // Permission denied message
    public static string DeniedMessage(string? url = null) => Language.Phrase("NoPermission").Replace("%s", url ?? ScriptName());

    // Set failure Message
    public static void SetFailureMessage(string message) => Session[Config.SessionFailureMessage] = message;

    /// <summary>
    /// Is local URL
    /// </summary>
    /// <param name="url">URL</param>
    /// <returns>Whether URL is local</returns>
    public static bool IsLocalUrl(string url)
    {
        if (Empty(url))
            return false;

        // Allows "/" or "/foo" but not "//" or "/\".
        if (url[0] == '/') {
            // url is exactly "/"
            if (url.Length == 1)
                return true;

            // url doesn't start with "//" or "/\"
            if (url[1] != '/' && url[1] != '\\')
                return true;
            return false;
        }

        // Allows "foo" relative url.
        if (!IsAbsoluteUrl(url))
            return true;
        return false;
    }

    /// <summary>
    /// Get claim value
    /// </summary>
    /// <param name="type">Claim type</param>
    /// <returns>Claim value</returns>
    public static string? ClaimValue(string type) => User?.Claims?.FirstOrDefault(d => SameText(type, d.Type))?.Value;

    /// <summary>
    /// Get connection object
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <param name="name">Registered name</param>
    /// <returns>DatabaseConnection instance</returns>
    public static dynamic GetConnection(string dbid = "DB", string name = "") => ResolveConnection(Db(dbid)["id"] ?? "", name); // DN

    /// <summary>
    /// Get secondary connection object
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>DatabaseConnection instance</returns>
    public static dynamic GetConnection2(string dbid = "DB") => GetConnection(dbid, Config.SecondaryConnectionName); // DN

    /// <summary>
    /// Get connection object
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <param name="name">Registered name</param>
    /// <returns>DatabaseConnection instance</returns>
    public static async Task<dynamic> GetConnectionAsync(string dbid = "DB", string name = "") => await Task.FromResult(GetConnection(dbid, name)); // DN

    /// <summary>
    /// Get secondary connection object
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>DatabaseConnection instance</returns>
    public static Task<dynamic> GetConnection2Async(string dbid = "DB") => GetConnectionAsync(dbid, Config.SecondaryConnectionName); // DN

    /// <summary>
    /// DB helper
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>DatabaseConnection instance</returns>
    public static dynamic DbHelper(string dbid = "DB") => GetConnection2(dbid);

    /// <summary>
    /// Get database connection info
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>ConfigurationSection of connection info</returns>
    public static ConfigurationSection Db(string dbid = "DB") => (ConfigurationSection)Configuration.GetSection("Databases:" + dbid); // Result will not be null

    /// <summary>
    /// Get database connection string
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>Connection string</returns>
    public static string GetConnectionString(string dbid = "DB") => Db(dbid)["connectionstring"] ?? "";

    /// <summary>
    /// Get database connection string builder
    /// </summary>
    /// <param name="connStr">Connection string</param>
    /// <returns>DbConnectionStringBuilder</returns>
    public static DbConnectionStringBuilder GetConnectionStringBuilder(string connStr = "")
    {
        DbConnectionStringBuilder builder = new ();
        if (!Empty(connStr))
            builder.ConnectionString = connStr;
        return builder;
    }

    /// <summary>
    /// Resolve object by name from Config.NamedTypes
    /// </summary>
    /// <param name="name">Name from NamedTypes or class name</param>
    /// <returns>Object</returns>
    public static dynamic? Resolve(string name)
    {
        bool named = Config.NamedTypes.ContainsKey(name);
        Type? t = named ? Config.NamedTypes[name] : Type.GetType(Config.ProjectClassName + "+" + name);
        return t != null ? (named ? Container.ResolveNamed(name, t) : Container.Resolve(t)) : null;
    }

    /// <summary>
    /// Resolve object by name from Config.NamedTypes
    /// </summary>
    /// <param name="name">Name from NamedTypes</param>
    /// <returns>Object</returns>
    public static T? Resolve<T>(string name) where T : notnull => Container.ResolveNamed<T>(name);

    /// <summary>
    /// Resolve object
    /// </summary>
    /// <returns>Object</returns>
    public static T? Resolve<T>() where T : notnull => Container.Resolve<T>();

    /// <summary>
    /// Resolve language object
    /// </summary>
    /// <returns>Language object</returns>
    public static Lang ResolveLanguage()
    {
        Language ??= Container.Resolve<Lang>();
        return Language;
    }

    /// <summary>
    /// Resolve security object
    /// </summary>
    /// <returns>Security object</returns>
    public static AdvancedSecurity ResolveSecurity()
    {
        Security ??= Container.Resolve<AdvancedSecurity>();
        return Security;
    }

    /// <summary>
    /// Resolve user profile object
    /// </summary>
    /// <returns>UserProfile object</returns>
    public static UserProfile ResolveProfile()
    {
        Profile ??= Container.Resolve<UserProfile>();
        return Profile;
    }

    /// <summary>
    /// Get connection type
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <returns>Database type</returns>
    public static string GetConnectionType(string dbid = "DB")
    {
        var db = Db(dbid);
        string type = db["type"] ?? "";
        if (SameText(type, "ODBC") && ConvertToBool(db["msaccess"]))
            type = "ACCESS";
        return type;
    }

    /// <summary>
    /// Resolve connection
    /// </summary>
    /// <param name="dbid">Database ID</param>
    /// <param name="name">Registered name</param>
    /// <returns>DatabaseConnection</returns>
    protected static dynamic ResolveConnection(string dbid = "DB", string name = "")
    {
        if (Empty(dbid))
            throw new Exception("Missing database ID");
        return GetConnectionType(dbid).ToUpper() switch {
            "MYSQL" => Container.ResolveNamed<DatabaseConnection<MySqlConnection, MySqlCommand, MySqlDataReader, MySqlDbType>>(dbid + name, new NamedParameter("dbid", dbid)),
            _ => throw new Exception("Database type not supported")
        };
    }

    // Get last insert ID SQL // DN
    public static string GetLastInsertIdSql(string dbid = "DB") =>
        GetConnectionType(dbid).ToUpper() switch {
            "MYSQL" => "SELECT LAST_INSERT_ID()",
            "SQLITE" => "SELECT LAST_INSERT_ROWID()",
            "ACCESS" => "SELECT @@Identity",
            "MSSQL" => "SELECT SCOPE_IDENTITY()",
            _ => ""
        };

    // Get SQL parameter symbol // DN
    public static string GetSqlParamSymbol(string dbid = "DB") =>
        GetConnectionType(dbid).ToUpper() switch {
            "MYSQL" => "@",
            "SQLITE" => "@",
            "MSSQL" => "@",
            "POSTGRESQL" => ":",
            "ORACLE" => ":",
            _ => "?"
        };

    // Get a row as Dictionary<string, object> from data reader // DN
    public static Dictionary<string, object>? GetDictionary(DbDataReader? dr)
    {
        if (dr != null) {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < dr.FieldCount; i++) {
                try {
                    if (!Empty(dr.GetName(i))) {
                        dict[dr.GetName(i)] = dr[i];
                    } else {
                        dict[i.ToString()] = dr[i]; // Convert index to string as key
                    }
                } catch {}
            }
            return dict;
        }
        return null;
    }

    // Resize binary to thumbnail (interpolation obsolete)
    [Obsolete("The 'interpolation' parameter is obsolete, please remove.", true)]
    public static bool ResizeBinary(ref byte[] filedata, ref int width, ref int height, int interpolation, List<Action<MagickImage>>? plugins = null) => false;

    // Resize binary to thumbnail
    public static bool ResizeBinary(ref byte[] filedata, ref int width, ref int height, List<Action<MagickImage>>? plugins = null, bool? resizeIgnoreAspectRatio = null)
    {
        try {
            MagickImage img = new (filedata);
            GetResizeDimension(img.Width, img.Height, ref width, ref height);
            if (width > 0 && height > 0) {
                MagickGeometry size = new (width, height);
                size.IgnoreAspectRatio = resizeIgnoreAspectRatio ?? Config.ResizeIgnoreAspectRatio;
                size.Less = Config.ResizeLess;
                img.Resize(size);
            }
            plugins?.ForEach(p => p(img));
            filedata = img.ToByteArray();
            width = img.Width;
            height = img.Height;
            return true;
        } catch {
            //if (Config.Debug) throw;
            return false;
        }
    }

    // Resize file to thumbnail file (interpolation obsolete)
    [Obsolete("The 'interpolation' parameter is obsolete, please remove.", true)]
    public static bool ResizeFile(string fn, string tn, ref int width, ref int height, int interpolation) => false;

    // Resize file to thumbnail file
    public static bool ResizeFile(string fn, ref int width, ref int height, bool? resizeIgnoreAspectRatio = null) => ResizeFile(fn, fn, ref width, ref height, resizeIgnoreAspectRatio);

    // Resize file to thumbnail file
    public static bool ResizeFile(string fn, string tn, ref int width, ref int height, bool? resizeIgnoreAspectRatio = null)
    {
        if (!FileExists(fn))
            return false;
        try {
            MagickImage img = new (fn);
            GetResizeDimension(img.Width, img.Height, ref width, ref height);
            if (width > 0 && height > 0) {
                MagickGeometry size = new (width, height);
                size.IgnoreAspectRatio = resizeIgnoreAspectRatio ?? Config.ResizeIgnoreAspectRatio;
                size.Less = Config.ResizeLess;
                img.Resize(size);
                img.Write(tn);
                width = img.Width;
                height = img.Height;
            } else {
                FileCopy(fn, tn, false); // No resize, just use the original file
            }
            return true;
        } catch {
            //if (Config.Debug) throw;
            return false;
        }
    }

    // Resize file to binary (interpolation obsolete)
    [Obsolete("The 'interpolation' parameter is obsolete, please remove.", true)]
    public static byte[] ResizeFileToBinary(string fn, ref int width, ref int height, int interpolation) => new byte[] {};

    // Resize file to binary
    public static byte[] ResizeFileToBinary(string fn, ref int width, ref int height, bool? resizeIgnoreAspectRatio = null)
    {
        if (!FileExists(fn))
            throw new Exception($"File '{fn}' not exist");
        MagickImage img = new (fn);
        if (width > 0 || height > 0) {
            try {
                MagickGeometry size = new (width, height);
                size.IgnoreAspectRatio = resizeIgnoreAspectRatio ?? Config.ResizeIgnoreAspectRatio;
                size.Less = Config.ResizeLess;
                img.Resize(size);
                width = img.Width;
                height = img.Height;
            } catch {
                if (Config.Debug)
                    throw;
            }
        }
        return img.ToByteArray();
    }

    // Set up resize width/height
    private static void GetResizeDimension(int imageWidth, int imageHeight, ref int resizeWidth, ref int resizeHeight)
    {
        if (resizeWidth <= 0) // maintain aspect ratio
            resizeWidth = ConvertToInt(imageWidth * resizeHeight / imageHeight);
        else if (resizeHeight <= 0) // maintain aspect ratio
            resizeHeight = ConvertToInt(imageHeight * resizeWidth / imageWidth);
    }

    /// <summary>
    /// Get content type
    /// </summary>
    /// <param name="data">File data</param>
    /// <param name="fn">File name</param>
    /// <returns>Content type</returns>
    public static string ContentType(byte[] data, string fn = "")
    {
        string? result = null;
        if (data != null)
            result = Inspector.Inspect(data).ByMimeType().FirstOrDefault()?.MimeType;
        else if (fn != "")
            result = ContentType(fn);
        return result ?? Config.DefaultMimeType;
    }

    /// <summary>
    /// Get content type
    /// </summary>
    /// <param name="fn">File name</param>
    /// <returns>Content type</returns>
    public static string ContentType(string fn) => ContentTypeProvider.TryGetContentType(fn, out string? contentType) ? contentType : Config.DefaultMimeType; // DN

    /// <summary>
    /// Return multi-value search SQL
    /// </summary>
    /// <param name="fld">Field object</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fldVal">Search value</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Multi-value search SQL</returns>
    public static string GetMultiSearchSql(DbField fld, string fldOpr, string fldVal, string dbid = "DB")
    {
        string wrk = "";
        DataType fldDataType = fld.DataType;
        fldOpr = ConvertSearchOperator(fldOpr, fld, fldVal);
        if ((new[] { "IS NULL", "IS NOT NULL", "IS EMPTY", "IS NOT EMPTY" }).Contains(fldOpr)) {
            return SearchFilter(fld.Expression, fldOpr, fldVal, fldDataType, dbid);
        } else {
            List<string> values = fldVal.Split(Config.MultipleOptionSeparator).ToList();
            if (fld.UseFilter) { // Use filter
                return String.Join(" OR ", values.Select(v => SearchFilter(fld.Expression, fldOpr, v.Trim(), fldDataType, dbid)));
            } else {
                string dbType = GetConnectionType(dbid);
                int searchOption = Config.SearchMultiValueOption;
                if (searchOption == 1 || !IsMultiSearchOperator(fldOpr)) { // No multiple value search
                    wrk = SearchFilter(fld.Expression, fldOpr, fldVal, DataType.String, dbid);
                } else { // Handle multiple search operator
                    string sql = "";
                    string searchCond = searchOption == 3 ? "OR" : "AND"; // Search condition
                    if (fldOpr.StartsWith("NOT ") || fldOpr == "<>") // Negate for NOT search
                        searchCond = searchCond == "AND" ? "OR" : "AND";
                    foreach (string value in values) {
                        string val = value.Trim();
                        if (!IsMultiSearchOperator(fldOpr)) {
                            sql = SearchFilter(fld.Expression, fldOpr, val, fldDataType, dbid);
                        } else if (dbType == "MYSQL" && (new[] { "=", "<>" }).Contains(fldOpr)) { // Use FIND_IN_SET() for MySQL
                            sql = MultiValueFilter(fld.Expression, val, dbid, fldOpr);
                        } else { // Build multi search SQL
                            sql = MultiSearchSqlFilter(fld.Expression, fldOpr, val, dbid, Config.MultipleOptionSeparator.ToString());
                        }
                        AddFilter(ref wrk, sql, searchCond);
                    }
                }
            }
        }
        return wrk;
    }

    // Multi value search operator
    public static bool IsMultiSearchOperator(string opr) =>
        (new[] { "=", "<>", "LIKE", "NOT LIKE", "STARTS WITH", "ENDS WITH" }).Contains(opr);

    /// <summary>
    /// Get multi search SQL filter
    /// </summary>
    /// <param name="fldExpression">Field expression</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fldVal">Converted search value</param>
    /// <param name="dbid">Database ID</param>
    /// <param name="sep">Separator, e.g. Config.MultipleOptionSeparator</param>
    /// <returns>WHERE clause (fld = val OR fld LIKE val,% OR fld LIKE %,val,% OR fld LIKE %,val)</returns>
    protected static string MultiSearchSqlFilter(string fldExpression, string fldOpr, string fldVal, string dbid, string sep)
    {
        string opr = "=";
        string cond = "OR";
        string likeOpr = "LIKE";
        if (fldOpr.StartsWith("NOT ") || fldOpr == "<>") {
            opr = "<>";
            cond = "AND";
            likeOpr = "NOT LIKE";
        }
        string sql = fldExpression + " " + opr + " '" + AdjustSql(fldVal, dbid) + "' " + cond + " ";
        sql += fldExpression + LikeOrNotLike(likeOpr, QuotedValue(fldVal + sep, DataType.String, dbid, "STARTS WITH"), dbid) + " " + cond + " " +
               fldExpression + LikeOrNotLike(likeOpr, QuotedValue(sep + fldVal + sep, DataType.String, dbid, "LIKE"), dbid) + " " + cond + " " +
               fldExpression + LikeOrNotLike(likeOpr, QuotedValue(sep + fldVal, DataType.String, dbid, "ENDS WITH"), dbid);
        return sql;
    }

    // Check if float type
    public static bool IsFloatType(int fldType) => (fldType == 4 || fldType == 5 || fldType == 131 || fldType == 6);

    /// <summary>
    /// Get search SQL
    /// </summary>
    /// <param name="fld">Field object</param>
    /// <param name="fldVal">Search value</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fldCond">Search condition</param>
    /// <param name="fldVal2">Search value 2</param>
    /// <param name="fldOpr2">Search operator 2</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Search SQL (WHERE clause)</returns>
    public static string GetSearchSql(DbField fld, string fldVal, string fldOpr, string fldCond, string fldVal2, string fldOpr2, string dbid = "DB")
    {
        string sql = "";
        bool isVirtual = fld.VirtualSearch;
        string fldExpression = isVirtual ? fld.VirtualExpression : fld.Expression;
        DataType fldDataType = isVirtual ? DataType.String : fld.DataType;
        if ((new [] { "BETWEEN", "NOT BETWEEN" }).Contains(fldOpr)) {
            var isValidValue = fldDataType != DataType.Number || IsNumeric(fldVal) && IsNumeric(fldVal2);
            if (!Empty(fldVal) && !Empty(fldVal2) && isValidValue)
                sql = fldExpression + " " + fldOpr + " " + QuotedValue(fldVal, fldDataType, dbid) +
                    " AND " + QuotedValue(fldVal2, fldDataType, dbid);
        } else {
            // Handle first value
            if (!Empty(fldVal) && IsValidOperator(fldOpr) || IsNullOrEmptyOperator(fldOpr)) {
                sql = SearchFilter(fldExpression, fldOpr, fldVal, fldDataType, dbid);
                if (fld.IsBoolean && fldVal == fld.FalseValue && fldOpr == "=")
                    sql = "(" + sql + " OR " + fldExpression + " IS NULL)";
            }
            // Handle second value
            string sql2 = "";
            if (!Empty(fldVal2) && !Empty(fldOpr2) && IsValidOperator(fldOpr2) || IsNullOrEmptyOperator(fldOpr2)) {
                sql2 = SearchFilter(fldExpression, fldOpr2, fldVal2, fldDataType, dbid);
                if (fld.IsBoolean && fldVal2 == fld.FalseValue && fldOpr2 == "=")
                    sql2 = "(" + sql2 + " OR " + fldExpression + " IS NULL)";
            }
            // Combine SQL
            AddFilter(ref sql, sql2, fldCond == "OR" ? "OR" : "AND");
        }
        return sql;
    }

    /// <summary>
    /// Get search filter
    /// </summary>
    /// <param name="fldExpression">Field expression</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fldVal">Search value</param>
    /// <param name="fldType">Field type</param>
    /// <param name="dbid">Database id</param>
    /// <returns>WHERE clause</returns>
    public static string SearchFilter(string fldExpression, string fldOpr, object? fldVal, DataType fldType, string dbid = "DB")
    {
        string filter = fldExpression;
        if (Empty(filter))
            return "";
        if (Empty(fldOpr))
            fldOpr = "=";
        if (Empty(dbid))
            dbid = "DB";
        string val = fldVal is string s ? s : (fldVal is DateTime dt && GetConnectionType(dbid) == "MYSQL" ? dt.ToString("yyyy-MM-dd HH:mm:ss") : ConvertToString(fldVal));
        if ((new[] { "=", "<>", "<", "<=", ">", ">=" }).Contains(fldOpr)) {
            filter += " " + fldOpr + " " + QuotedValue(val, fldType, dbid);
        } else if (fldOpr == "IS NULL" || fldOpr == "IS NOT NULL") {
            filter += " " + fldOpr;
        } else if (fldOpr == "IS EMPTY") {
            filter += " = ''";
        } else if (fldOpr == "IS NOT EMPTY") {
            filter += " <> ''";
        } else if (fldOpr == "FIND_IN_SET" || fldOpr == "NOT FIND_IN_SET") { // MYSQL only
            filter = fldOpr + "(" + fldExpression + ", '" + AdjustSql(val, dbid) + "')";
        } else if (fldOpr == "IN" || fldOpr == "NOT IN") {
            filter += " " + fldOpr + " (" + String.Join(", ", val.Split(Config.InOperatorValueSeparator).Select(v => QuotedValue(v, fldType, dbid))) + ")";
        } else if (fldOpr == "STARTS WITH" || fldOpr == "LIKE" || fldOpr == "ENDS WITH") {
            filter += Like(QuotedValue(val, DataType.String, dbid, fldOpr), dbid);
        } else if (fldOpr == "NOT STARTS WITH" || fldOpr == "NOT LIKE" || fldOpr == "NOT ENDS WITH") {
            filter += NotLike(QuotedValue(val, DataType.String, dbid, fldOpr), dbid);
        } else { // Default is equal
            filter += " = " + QuotedValue(val, fldType, dbid);
        }
        return filter;
    }

    /// <summary>
    /// Convert search operator
    /// </summary>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fld">Field object</param>
    /// <param name="fldVal">Search value</param>
    /// <returns>Search operator</returns>
    public static string ConvertSearchOperator(string fldOpr, DbField fld, object? fldVal)
    {
        if (fld.UseFilter)
            fldOpr = "="; // Use "equal"
        // Convert client operator
        string opr = Config.ClientSearchOperators.FirstOrDefault(clientOpr => clientOpr.Value == fldOpr).Key;
        if (!Empty(opr))
            fldOpr = opr;
        if (!IsValidOperator(fldOpr))
            return "";
        if (SameText(fldVal, Config.NullValue)) { // Null value
            return "IS NULL";
        } else if (SameText(fldVal, Config.NotNullValue)) { // Not Null value
            return "IS NOT NULL";
        } else if (Empty(fldOpr)) { // Not specified, ignore
            return fldOpr;
        } else if (fld.DataType == DataType.Number && !fld.VirtualSearch) { // Numeric value(s)
            if (!IsNumericSearchValue(fldVal, fldOpr, fld) || (new[] { "IS EMPTY", "IS NOT EMPTY" }).Contains(fldOpr)) {
                return ""; // Invalid
            } else if ((new[] { "STARTS WITH", "LIKE", "ENDS WITH" }).Contains(fldOpr)) {
                return "=";
            } else if((new[] { "NOT STARTS WITH", "NOT LIKE", "NOT ENDS WITH" }).Contains(fldOpr)) {
                return "<>";
            }
        } else if ((new[] { "LIKE", "NOT LIKE", "STARTS WITH", "NOT STARTS WITH", "ENDS WITH", "NOT ENDS WITH", "IS EMPTY", "IS NOT EMPTY" }).Contains(fldOpr) &&
          !(new[] { DataType.String, DataType.Memo }).Contains(fld.DataType) &&
          !fld.VirtualSearch) { // String type
            return ""; // Invalid
        }
        return fldOpr;
    }

    /// <summary>
    /// Check if search value is numeric
    /// </summary>
    /// <param name="fldVal">Search value</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fld">Field object</param>
    /// <returns>Search value is numeric</returns>
    public static bool IsNumericSearchValue(object? fldVal, string fldOpr, DbField fld)
    {
        if ((fld.IsMultiSelect || fld.UseFilter) && fldVal is string s1 && s1.Contains(Config.MultipleOptionSeparator)) {
            return s1.Split(Config.MultipleOptionSeparator).All(s => IsNumeric(s));
        } else if ((fldOpr == "IN" || fldOpr == "NOT IN") && fldVal is string s2 && s2.Contains(Config.InOperatorValueSeparator)) {
            return s2.Split(Config.InOperatorValueSeparator).All(s => IsNumeric(s));
        } else if (fldVal is List<object> l) {
           return l.All(o => IsNumeric(o));
        }
        return IsNumeric(fldVal);
    }

    /// <summary>
    /// Check if valid search operator
    /// </summary>
    /// <param name="fldOpr">Search operator</param>
    /// <returns>Search operator is valid</returns>
    public static bool IsValidOperator(string fldOpr)
    {
        return Empty(fldOpr) || Config.ClientSearchOperators.ContainsKey(fldOpr);
    }

    /// <summary>
    /// Check if NULL or EMPTY search operator
    /// </summary>
    /// <param name="fldOpr">Search operator</param>
    /// <returns>Search operator is NULL or EMPTY</returns>
    public static bool IsNullOrEmptyOperator(string fldOpr)
    {
        return (new [] { "IS NULL", "IS NOT NULL", "IS EMPTY", "IS NOT EMPTY" }).Contains(fldOpr);
    }

    /// <summary>
    /// Convert search value(s)
    /// </summary>
    /// <param name="fldVal">Search value</param>
    /// <param name="fldOpr">Search operator</param>
    /// <param name="fld">Field object</param>
    /// <returns>Converted Search value(s)</returns>
    public static string ConvertSearchValue(object? fldVal, string fldOpr, DbField fld)
    {
        string convert(object? val) {
            if (SameText(val, Config.NullValue) || SameText(val, Config.NotNullValue)) {
                return ConvertToString(val);
            } else if (IsFloatType(fld.Type)) {
                return ConvertToFloatString(val);
            } else if (fld.IsBoolean) {
                return !Empty(val) ? (ConvertToBool(val) ? fld.TrueValue : fld.FalseValue) : ConvertToString(val);
            } else if (fld.DataType == DataType.Date || fld.DataType == DataType.Time) {
                return !Empty(val) ? UnformatDateTime(val, fld.FormatPattern) : ConvertToString(val);
            }
            return ConvertToString(val);
        };
        if ((fld.IsMultiSelect || fld.UseFilter) && fldVal is string s1 && s1.Contains(Config.MultipleOptionSeparator)) {
            return String.Join(Config.MultipleOptionSeparator, s1.Split(Config.MultipleOptionSeparator).Select(s => convert(s)));
        } else if((fldOpr == "IN" || fldOpr == "NOT IN") && fldVal is string s2 && s2.Contains(Config.InOperatorValueSeparator)) {
            return String.Join(Config.InOperatorValueSeparator, s2.Split(Config.InOperatorValueSeparator).Select(s => convert(s)));
        }
        return convert(fldVal);
    }

    /// <summary>
    /// Get quoted table/field name
    /// </summary>
    /// <param name="name">Table/Field name</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Quoted name</returns>
    public static string QuotedName(string name, string dbid = "DB") =>
        Db(dbid) is var db && db["qs"] is string qs && db["qe"] is string qe ? qs + name.Replace(qe, qe + qe) + qe : name;

    /// <summary>
    /// Get quoted field value
    /// </summary>
    /// <param name="value">Field value</param>
    /// <param name="fldType">Field type</param>
    /// <param name="dbid">Database ID</param>
    /// <param name="likeOpr">Like operator (LIKE, STARTS WITH, ENDS WITH)</param>
    /// <returns>Quoted value</returns>
    public static string QuotedValue(object? value, DataType fldType, string dbid = "DB", string likeOpr = "")
    {
        string dbtype = GetConnectionType(dbid);
        switch (fldType) {
            case DataType.String:
            case DataType.Memo:
                string val = RemoveXss(value);
                if (likeOpr.EndsWith("STARTS WITH"))
                    val = "'" + AdjustSqlForLike(val, dbid) + "%'";
                else if (likeOpr.EndsWith("ENDS WITH"))
                    val = "'%" + AdjustSqlForLike(val, dbid) + "'";
                else if (likeOpr.EndsWith("LIKE"))
                    val = "'%" + AdjustSqlForLike(val, dbid) + "%'";
                else
                    val = "'" + AdjustSql(val, dbid) + "'";
                return dbtype == "MSSQL" ? "N" + val : val;
            case DataType.Guid:
                if (dbtype == "ACCESS") {
                    if (ConvertToString(value).StartsWith("{")) {
                        return ConvertToString(value);
                    } else {
                        return "{" + AdjustSql(value, dbid) + "}";
                    }
                } else {
                    return "'" + AdjustSql(value, dbid) + "'";
                }
            case DataType.Date:
            case DataType.Time:
                if (dbtype == "ACCESS") {
                    return "#" + AdjustSql(value, dbid) + "#";
                } else if (dbtype == "ORACLE") {
                    return "TO_DATE('" + AdjustSql(value, dbid) + "', 'YYYY/MM/DD HH24:MI:SS')";
                } else {
                    return "'" + AdjustSql(value, dbid) + "'";
                }
            case DataType.Boolean:
                if (dbtype == "MYSQL" || dbtype == "ORACLE") { // ENUM('Y','N'), ENUM('y','n'), ENUM('1'/'0')
                    return "'" + AdjustSql(value, dbid) + "'";
                } else if (dbtype == "MSSQL") { // Boolean (MSSQL)
                    return ConvertToBool(value) ? "1" : "0";
                } else { // Boolean
                    return ConvertToString(value);
                }
            case DataType.Bit: // dbtype == "MYSQL" || dbtype == "POSTGRESQL"
                return "b'" + value + "'";
            case DataType.Number:
                if (IsNumeric(value))
                    return ConvertToString(value);
                else
                    return "null"; // Treat as null
            default:
                return ConvertToString(value);
        }
    }

    // Pad zeros before number
    public static string ZeroPad(object m, int t) => ConvertToString(m).PadLeft(t, '0');

    /// <summary>
    /// Cast date/time field for LIKE
    /// </summary>
    /// <param name="fld">Field expression</param>
    /// <param name="namedformat">Date format</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>SQL expression formatting the field to 'y-MM-dd HH:mm:ss'</returns>
    public static string CastDateFieldForLike(string fld, int namedformat, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        string dateFormat = DbDateFormat(namedformat, dbtype);
        if (!Empty(dateFormat)) {
            return dbtype switch {
                "MYSQL" => "DATE_FORMAT(" + fld + ", '" + dateFormat + "')",
                "MSSQL" => "FORMAT(" + fld + ", '" + dateFormat + "')",
                "ORACLE" => "TO_CHAR(" + fld + ", '" + dateFormat + "')",
                "POSTGRESQL" => "TO_CHAR(" + fld + ", '" + dateFormat + "')",
                "SQLITE" => "STRFTIME('" + dateFormat + "', " + fld + ")",
                _ => fld
            };
        }
        return fld;
    }

    // Append like operator
    public static string Like(string pat, string dbid = "DB") => LikeOrNotLike("LIKE", pat, dbid);

    // Append not like operator
    public static string NotLike(string pat, string dbid = "DB") => LikeOrNotLike("NOT LIKE", pat, dbid);

    /// <summary>
    /// Convert object to numeric value
    /// </summary>
    /// <param name="opr">Operator (LIKE/NOT LIKE)</param>
    /// <param name="pat">Pattern</param>
    /// <param name="dbid">Database Id</param>
    /// <returns>LIKE / NOT LIKE operator</returns>
    public static string LikeOrNotLike(string opr, string pat, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        opr = " " + opr + " ";
        if (dbtype == "POSTGRESQL" && Config.UseIlikeForPostgresql)
            return opr.Replace(" LIKE ", " ILIKE ") + pat;
        else if (dbtype == "MYSQL" && !Empty(Config.LikeCollationForMysql))
            return opr + pat + " COLLATE " + Config.LikeCollationForMysql;
        else if (dbtype == "MSSQL" && !Empty(Config.LikeCollationForMssql))
            return " COLLATE " + Config.LikeCollationForMssql + opr + pat;
        else if (dbtype == "SQLITE" && !Empty(Config.LikeCollationForSqlite))
            return opr + pat + " COLLATE " + Config.LikeCollationForSqlite;
        return opr + pat;
    }

    /// <summary>
    /// Get script name
    /// </summary>
    /// <returns>Current script name</returns>
    public static string ScriptName() => Request?.Path ?? "";

    /// <summary>
    /// Convert object to numeric value
    /// </summary>
    /// <param name="v">Value to be converted</param>
    /// <param name="t">ADO Data type</param>
    /// <returns>Numeric value</returns>
    public static object ConvertType(object v, int t) =>
        IsNull(v) // DN
            ? DbNullValue
            : t switch {
                20 => Convert.ToInt64(v, CultureInfo.CreateSpecificCulture("en-US")), // adBigInt
                21 => Convert.ToUInt64(v, CultureInfo.CreateSpecificCulture("en-US")), // adUnsignedBigInt
                2 or 16 => Convert.ToInt16(v, CultureInfo.CreateSpecificCulture("en-US")), // adSmallInt/adTinyInt
                3 => Convert.ToInt32(v, CultureInfo.CreateSpecificCulture("en-US")), // adInteger
                17 or 18 => Convert.ToUInt16(v, CultureInfo.CreateSpecificCulture("en-US")), // adUnsignedTinyInt/adUnsignedSmallInt
                19 => Convert.ToUInt32(v, CultureInfo.CreateSpecificCulture("en-US")), // adUnsignedInt
                4 => Convert.ToSingle(v, CultureInfo.CreateSpecificCulture("en-US")), // adSingle
                5 or 6 or 131 or 139 => Convert.ToDouble(v, CultureInfo.CreateSpecificCulture("en-US")), // adDouble/adCurrency/adNumeric/adVarNumeric
                14 => Convert.ToDecimal(v, CultureInfo.CreateSpecificCulture("en-US")), // adDecimal
                _ => v
            };

    /// <summary>
    /// Concatenate string
    /// </summary>
    /// <param name="str1">String 1</param>
    /// <param name="str2">String 2</param>
    /// <param name="sep">Separator</param>
    /// <returns>Concatenated string</returns>
    public static string Concatenate(string str1, string str2, string sep)
    {
        str1 = str1?.Trim() ?? "";
        str2 = str2?.Trim() ?? "";
        if (str2 == "")
            return str1;
        if (str1 != "" && sep != "" && !str1.EndsWith(sep))
            str1 += sep;
        return str1 + str2;
    }

    /// <summary>
    /// Calculate elapsed time
    /// </summary>
    /// <returns>Time in seconds</returns>
    public static string GetElapsedTime() => ((double)(Environment.TickCount - StartTime) / 1000).ToString("F3");

    /// <summary>
    /// Show elapsed time // DN
    /// </summary>
    /// <returns></returns>
    public static IHtmlContent ElapsedTime()
    {
        string str = "";
        if (Config.Debug)
            str = "<p class=\"ew-timer text-info\">Page processing time: " + GetElapsedTime() + " seconds</p>";
        return new HtmlString(str);
    }

    // Compare values with special handling for null values (Treat DBNull as null) // DN
    public static bool CompareValue(object? v1, object? v2)
    {
        if (IsNull(v1) && IsNull(v2)) {
            return true;
        } else if (IsNull(v1) || IsNull(v2)) {
            return false;
        }
        return SameString(v1, v2);
    }

    /// <summary>
    /// Adjust value for SQL based on dbid
    /// </summary>
    /// <param name="value">Value to adjust</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Adjusted value</returns>
    public static string AdjustSql(object? value, string dbid = "DB") =>
        ConvertToString(value).Trim().Replace("'", GetConnectionType(dbid) == "MYSQL" ? "\\'" : "''"); // Adjust for single quote

    /// <summary>
    /// Adjust value for SQL Like operator based on dbid // test for "%", "_" characters
    /// </summary>
    /// <param name="value">Value to adjust</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Adjusted value</returns>
    public static string AdjustSqlForLike(object? value, string dbid = "DB") =>
        AdjustSql(ConvertToString(value).Replace("%", @"\%").Replace("_", @"\_"), dbid);

    // Adjust GUID for MS Access // DN
    public static string AdjustGuid(object? value, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        var str = ConvertToString(value).Trim();
        if (dbtype == "ACCESS" && !str.StartsWith("{"))
            str = "{" + str + "}"; // Add curly braces
        return str;
    }

    // Build SELECT SQL based on different SQL part
    public static string BuildSelectSql(string select, string where, string groupBy, string having, string orderBy, string filter, string sort)
    {
        string dbWhere = where;
        AddFilter(ref dbWhere, filter);
        string dbOrderBy = orderBy;
        if (!Empty(sort))
            dbOrderBy = sort;
        string sql = select;
        if (!Empty(dbWhere))
            sql += " WHERE " + dbWhere;
        if (!Empty(groupBy))
            sql += " GROUP BY " + groupBy;
        if (!Empty(having))
            sql += " HAVING " + having;
        if (!Empty(dbOrderBy))
            sql += " ORDER BY " + dbOrderBy;
        return sql;
    }

    // Load a UTF-8 encoded text file (relative to wwwroot)
    public static async Task<string> LoadText(string fn)
    {
        if (!Empty(fn)) {
            fn = MapPath(fn);
            return FileExists(fn) ? await FileReadAllText(fn) : "";
        }
        return "";
    }

    // Write audit trail (insert/update/delete)
    public static async Task WriteAuditTrailAsync(string pfx, string dt, string scrpt, string user, string action, string table, string field, object? keyvalue, object? oldvalue, object? newvalue)
    {
        if (table == Config.AuditTrailTableName)
            return;
        try {
            string usrwrk = !Empty(user) ? user : (IsLoggedIn() ? Language.Phrase("UserAdministrator") : Language.Phrase("UserAnonymous")); // assume Administrator (logged in) / Anonymous user (not logged in) if no user
            Dictionary<string, object> rsnew;
            if (Config.AuditTrailToDatabase) {
                rsnew = new () {
                    { Config.AuditTrailFieldNameDateTime, dt },
                    { Config.AuditTrailFieldNameScript, scrpt },
                    { Config.AuditTrailFieldNameUser, usrwrk },
                    { Config.AuditTrailFieldNameAction, action },
                    { Config.AuditTrailFieldNameTable, table },
                    { Config.AuditTrailFieldNameField, field },
                    { Config.AuditTrailFieldNameKeyvalue, IsNull(keyvalue) ? "" : keyvalue },
                    { Config.AuditTrailFieldNameOldvalue, IsNull(oldvalue) ? "" : oldvalue },
                    { Config.AuditTrailFieldNameNewvalue, IsNull(newvalue) ? "" : newvalue }
                 };
             } else {
                rsnew = new () {
                    { "datetime", dt },
                    { "script", scrpt },
                    { "user", usrwrk },
                    { "action", action },
                    { "table", table },
                    { "field", field },
                    { "keyvalue", keyvalue ?? "" },
                    { "oldvalue", oldvalue ?? "" },
                    { "newvalue", newvalue ?? "" }
                };
            }

            // Call AuditTrail Inserting event
            bool writeAuditTrail = AuditTrailInserting(rsnew);
            if (writeAuditTrail) {
                if (!Config.AuditTrailToDatabase) { // Write audit trail to log file
                    string folder = ServerMapPath(Config.AuditTrailPath);
                    string file = folder + pfx + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    if (CreateFolder(folder)) {
                        bool writeHeader = !FileExists(file);
                        using var sw = FileAppendText(file);
                        if (writeHeader)
                            sw.WriteLine(String.Join("\t", rsnew.Keys));
                        sw.WriteLine(String.Join("\t", rsnew.Select(kvp => ConvertToString(kvp.Value))));
                    }
                } else if (!Empty(Config.AuditTrailTableName)) { // DN
                    var tbl = Resolve(Config.AuditTrailTableName);
                    if (tbl != null) {
                        if (tbl.Fields.TryGetValue(Config.AuditTrailFieldNameDateTime, out DbField? fld)) // Set date // DN
                            fld?.SetDbValue(rsnew, dt);
                        if ((bool)tbl.Invoke("RowInserting", new object?[] { null, rsnew })) {
                            if (await tbl.InsertAsync(rsnew) > 0)
                                tbl.Invoke("RowInserted", new object?[] { null, rsnew });
                        }
                    }
                }
            }
        } catch {
            if (Config.Debug)
                throw;
        }
    }

    // Write audit trail (insert/update/delete)
    public static async Task WriteAuditTrailAsync(string user, string action, string table = "", string field = "", object? keyvalue = null, object? oldvalue = null, object? newvalue = null) =>
        await WriteAuditTrailAsync("log", DbCurrentDateTime(), ScriptName(), user, action, table, field, keyvalue, oldvalue, newvalue);

    // Write audit trail (insert/update/delete)
    public static void WriteAuditTrail(string pfx, string dt, string scrpt, string user, string action, string table, string field, object? keyvalue, object? oldvalue, object? newvalue) =>
        WriteAuditTrailAsync(pfx, dt, scrpt, user, action, table, field, keyvalue, oldvalue, newvalue).GetAwaiter().GetResult();

    // Write audit trail (insert/update/delete)
    public static void WriteAuditTrail(string user, string action, string table = "", string field = "", object? keyvalue = null, object? oldvalue = null, object? newvalue = null) =>
        WriteAuditTrailAsync(user, action, table, field, keyvalue, oldvalue, newvalue).GetAwaiter().GetResult();

    /// <summary>
    /// Write export log
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="dt">DateTime</param>
    /// <param name="user">User ID or user name</param>
    /// <param name="exportType">Export type</param>
    /// <param name="table">Table</param>
    /// <param name="keyValue">Key value</param>
    /// <param name="fileName">File name</param>
    /// <param name="req">Request</param>
    /// <returns></returns>
    public static async Task WriteExportLog(string fileId, string dt, string user, string exportType, string table, string keyValue, string fileName, string req)
    {
        if (Empty(Config.ExportLogTableVar))
            return;
        Dictionary<string, object> rsnew = new()
        {
            { Config.ExportLogFieldNameFileId, fileId },
            { Config.ExportLogFieldNameDateTime, dt },
            { Config.ExportLogFieldNameUser, user },
            { Config.ExportLogFieldNameExportType, exportType },
            { Config.ExportLogFieldNameTable, table },
            { Config.ExportLogFieldNameKeyValue, keyValue },
            { Config.ExportLogFieldNameFileName, fileName },
            { Config.ExportLogFieldNameRequest, req }
        };
        if (Config.Debug)
            Log("Export: " + ConvertToJson(rsnew));
        var tbl = Resolve(Config.ExportLogTableVar);
        if (tbl != null)
        {
            if (tbl.Fields.TryGetValue(Config.ExportLogFieldNameDateTime, out DbField? fld)) // Set date // DN
                fld?.SetDbValue(rsnew, dt);
            if ((bool)tbl.Invoke("RowInserting", new object?[] { null, rsnew }))
            {
                if (await tbl.InsertAsync(rsnew) > 0)
                    tbl.Invoke("RowInserted", new object?[] { null, rsnew });
            }
        }
    }

    /// <summary>
    /// Export path
    /// </summary>
    /// <param name="phyPath">Physical path</param>
    /// <returns>Export path</returns>
    public static string ExportPath(bool phyPath = false)
    {
        return phyPath
            ? IncludeTrailingDelimiter(UploadPath(true) + Config.ExportPath, true)
            : IncludeTrailingDelimiter(FullUrl(UploadPath(false) + Config.ExportPath), false); // Full URL
    }

    /// <summary>
    /// Unformat date time
    /// </summary>
    /// <param name="value">Date/Time string</param>
    /// <param name="dateTimeFormat">Formatter pattern</param>
    /// <param name="dateTimeStyles">DateTime style</param>
    /// <returns></returns>
    public static string UnformatDateTime(object? value, string dateTimeFormat = "", DateTimeStyles dateTimeStyles = DateTimeStyles.None)
    {
        string s = ConvertToString(value).Trim();
        if (Empty(s) ||
            Regex.IsMatch(s, @"^([0-9]{4})-([0][1-9]|[1][0-2])-([0][1-9]|[1|2][0-9]|[3][0|1])( (0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:([0-5][0-9]))?)?") || // Date/Time
            Regex.IsMatch(s, @"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:([0-5][0-9]))?")) // Time
            return s;
        string df = DateFormat(dateTimeFormat);
        DateTime? dt = ParseDateTime(s, df, dateTimeStyles);
        if (dt.HasValue) {
            if (df.Contains("y", StringComparison.InvariantCultureIgnoreCase) && df.Contains("h", StringComparison.InvariantCultureIgnoreCase)) // Date/Time
                return dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            else if (df.Contains("y", StringComparison.InvariantCultureIgnoreCase)) // Date
                return dt.Value.ToString("yyyy-MM-dd");
            else if (df.Contains("h", StringComparison.InvariantCultureIgnoreCase)) // Time
                return dt.Value.ToString("HH:mm:ss");
        }
        return s;
    }

    /// <summary>
    /// Format a timestamp, datetime, date or time field
    /// </summary>
    /// <param name="ts">Timestamp or Date/Time string</param>
    /// <param name="dateTimeFormat">Formatter pattern without Right-to-Left Mark (RLM), or an integer (predefined format)</param>
    /// <param name="locale">Locale ID</param>
    /// <returns>Formatted date/time</returns>
    public static string FormatDateTime(object? ts, object dateTimeFormat, string? locale = null)
    {
        DateTime? dt = null;
        string df;
        if (ts is DateTime d)
            dt = d;
        else if (ts is DateTimeOffset dto)
            dt = dto.DateTime;
        else if (ts is TimeSpan t)
            dt = ParseDateTime(t.ToString());
        else if (IsNumeric(ts)) // Timestamp
            dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(ts)).ToLocalTime();
        else if (ts is string s && !Empty(ts))
            dt = ParseDateTime(s.Trim());
        if (dt.HasValue) {
            df = dateTimeFormat is int i && i == 8 // Handle edit format (show time part only if exists)
                ? ConvertToInt(dt.Value.ToString("HHmmss")) == 0 ? DateFormat(0) : DateFormat(1)
                : DateFormat(dateTimeFormat);
            df = df.Replace("\u200F", ""); // Remove Right-to-Left Mark (RLM)
            df = Regex.Replace(df, @"a(?<=(\b|H|h|m|s))\b", "tt"); // Convert ICU to .NET format, "a" => "tt"
            return Empty(locale)
                ? Format(dt.Value.ToString(df, CurrentDateTimeFormat)) // Empty locale => format by CurrentDateTimeFormat
                : CultureInfo.CreateSpecificCulture(locale) is var ci
                ? Format(dt.Value.ToString(df, ci)) // Specified locale => format by DateTimeFormat
                : ConvertToString(ts);
        }
        return ConvertToString(ts);
        // Local function to format quarter and convert digits
        string Format(string str) => Regex.Replace(str, "(?i)q+", new MatchEvaluator((Match m) => m.Groups[0].Value.ToLower() switch {
            "qq" => dt.Value.Quarter().ToString().PadLeft(2, '0'), // e.g. 02
            "qqq" => Language.Phrase("QuarterShort").Replace("%q", dt.Value.Quarter().ToString()), // e.g. Q2
            "qqqq" => Language.Phrase("Quarter").Replace("%q", dt.Value.Quarter().ToString()), // e.g. Quarter 2
            _ => dt.Value.Quarter().ToString(), // e.g. 2
        })).Replace(LatinDigits, NativeDigits);
    }

    /// <summary>
    /// Parse datetime
    /// </summary>
    /// <param name="value">Date/Time string</param>
    /// <param name="dateTimeFormat">Formatter pattern</param>
    /// <param name="dateTimeStyles">DateTime style</param>
    /// <returns></returns>
    public static DateTime? ParseDateTime(string? value, string dateTimeFormat = "", DateTimeStyles dateTimeStyles = DateTimeStyles.None)
    {
        if (Empty(value))
            return null;
        value = value.Replace(NativeDigits, LatinDigits);
        if (!Empty(dateTimeFormat)) {
            dateTimeFormat = Regex.Replace(dateTimeFormat, @"\ba\b", "tt"); // Convert ICU to .NET format, "a" => "tt"
            if (DateTime.TryParseExact(value, dateTimeFormat, CurrentDateTimeFormat, dateTimeStyles, out DateTime dt))
                return dt;
        }
        if (DateTime.TryParse(value, CurrentDateTimeFormat, dateTimeStyles, out DateTime dt2))
            return dt2;
        else if (DateTime.TryParse(value, out DateTime dt3))
            return dt3;
        return null;
    }

    /// <summary>
    /// Parse datetime offset
    /// </summary>
    /// <param name="value">Date/Time string</param>
    /// <param name="dateTimeFormat">Formatter pattern</param>
    /// <param name="dateTimeStyles">DateTime style</param>
    /// <returns></returns>
    public static DateTimeOffset? ParseDateTimeOffset(string? value, string dateTimeFormat = "", DateTimeStyles dateTimeStyles = DateTimeStyles.None)
    {
        if (Empty(value))
            return null;
        value = value.Replace(NativeDigits, LatinDigits);
        if (!Empty(dateTimeFormat)) {
            dateTimeFormat = Regex.Replace(dateTimeFormat, @"\ba\b", "tt"); // Convert ICU to .NET format, "a" => "tt"
            if (DateTimeOffset.TryParseExact(value, dateTimeFormat, CurrentDateTimeFormat, dateTimeStyles, out DateTimeOffset dt))
                return dt;
        }
        if (DateTimeOffset.TryParse(value, CurrentDateTimeFormat, dateTimeStyles, out DateTimeOffset dt2))
            return dt2;
        else if (DateTimeOffset.TryParse(value, out DateTimeOffset dt3))
            return dt3;
        return null;
    }

    /// <summary>
    /// Parse time span
    /// </summary>
    /// <param name="value">Time string</param>
    /// <param name="timeFormat">Formatter pattern</param>
    /// <param name="timeSpanStyles">TimeSpan style</param>
    /// <returns></returns>
    public static TimeSpan? ParseTimeSpan(string? value, string timeFormat = "", TimeSpanStyles timeSpanStyles = TimeSpanStyles.None)
    {
        if (Empty(value))
            return null;
        value = value.Replace(NativeDigits, LatinDigits);
        if (!Empty(timeFormat)) {
            timeFormat = Regex.Replace(timeFormat, @"\ba\b", "tt"); // Convert ICU to .NET format, "a" => "tt"
            if (TimeSpan.TryParseExact(value, timeFormat, CurrentDateTimeFormat, timeSpanStyles, out TimeSpan ts))
                return ts;
            else if (TimeSpan.TryParseExact(value, timeFormat, CurrentDateTimeFormat, out TimeSpan ts2))
                return ts2;
        }
        if (TimeSpan.TryParse(value, CurrentDateTimeFormat, out TimeSpan ts3))
            return ts3;
        else if (TimeSpan.TryParse(value, out TimeSpan ts4))
            return ts4;
        return null;
    }

    /// <summary>
    /// Is formatted
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static bool IsFormatted(object? value)
    {
        if (value == null || value is string s && s == "" || value is decimal || value.GetType().IsPrimitive) // Number, not formatted
            return false;
        string str = ConvertToString(value);
        if (!Regex.IsMatch(str, @"^[\+\-]?[0-9]*\.?[Ee]?[\+\-]?[0-9]*$")) // Contains non-numeric characters, assume formatted
            return true;
        if (CurrentNumberFormat.NumberGroupSeparator == "." && str.Contains(".")) { // Contains one ".", e.g. 123.456
            if (ParseInteger(str).HasValue) // Can be parsed, assume "." is grouping separator (not always true)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Latin native digits
    /// </summary>
    /// <value></value>
    public static string[] LatinDigits = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    /// <summary>
    /// Native digits of current culture
    /// </summary>
    public static string[] NativeDigits => NumberingSystem == "latn" ? LatinDigits : CurrentNumberFormat.NativeDigits;

    /// <summary>
    /// Format number
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="pattern">Numeric format string. If empty, use current number format. If null, keep all decimal digits.</param>
    /// <returns></returns>
    public static string FormatNumber(object? value, string? pattern = null)
    {
        if (Empty(value))
            return "";
        string formatPattern = !Empty(pattern) ? pattern : NumberFormat;
        string? n = null;
        string? g = null;
        if (value is decimal dValue) { // Convert decimal value directly (Convert to double may cause precision problem)
            n = dValue.ToString(formatPattern, CurrentNumberFormat); // Number format
            g = dValue.ToString("G", CurrentNumberFormat); // General
        } else if (value is Single sValue) { // Convert single value directly (Convert to double may cause precision problem)
            n = sValue.ToString(formatPattern, CurrentNumberFormat); // Number format
            g = sValue.ToString("G", CurrentNumberFormat); // General
        } else {
            double? val = IsFormatted(value) ? ParseNumber(ConvertToString(value)) : ConvertToNullableDouble(value);
            if (val.HasValue) {
                n = val.Value.ToString(formatPattern, CurrentNumberFormat); // Number format
                g = val.Value.ToString("G", CurrentNumberFormat); // General
            }
        }
        if (n != null && g != null) {
            string s = pattern == null && n.Contains(CurrentNumberFormat.NumberDecimalSeparator) && g.Contains(CurrentNumberFormat.NumberDecimalSeparator)
                ? n.Split(CurrentNumberFormat.NumberDecimalSeparator)[0] + CurrentNumberFormat.NumberDecimalSeparator + g.Split(CurrentNumberFormat.NumberDecimalSeparator)[1]
                : n;
            return s.Replace(LatinDigits, NativeDigits);
        }
        return ConvertToString(value);
    }

    /// <summary>
    /// Format number
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="numberDecimalDigits">Number of decimal digits</param>
    /// <returns></returns>
    public static string FormatNumber(object? value, int numberDecimalDigits) =>
        FormatNumber(value, Regex.Replace(NumberFormat, @"\.[#0]*", ".".PadRight(numberDecimalDigits + 1, '#')));

    /// <summary>
    /// Format integer
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="pattern">Numeric format string</param>
    /// <returns></returns>
    public static string FormatInteger(object? value, string? pattern = null)
    {
        if (Empty(value))
            return "";
        long? val = IsFormatted(value) ? ParseInteger(ConvertToString(value)) : ConvertToNullableInt64(value);
        pattern = !Empty(pattern) ? pattern : NumberFormat;
        pattern = pattern.Split(".")[0]; // Remove decimal places in the pattern
        return val.HasValue ? val.Value.ToString(pattern, CurrentNumberFormat).Replace(LatinDigits, NativeDigits) : ConvertToString(value);
    }

    /// <summary>
    /// Parse number
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static double? ParseNumber(string? value)
    {
        if (Empty(value))
            return null;
        else if (value.Contains(CurrentNumberFormat.PercentSymbol))
            return ParsePercent(value);
        else if (value.Contains(CurrentNumberFormat.CurrencySymbol))
            return ParseCurrency(value);
        return double.TryParse(value.Replace(NativeDigits, LatinDigits), NumberStyles.Number, CurrentNumberFormat, out double v) ? v : null;
    }

    /// <summary>
    /// Parse integer
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static long? ParseInteger(string? value) =>
        !Empty(value) && long.TryParse(value.Replace(NativeDigits, LatinDigits), NumberStyles.Number, CurrentNumberFormat, out long v) ? v : null;

    /// <summary>
    /// Convert string to float (as string in "en-US")
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static string ConvertToFloatString(object? value)
    {
        if (Empty(value))
            return ConvertToString(value);
        string s = value is string v ? v : ConvertToString(value);
        return (ParseNumber(s) is var result) && result.HasValue
            ? result.Value.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))
            : "";
    }

    /// <summary>
    /// Format currency
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="pattern">Numeric format string</param>
    /// <returns></returns>
    public static string FormatCurrency(object? value, string? pattern = null)
    {
        if (Empty(value))
            return "";
        double? val = IsFormatted(value) ? ParseNumber(ConvertToString(value)) : ConvertToNullableDouble(value);
        pattern = !Empty(pattern) ? pattern : CurrencyFormat;
        return val.HasValue
            ? val.Value.ToString(pattern, CurrentNumberFormat).Replace("¤", CurrentNumberFormat.CurrencySymbol).Replace(LatinDigits, NativeDigits) // Currency
            : ConvertToString(value);
    }

    /// <summary>
    /// Format currency
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="numberDecimalDigits">Number of decimal digits</param>
    /// <returns></returns>
    public static string FormatCurrency(object? value, int numberDecimalDigits) =>
        FormatCurrency(value, Regex.Replace(CurrencyFormat, @"\.[#0]*", ".".PadRight(numberDecimalDigits + 1, '#')));

    /// <summary>
    /// Parse currency
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static double? ParseCurrency(string? value) =>
        !Empty(value) && double.TryParse(value.Replace(NativeDigits, LatinDigits), NumberStyles.Currency, CurrentNumberFormat, out double v) ? v : null;

    /// <summary>
    /// Format percent
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="pattern">Numeric format string</param>
    /// <returns></returns>
    public static string FormatPercent(object? value, string? pattern = null)
    {
        if (Empty(value))
            return "";
        double? val = IsFormatted(value) ? ParseNumber(ConvertToString(value)) : ConvertToNullableDouble(value);
        pattern = !Empty(pattern) ? pattern : PercentFormat;
        return val.HasValue
            ? val.Value.ToString(pattern, CurrentNumberFormat).Replace(LatinDigits, NativeDigits) // Percent
            : ConvertToString(value);
    }

    /// <summary>
    /// Format currency
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="numberDecimalDigits">Number of decimal digits</param>
    /// <returns></returns>
    public static string FormatPercent(object? value, int numberDecimalDigits) =>
        FormatPercent(value, Regex.Replace(CurrencyFormat, @"\.[#0]*", ".".PadRight(numberDecimalDigits + 1, '#')));

    /// <summary>
    /// Parse percent
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns></returns>
    public static double? ParsePercent(string? value)
    {
        if (value?.Contains(CurrentNumberFormat.PercentSymbol) ?? false) {
            double? val = ParseNumber(value.Replace(CurrentNumberFormat.PercentSymbol, ""));
            return val.HasValue ? val.Value / 100 : null;
        }
        return null;
    }

    /// <summary>
    /// Add SCRIPT tag (async) by script
    /// </summary>
    /// <param name="src">Path of script</param>
    /// <param name="id">ID (for loadjs)</param>
    /// <param name="options">Options (async and numRetries), see https://github.com/muicss/loadjs</param>
    public static void AddClientScript(string src, string id = "", dynamic? options = null) => LoadJs(src, id, options);

    /// <summary>
    /// Add LINK tag by script
    /// </summary>
    /// <param name="href">Path of stylesheet</param>
    /// <param name="id">ID (for loadjs)</param>
    public static void AddStylesheet(string href, string id = "") => LoadJs("css!" + href, id);

    /// <summary>
    /// Load JavaScript or Stylesheet by loadjs
    /// </summary>
    /// <param name="src">Path of script/stylesheet</param>
    /// <param name="id">ID (for loadjs)</param>
    /// <param name="options">Options (async and numRetries), see https://github.com/muicss/loadjs</param>
    public static void LoadJs(string src, string id = "", dynamic? options = null)
    {
        var m = Regex.Match(src, @"^css!", RegexOptions.IgnoreCase);
        if (m.Success)
            src = Regex.Replace(src, @"^css!", "");
        Write("<script>loadjs(\"" + (m.Success ? "css!" : "") + AppPath(src) + "\"" + (!Empty(id) ? ", \"" + id + "\"" : "") + ((options != null) ? ", " + ConvertToJson(options) : "") + ");</script>");
    }

    // Is Boolean attribute
    public static bool IsBooleanAttribute(string attr) => Config.BooleanHtmlAttributes.Contains(attr, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Build HTML element
    /// </summary>
    /// <param name="tagname">HTML tag name</param>
    /// <param name="attrs">Attributes</param>
    /// <param name="innerhtml">Inner HTML</param>
    /// <returns></returns>
    public static string HtmlElement(string tagname, object? attrs = null, string innerhtml = "")
    {
        tagname = tagname.ToLower();
        Attributes atts = attrs != null ? new (attrs) : new ();
        string html = "<" + tagname + atts.ToString() + ">";
        if (!Config.HtmlSingletonTags.Contains(tagname)) { // Not singleton
            if (innerhtml != "")
                html += innerhtml;
            html += "</" + tagname + ">";
        }
        return html;
    }

    /// <summary>
    /// Get HTML &lt;a&gt; tag
    /// </summary>
    /// <param name="attrs">The href attribute, or attributes.</param>
    /// <param name="phraseId">Phrase ID for inner HTML</param>
    /// <returns></returns>
    public static string GetLinkHtml(object? attrs, string phraseId)
    {
        if (attrs is string str)
            attrs = new { href = str };
        Attributes atts = attrs != null ? new (attrs) : new ();
        if (atts.ContainsKey("onclick") && !atts.ContainsKey("href"))
            atts["href"] = "#";
        string phrase = Language.Phrase(phraseId);
        string title = ConvertToString(atts["title"]);
        if (!atts.ContainsKey(title)) {
            title = Language.Phrase(phraseId, true);
            atts["title"] = title;
        }
        if (!Empty(title) && !atts.ContainsKey("data-caption"))
            atts["data-caption"] = title;
        return HtmlElement("a", atts, phrase);
    }

    /// <summary>
    /// Get HTML markup of an option
    /// </summary>
    /// <param name="val">An option value</param>
    /// <returns>HTML string</returns>
    public static string OptionHtml(string val) => Regex.Replace(Config.OptionHtmlTemplate, "{value}", val);

    /// <summary>
    /// Get HTML markup for options
    /// </summary>
    /// <param name="values">Option values</param>
    /// <returns>HTML markup</returns>
    public static string OptionsHtml(List<string> values) => values.Aggregate("", (html, next) => html + OptionHtml(next));

    /// <summary>
    /// HTML-Encode
    /// </summary>
    /// <param name="expression">Value to encode</param>
    /// <returns>Encoded string</returns>
    public static string HtmlEncode(object? expression) => WebUtility.HtmlEncode(ConvertToString(expression)) ?? "";

    /// <summary>
    /// HTML-Decode
    /// </summary>
    /// <param name="expression">Value to decode</param>
    /// <returns>Decoded string</returns>
    public static string HtmlDecode(object? expression) => WebUtility.HtmlDecode(ConvertToString(expression)) ?? "";

    /// <summary>
    /// URL-Encode
    /// </summary>
    /// <param name="expression">Value to encode</param>
    /// <returns>Encoded value</returns>
    public static string UrlEncode(object? expression) => WebUtility.UrlEncode(ConvertToString(expression)) ?? "";

    /// <summary>
    /// Raw URL-Encode (also replaces "+" as "%20")
    /// </summary>
    /// <param name="expression">Value to encode</param>
    /// <returns>Encoded value</returns>
    public static string RawUrlEncode(object? expression) => WebUtility.UrlEncode(ConvertToString(expression)) is string s ? s.Replace("+", "%20") : "";

    /// <summary>
    /// URL-Decode
    /// </summary>
    /// <param name="expression">Value to decode</param>
    /// <returns>Decoded value</returns>
    public static string UrlDecode(object? expression) => WebUtility.UrlDecode(ConvertToString(expression)) ?? "";

    /// <summary>
    /// Get title
    /// </summary>
    /// <param name="name">HTML string</param>
    /// <returns>Title</returns>
    public static string HtmlTitle(string name)
    {
        List<string> patterns = new () {
            @"<span class=([\'""])visually-hidden\\1>([\s\S]*?)<\/span>",  // Match span.visually-hidden
            @"\s+title\s*=\s*([\'""])([\s\S]*?)\\1", // Match title='title'
            @"\s+data-caption\s*=\s*([\'""])([\s\S]*?)\\1" // Match data-caption='caption'
        };
        Match? m = null;
        foreach (string pattern in patterns) {
            m = Regex.Match(name, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
                return m.Groups[2].Value;
        }
        return name;
    }

    // Format sequence number
    public static string FormatSequenceNumber(object seq) => Language.Phrase("SequenceNumber").Replace("%s", ConvertToString(seq));

    /// <summary>
    /// Format phone number (https://github.com/twcclegg/libphonenumber-csharp)
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="region">Region code</param>
    /// <param name="format">Format</param>
    /// <returns>Phone number in specified format</returns>
    public static string FormatPhoneNumber(string phoneNumber, string? region = null, PhoneNumbers.PhoneNumberFormat format = PhoneNumbers.PhoneNumberFormat.E164)
    {
        region ??= Config.SmsRegionCode;
        if (region == null) // Get region from locale
            region = CurrentLanguage.Replace("_", "-").Split('-').First();
        PhoneNumbers.PhoneNumberUtil phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
        PhoneNumbers.PhoneNumber phoneNumberObject = phoneNumberUtil.Parse(phoneNumber, region);
        return phoneNumberUtil.Format(phoneNumberObject, format);
    }

    /// <summary>
    /// Encode value for double-quoted JavaScript string
    /// </summary>
    /// <param name="val">Value to encode</param>
    /// <returns>Encoded value</returns>
    public static string JsEncode(object val) => ConvertToString(val).Replace("\\", "\\\\")
        .Replace("\"", "\\\"").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

    /// <summary>
    /// Encode value to single-quoted Javascript string for HTML attributes
    /// </summary>
    /// <param name="val">Value to encode</param>
    /// <returns>Encoded value</returns>
    public static string JsEncodeAttribute(object val) => HtmlEncode(val).Replace("\\", "\\\\")
        .Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

    /// <summary>
    /// Get display field value separator
    /// </summary>
    /// <param name="idx">Display field index (1, 2, or 3)</param>
    /// <param name="fld">Field object</param>
    /// <returns>Separator</returns>
    public static string ValueSeparator(int idx, DbField fld)
    {
        object sep = fld?.DisplayValueSeparator ?? ", ";
        return IsList<string>(sep) ? ((List<string>)sep)[idx - 1] : ConvertToString(sep);
    }

    /// <summary>
    /// Delimited values separator (for select-multiple or checkbox)
    /// </summary>
    /// <returns>Method takes the index and returns the separator</returns>
    public static Func<int, string> ViewOptionSeparator { get; set; } = (idx) => ", ";

    /// <summary>
    /// Get temp upload path root
    /// </summary>
    /// <param name="physical">Physical path or not (default is true)</param>
    /// <returns>Temp upload path root</returns>
    public static string UploadTempPathRoot(bool physical = true)
    {
        if (physical)
            return !Empty(Config.UploadTempPath) && !Empty(Config.UploadTempHrefPath) ? IncludeTrailingDelimiter(Config.UploadTempPath, true) : UploadPath(true);
        else
            return !Empty(Config.UploadTempPath) && !Empty(Config.UploadTempHrefPath) ? IncludeTrailingDelimiter(Config.UploadTempHrefPath, false) : UploadPath(false);
    }

    /// <summary>
    /// Get temp upload path
    /// </summary>
    /// <param name="physical">Physical path or not (default is true)</param>
    /// <returns>Temp upload path</returns>
    public static string UploadTempPath(bool physical = true)
    {
        string sessionId = !Empty(Session.SessionId) ? Session.SessionId : ExportId;
        string path = UploadTempPathRoot(physical);
        path = IncludeTrailingDelimiter(path + Config.UploadTempFolderPrefix + sessionId, physical);
        if (physical && !DirectoryExists(path))
            CreateFolder(path);
        return path;
    }

    /// <summary>
    /// Get temp upload path
    /// </summary>
    /// <param name="fldvar">Field variable name</param>
    /// <param name="tblvar">Table variable name</param>
    /// <param name="physical">Physical path or not (default is true)</param>
    /// <returns>Temp upload path</returns>
    public static string UploadTempPath(string fldvar, string tblvar, bool physical = true)
    {
        string path = UploadTempPath(physical);
        if (!Empty(tblvar)) {
            path = IncludeTrailingDelimiter(path + tblvar, physical);
            if (!Empty(fldvar))
                path = IncludeTrailingDelimiter(path + fldvar, physical);
        }
        return path;
    }

    /// <summary>
    /// Get temp upload path
    /// </summary>
    /// <param name="fld">DbField</param>
    /// <param name="idx">Index of the field</param>
    /// <param name="tableLevel">Table level or field level</param>
    /// <param name="physical">Physical path or not (default is true)</param>
    /// <returns>Temp upload path</returns>
    public static string UploadTempPath(DbField fld, int idx = -1, bool tableLevel = false, bool physical = true)
    {
        string fldvar = (idx < 0) ? fld.FieldVar : fld.FieldVar.Substring(0, 1) + idx + fld.FieldVar.Substring(1);
        string tblvar = fld.TableVar;
        return UploadTempPath(tableLevel ? "" : fldvar, tblvar, physical);
    }

    /// <summary>
    /// Get temp upload path for API Upload
    /// </summary>
    /// <param name="token">File token to locate the upload temp path</param>
    /// <param name="physical">Physical path or not (default is true)</param>
    /// <returns>Temp upload path</returns>
    public static string UploadTempPath(string token, bool physical = true) // DN
    {
        string path = UploadTempPathRoot(physical);
        return IncludeTrailingDelimiter(path + Config.UploadTempFolderPrefix + token, physical);
    }

    /// <summary>
    /// Render upload field to temp path
    /// </summary>
    /// <param name="fld">Field object</param>
    /// <param name="idx">Row index</param>
    /// <returns></returns>
    public static async Task RenderUploadField(DbField fld, int idx = -1)
    {
        if (CurrentTable.EventCancelled)
            return;
        string folder = UploadTempPath(fld, idx);
        if (!DirectoryExists(folder)) {
            if (!CreateFolder(folder))
                End("Cannot create folder: " + folder);
        }
        CleanPath(folder); // Clean the upload folder
        bool physical = !IsRemote(folder);
        string thumbnailfolder = PathCombine(folder, Config.UploadThumbnailFolder, physical);
        if (!DirectoryExists(thumbnailfolder)) {
            if (!CreateFolder(thumbnailfolder))
                End("Cannot create folder: " + thumbnailfolder);
        }
        if (fld.IsBlob) { // Blob field
            if (!Empty(fld.Upload.DbValue)) {
                // Create upload file
                var filename = !Empty(fld.Upload.FileName) ? fld.Upload.FileName : fld.Param;
                var f = IncludeTrailingDelimiter(folder, physical) + filename;
                f = await CreateUploadFile(f, (byte[])fld.Upload.DbValue);
                // Create thumbnail file
                f = IncludeTrailingDelimiter(thumbnailfolder, physical) + filename;
                byte[] data = (byte[])fld.Upload.DbValue;
                int width = Config.UploadThumbnailWidth;
                int height = Config.UploadThumbnailHeight;
                ResizeBinary(ref data, ref width, ref height);
                f = await CreateUploadFile(f, data);
                fld.Upload.FileName = Path.GetFileName(f); // Update file name
            }
        } else { // Upload to folder
            fld.Upload.FileName = fld.HtmlDecode(ConvertToString(fld.Upload.DbValue)); // Update file name
            if (!Empty(fld.Upload.FileName)) {
                // Create upload file
                string[] files;
                if (fld.UploadMultiple)
                    files = fld.Upload.FileName.Split(Config.MultipleUploadSeparator);
                else
                    files = new string[] { fld.Upload.FileName };
                for (var i = 0; i < files.Length; i++) {
                    string file = files[i];
                    if (!Empty(file)) {
                        string filename = Path.GetFileName(file);
                        string dirname = filename.Remove(filename.LastIndexOf(filename));
                        string filepath = dirname != "" && dirname != "." ? PathCombine(fld.UploadPath ?? "", dirname, !IsRemote(fld.UploadPath)) : fld.UploadPath ?? "";
                        string srcfile = ServerMapPath(filepath) + filename;
                        string f = IncludeTrailingDelimiter(folder, physical) + filename;
                        byte[] data;
                        if (FileExists(srcfile) && !IsDirectory(srcfile)) {
                            data = await FileReadAllBytes(srcfile);
                            f = await CreateUploadFile(f, data);
                            // Create thumbnail file
                            f = IncludeTrailingDelimiter(thumbnailfolder, physical) + filename;
                            int width = Config.UploadThumbnailWidth;
                            int height = Config.UploadThumbnailHeight;
                            ResizeBinary(ref data, ref width, ref height);
                            f = await CreateUploadFile(f, data);
                        } else { // File not found
                            await FileWriteAllBytes(f, Convert.FromBase64String(Config.FileNotFound));
                            data = await FileReadAllBytes(f);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Write uploaded file
    /// </summary>
    /// <param name="f">File path</param>
    /// <param name="data">File data</param>
    /// <returns></returns>
    public static async Task<string> CreateUploadFile(string f, byte[] data)
    {
        await FileWriteAllBytes(f, data);
        string ext = Path.GetExtension(f);
        string newfile = f;
        if (Empty(ext)) { // No file extension
            ext = ContentExtension(data);
            newfile = f + ext;
            if (newfile != f)
                MoveFile(f, newfile);
        }
        return newfile;
    }

    /// <summary>
    /// Get uploaded file names
    /// </summary>
    /// <param name="filetoken">File token</param>
    /// <param name="fullPath">Includes full path or not</param>
    /// <returns>The list of file names</returns>
    public static List<string> GetUploadedFileNames(string filetoken, bool fullPath = true) =>
        HttpUpload.GetUploadedFileNames(filetoken, fullPath);

    /// <summary>
    /// Get uploaded file name(s) as comma separated values by file token
    /// </summary>
    /// <param name="filetoken">File token</param>
    /// <param name="fullPath">Includes full path or not</param>
    /// <returns>file name(s)</returns>
    public static string GetUploadedFileName(string filetoken, bool fullPath = true) =>
        HttpUpload.GetUploadedFileName(filetoken, fullPath);

    /// <summary>
    /// Clean temp upload folders
    /// </summary>
    /// <param name="sessionid">Session ID</param>
    public static void CleanUploadTempPaths(string sessionid = "")
    {
        string folder = UploadTempPathRoot();
        if (!DirectoryExists(folder))
            return;
        var dir = GetDirectoryInfo(folder);
        var subDirs = dir.GetDirectories(Config.UploadTempFolderPrefix + "*");
        foreach (var dirInfo in subDirs) {
            string subfolder = dirInfo.Name;
            string tempfolder = dirInfo.FullName;
            if (Config.UploadTempFolderPrefix + sessionid == subfolder) { // Clean session folder
                CleanPath(tempfolder, true);
            } else {
                if (Config.UploadTempFolderPrefix + Session.SessionId != subfolder) {
                    if (IsEmptyPath(tempfolder)) { // Empty folder
                        CleanPath(tempfolder, true);
                    } else { // Old folder
                        var lastmdtime = dirInfo.LastWriteTime;
                        if (((TimeSpan)(DateTime.Now - lastmdtime)).TotalMinutes > Config.UploadTempFolderTimeLimit)
                            CleanPath(tempfolder, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Clean temp upload folder
    /// </summary>
    /// <param name="token">Upload token</param>
    public static void CleanUploadTempPath(string token) => CleanPath(UploadTempPath(token), true); // Clean the upload folder

    /// <summary>
    /// Clean folder
    /// </summary>
    /// <param name="folder">Folder path</param>
    /// <param name="delete">Delete folder or not</param>
    /// <param name="callback">Callback function for file processing</param>
    public static void CleanPath(string? folder, bool delete = false, Action<string>? callback = null)
    {
        if (folder == null)
            return;
        try {
            if (DirectoryExists(folder)) {
                Collect(); // DN
                // Delete files
                var files = GetFiles(folder);
                foreach (string file in files) {
                    try {
                        if (callback != null)
                            callback(Path.Combine(folder, file));
                        else
                            FileDelete(file);
                        if (Config.Debug)
                            Log("Temp file '" + file + "' deleted.");
                    } catch (Exception e) {
                        if (Config.Debug)
                            Log("Temp file '" + Path.Combine(folder, file) + "' delete failed. Exception: " + e.Message);
                    }
                }
                // Delete directories
                GetDirectories(folder).ToList().ForEach(dir => CleanPath(dir, delete, callback));
                if (delete) {
                    try {
                        DirectoryDelete(folder);
                        if (Config.Debug)
                            Log("Temp folder '" + folder + "' deleted.");
                    } catch (Exception e) {
                        if (Config.Debug)
                            Log("Temp folder '" + folder + "' delete failed. Exception: " + e.Message);
                    }
                }
            }
        } catch (Exception e) {
            if (Config.Debug)
                Log("CleanPath '" + folder + "' failed. Exception: " + e.Message);
        } finally {
            Collect(); // DN
        }
    }

    /// <summary>
    /// Check if empty folder
    /// </summary>
    /// <param name="folder">Folder path</param>
    /// <returns>Is empty or not</returns>
    public static bool IsEmptyPath(string folder)
    {
        if (DirectoryExists(folder)) {
            var files = GetFiles(folder);
            if (files.Length > 0)
                return false;
            var dirs = GetDirectories(folder);
            return dirs.All(dir => !IsEmptyPath(dir));
        }
        return false;
    }

    /// <summary>
    /// Get file count in folder
    /// </summary>
    /// <param name="folder">Folder path</param>
    /// <returns>File count</returns>
    public static int FolderFileCount(string folder) => DirectoryExists(folder) ? GetFiles(folder).Length : 0;

    /// <summary>
    /// Truncate memo field based on specified length, string truncated to nearest space or CrLf
    /// </summary>
    /// <param name="memostr">String to be truncated</param>
    /// <param name="maxlen">Max. length</param>
    /// <param name="removehtml">Remove HTML or not</param>
    /// <returns>Truncated string</returns>
    public static string TruncateMemo(string memostr, int maxlen, bool removehtml)
    {
        string str = removehtml ? RemoveHtml(memostr) : memostr;
        str = Regex.Replace(str, @"\s+", " ");
        int len = str.Length;
        if (len > 0 && len > maxlen) {
            int i = 0;
            while (i >= 0 && i < len) {
                int j = str.IndexOf(" ", i);
                if (j == -1) { // No whitespaces
                    return str.Substring(0, maxlen) + "..."; // Return the first part only
                } else {
                    // Get nearest whitespace
                    if (j > 0)
                        i = j;
                    // Get truncated text
                    if (i >= maxlen) {
                        return str.Substring(0, i) + "...";
                    } else {
                        i++;
                    }
                }
            }
        }
        return str;
    }

    /// <summary>
    /// Remove HTML tags from text
    /// </summary>
    /// <param name="str">String to clean</param>
    /// <returns>Cleaned string</returns>
    public static string RemoveHtml(string str) => Regex.Replace(str, "<[^>]*>", String.Empty);

    /// <summary>
    /// Convert email address to MailboxAddress
    /// </summary>
    /// <param name="email">Email address as string</param>
    /// <returns>MailAddress instance</returns>
    public static MailboxAddress ConvertToMailAddress(string email)
    {
        email = email.Trim();
        var m = Regex.Match(email, @"^(.+)<([\w.%+-]+@[\w.-]+\.[A-Z]{2,6})>$", RegexOptions.IgnoreCase);
        if (m.Success)
            return new MailboxAddress(m.Groups[1].Value.Trim(), m.Groups[2].Value);
        return new MailboxAddress(email, email);
    }

    /// <summary>
    /// Send email (async)
    /// </summary>
    /// <param name="from">Sender</param>
    /// <param name="to">Recipients</param>
    /// <param name="cc">CC recipients</param>
    /// <param name="bcc">BCC recipients</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="format">Format ("html" or not)</param>
    /// <param name="charset">Charset</param>
    /// <param name="options">Secure socket options</param>
    /// <param name="attachments">Attachments</param>
    /// <param name="images">Images</param>
    /// <param name="smtp">Custom SmtpClient</param>
    /// <returns>Whether the email is sent successfully</returns>
    public static async Task<bool> SendEmailAsync(string from, string to, string cc, string bcc, string subject, string body, string format, string charset, SecureSocketOptions options, List<Dictionary<string, string>>? attachments = null, List<string>? images = null, SmtpClient? smtp = null)
    {
        var mail = new MimeMessage();
        if (!Empty(from))
            mail.From.Add(ConvertToMailAddress(from));
        if (!Empty(to))
            to.Replace(';', ',').Split(',').Select(x => ConvertToMailAddress(x)).ToList().ForEach(address => mail.To.Add(address));
        if (!Empty(cc))
            cc.Replace(';', ',').Split(',').Select(x => ConvertToMailAddress(x)).ToList().ForEach(address => mail.Cc.Add(address));
        if (!Empty(bcc))
            bcc.Replace(';', ',').Split(',').Select(x => ConvertToMailAddress(x)).ToList().ForEach(address => mail.Bcc.Add(address));
        mail.Subject = subject;
        var builder = new BodyBuilder();
        if (SameText(format, "html"))
            builder.HtmlBody = body;
        else
            builder.TextBody = body;
        attachments?.ForEach(attachment => {
            if (attachment.TryGetValue("filename", out string? filename))
            {
                if (attachment.TryGetValue("content", out string? content))
                    builder.Attachments.Add(filename, Encoding.UTF8.GetBytes(content));
                else
                    builder.Attachments.Add(filename);
            }
        });
        images?.ForEach(tmpimage => {
            string file = UploadTempPath() + tmpimage;
            if (FileExists(file)) {
                var res = builder.LinkedResources.Add(file);
                res.ContentId = Path.GetFileNameWithoutExtension(file); // Remove extension (filename as cid)
            }
        });
        mail.Body = builder.ToMessageBody();
        smtp ??= new ();
        string host = !Empty(Config.SmtpServer) ? Config.SmtpServer : "localhost";
        int port = Config.SmtpServerPort > 0 ? Config.SmtpServerPort : 0;
        string smtpServerUsername = Config.SmtpServerUsername;
        string smtpServerPassword = Config.SmtpServerPassword;
        if (!Empty(Config.SmtpServerUsername) && !Empty(Config.SmtpServerPassword)) {
            if (Config.EncryptionEnabled) {
                smtpServerUsername = AesDecrypt(smtpServerUsername);
                smtpServerPassword = AesDecrypt(smtpServerPassword);
            }
        }
        try {
            await smtp.ConnectAsync(host, port, options);
            if (!Empty(smtpServerUsername) && !Empty(smtpServerPassword))
                await smtp.AuthenticateAsync(smtpServerUsername, smtpServerPassword);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
            return true;
        } catch (Exception e) {
            ClientSendError = e.Message;
            if (Config.Debug)
                throw;
            return false;
        }
    }

    /// <summary>
    /// Send email (async)
    /// </summary>
    /// <param name="from">Sender</param>
    /// <param name="to">Recipients</param>
    /// <param name="cc">CC recipients</param>
    /// <param name="bcc">BCC recipients</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="format">Format ("html" or not)</param>
    /// <param name="charset">Charset</param>
    /// <param name="ssl">Use SSL or not (see https://github.com/jstedfast/MailKit/blob/03d8c318b204140a03bda33bd001bb4e8a6cd4f0/MailKit/MailService.cs)</param>
    /// <param name="attachments">Attachments</param>
    /// <param name="images">Images</param>
    /// <param name="smtp">Custom SmtpClient</param>
    /// <returns>Whether the email is sent successfully</returns>
    public static async Task<bool> SendEmailAsync(string from, string to, string cc, string bcc, string subject, string body, string format, string charset, bool ssl = false, List<Dictionary<string, string>>? attachments = null, List<string>? images = null, SmtpClient? smtp = null) =>
        await SendEmailAsync(from, to, cc, bcc, subject, body, format, charset, ssl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, attachments, images, smtp);

    /// <summary>
    /// Send email
    /// </summary>
    /// <param name="from">Sender</param>
    /// <param name="to">Recipients</param>
    /// <param name="cc">CC recipients</param>
    /// <param name="bcc">BCC recipients</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="format">Format ("html" or not)</param>
    /// <param name="charset">Charset</param>
    /// <param name="ssl">Use SSL or not</param>
    /// <param name="attachments">Attachments</param>
    /// <param name="images">Images</param>
    /// <param name="smtp">Custom SmtpClient</param>
    /// <returns>Whether the email is sent successfully</returns>
    public static bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, string format, string charset, bool ssl = false, List<Dictionary<string, string>>? attachments = null, List<string>? images = null, SmtpClient? smtp = null) =>
        SendEmailAsync(from, to, cc, bcc, subject, body, format, charset, ssl, attachments, images, smtp).GetAwaiter().GetResult();

    /// <summary>
    /// Send email
    /// </summary>
    /// <param name="from">Sender</param>
    /// <param name="to">Recipients</param>
    /// <param name="cc">CC recipients</param>
    /// <param name="bcc">BCC recipients</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="format">Format ("html" or not)</param>
    /// <param name="charset">Charset</param>
    /// <param name="options">Secure socket options</param>
    /// <param name="attachments">Attachments</param>
    /// <param name="images">Images</param>
    /// <param name="smtp">Custom SmtpClient</param>
    /// <returns>Whether the email is sent successfully</returns>
    public static bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, string format, string charset, SecureSocketOptions options, List<Dictionary<string, string>>? attachments = null, List<string>? images = null, SmtpClient? smtp = null) =>
        SendEmailAsync(from, to, cc, bcc, subject, body, format, charset, options, attachments, images, smtp).GetAwaiter().GetResult();

    /// <summary>
    /// Change the file name of the uploaded file
    /// </summary>
    /// <param name="folder">Folder name</param>
    /// <param name="fileName">File name</param>
    /// <returns>Changed file name</returns>
    public static string UploadFileName(string folder, string fileName) => UniqueFileName(folder, fileName);

    /// <summary>
    /// Change the file name of the uploaded file using global upload folder
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <returns>Changed file name</returns>
    public static string UploadFileName(string fileName) => UploadFileName(UploadPath(true), fileName);

    /// <summary>
    /// Generate a unique file name for a folder (filename(n).ext)
    /// </summary>
    /// <param name="folder">Output folder</param>
    /// <param name="orifn">Original file name</param>
    /// <param name="indexed">Index starts from '(n)' at the end of the original file name</param>
    /// <returns>The unique file name</returns>
    public static string UniqueFileName(string folder, string orifn, bool indexed = false)
    {
        // Check folder
        if (!DirectoryExists(folder) && !CreateFolder(folder))
            End("Folder does not exist: " + folder);
        folder = IncludeTrailingDelimiter(folder, true);

        // Check file
        if (Empty(orifn))
            End("Missing file name");
        string ext = Path.GetExtension(orifn);
        string oldfn = Path.GetFileNameWithoutExtension(orifn);
        if (Empty(ext))
            End("Missing file extension");
        if (!CheckFileType(orifn))
            End("File extension not allowed: " + ext);
        if (oldfn.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            End("File name contains invalid character(s): " + orifn);

        // Suffix
        int i = 0;
        string suffix = "";
        if (indexed) {
            Match m = Regex.Match(@"\(\d+\)$", oldfn);
            i = m.Success ? ConvertToInt(m.Groups[1].Value) : 1;
            suffix = "(" + i.ToString() + ")";
        }

        // Check to see if filename exists
        string name = Regex.Replace(oldfn, @"\(\d+\)$", ""); // Remove "(n)" at the end of the file name
        while (FileExists(folder + name + suffix + ext)) {
            i++;
            suffix = "(" + i.ToString() + ")";
        }
        return name + suffix + ext;
    }

    /// <summary>
    /// Generate a unique file name for multiple folders (filename(n).ext)
    /// </summary>
    /// <param name="folders">Output folders</param>
    /// <param name="orifn">Original file name</param>
    /// <param name="indexed">Index starts from '(n)' at the end of the original file name</param>
    /// <returns>The unique file name</returns>
    public static string UniqueFileName(IList<string> folders, string orifn, bool indexed = false)
    {
        string fn = orifn;
        foreach (string folder in folders) {
            fn = UniqueFileName(folder, fn, indexed);
        }
        return fn;
    }

    /// <summary>
    /// Get ASP.NET field data type
    /// </summary>
    /// <param name="fldtype">Field ADO data type</param>
    /// <returns>ASP.NET field data type</returns>
    public static DataType FieldDataType(int fldtype) =>
        fldtype switch {
            20 or 3 or 2 or 16 or 4 or 5 or 131 or 139 or 6 or 17 or 18 or 19 or 21 => DataType.Number, // Numeric
            7 or 133 or 135 => DataType.Date, // Date
            146 => DataType.Date, // DateTimeOffset
            134 or 145 => DataType.Time, // Time
            201 or 203 => DataType.Memo,
            129 or 130 or 200 or 202 => DataType.String, // String
            11 => DataType.Boolean, // Boolean
            72 => DataType.Guid, // GUID
            128 or 204 or 205 => DataType.Blob, // Binary
            141 => DataType.Xml, // XML
            _ => DataType.Other
        };

    /// <summary>
    /// Get field query builder data type
    /// </summary>
    /// <param name="fldtype">Field ADO data type</param>
    /// <returns>Field query builder data type</returns>
    public static string FieldQueryBuilderDataType(int fldtype) =>
        fldtype switch {
            20 or 3 or 2 or 16 or 17 or 18 or 19 or 21 => "integer", // Integer
            4 or 5 or 131 or 139 or 6 => "double", // Double
            7 or 133 or 135 => "datetime", // Date
            146 => "datetime", // DateTimeOffset
            134 or 145 => "datetime", // Time
            _ => "string"
        };

    /// <summary>
    /// Get web root // DN
    /// </summary>
    /// <param name="physical">Physical path or not</param>
    /// <returns>The path (returns wwwroot, NOT project folder)</returns>
    public static string AppRoot(bool physical = true)
    {
        string p = physical
            ? WebRootPath // Physical path
            : Request?.PathBase.ToString() ?? ""; // Path relative to host
        return IncludeTrailingDelimiter(p, physical);
    }

    /// <summary>
    /// Get path relative to wwwroot
    /// </summary>
    /// <param name="path">Path to map</param>
    /// <param name="physical">Physical or not</param>
    /// <returns>Mapped path</returns>
    public static string MapPath(string path, bool physical = true)
    {
        if (Path.IsPathRooted(path) || IsRemote(path))
            return path;
        path = path.Trim().Replace("\\", "/");
        path = Regex.Replace(path, @"^[~/]+", "");
        return PathCombine(AppRoot(physical), path, physical); // relative to wwwroot
    }

    /// <summary>
    /// Get path (not file) with trailing delimiter relative to wwwroot
    /// </summary>
    /// <param name="physical">Physical or not</param>
    /// <param name="path">Path to map</param>
    /// <returns>Mapped path</returns>
    public static string MapPath(bool physical, string path) => IncludeTrailingDelimiter(MapPath(path, physical), physical);

    /// <summary>
    /// Get path with trailing delimiter of the global upload folder relative to wwwroot
    /// </summary>
    /// <param name="physical">Physical or not</param>
    /// <returns>Upload path</returns>
    public static string UploadPath(bool physical) => MapPath(physical, Config.UploadDestPath);

    /// <summary>
    /// Get physical path (folder with trailing delimiter or file) relative to wwwroot
    /// </summary>
    /// <param name="path">Path to map</param>
    /// <returns>Mapped path</returns>
    public static string ServerMapPath(string path)
    {
        if (IsRemote(path))
            return path;
        if (Path.HasExtension(path)) // File
            return MapPath(true, Path.GetDirectoryName(path) ?? "") + Path.GetFileName(path);
        else // Folder
            return MapPath(true, path); // With trailing delimiter
    }

    // Get path relative to a base path
    public static string PathCombine(string basePath, string relPath, bool physical)
    {
        physical = !IsRemote(basePath) && physical;
        string delimiter = physical ? Config.PathDelimiter : "/";
        if (basePath != delimiter) // If basePath = root, do not remove delimiter
            basePath = RemoveTrailingDelimiter(basePath, physical);
        relPath = physical ? relPath.Replace("/", Config.PathDelimiter) : relPath.Replace("\\", "/");
        if (relPath.EndsWith(".")) // DN
            relPath = IncludeTrailingDelimiter(relPath, physical);
        if (basePath == delimiter && !physical) // If BasePath = root and not physical path, just return relative path(?)
            return relPath;
        int p1 = relPath.IndexOf(delimiter);
        string Path2 = "";
        while (p1 > -1) {
            string Path = relPath.Substring(0, p1 + 1);
            if (Path == delimiter || Path == "." + delimiter) { // Skip
            } else if (Path == ".." + delimiter) {
                int p2 = basePath.LastIndexOf(delimiter);
                if (p2 == 0) // basePath = "/xxx", cannot move up
                    basePath = delimiter;
                else if (p2 > -1 && !basePath.EndsWith(".."))
                    basePath = basePath.Substring(0, p2);
                else if (!Empty(basePath) && basePath != "..")
                    basePath = "";
                else
                    Path2 += ".." + delimiter;
            } else {
                Path2 += Path;
            }
            try {
                relPath = relPath.Substring(p1 + 1);
            } catch {
                relPath = "";
            }
            p1 = relPath.IndexOf(delimiter);
        }
        return ((!Empty(basePath) && basePath != ".") ? IncludeTrailingDelimiter(basePath, physical) : "") + Path2 + relPath;
    }

    /// <summary>
    /// Remove the last delimiter for a path
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="physical">Physical or not</param>
    /// <returns>Result path</returns>
    public static string RemoveTrailingDelimiter(string path, bool physical)
    {
        string delimiter = (!IsRemote(path) && physical) ? Config.PathDelimiter : "/";
        return path.TrimEnd(Convert.ToChar(delimiter));
    }

    /// <summary>
    /// Include the last delimiter for a path
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="physical">Physical or not</param>
    /// <returns>Result path</returns>
    public static string IncludeTrailingDelimiter(string path, bool physical)
    {
        string delimiter = (!IsRemote(path) && physical) ? Config.PathDelimiter : "/";
        path = RemoveTrailingDelimiter(path, physical);
        return path + delimiter;
    }

    /// <summary>
    /// Write info for config/debug only
    /// </summary>
    public static void Info()
    {
        Write("wwwroot: " + AppRoot() + "<br>");
        Write("Global upload folder (relative to wwwroot): " + Config.UploadDestPath + "<br>");
        Write("Global upload folder (physical): " + UploadPath(true) + "<br>");
        Write("User.Identity.Name = " + ConvertToString(User?.Identity?.Name) + "<br>");
        Write("CurrentUserName() = " + CurrentUserName() + "<br>");
        Write("CurrentUserID() = " + CurrentUserID() + "<br>");
        Write("CurrentParentUserID() = " + CurrentParentUserID() + "<br>");
        Write("IsLoggedIn() = " + (IsLoggedIn() ? "true" : "false") + "<br>");
        Write("IsAdmin() = " + (IsAdmin() ? "true" : "false") + "<br>");
        Write("IsSysAdmin() = " + (IsSysAdmin() ? "true" : "false") + "<br>");
        ResolveSecurity().ShowUserLevelInfo();
    }

    /// <summary>
    /// Get current page name only
    /// </summary>
    /// <returns>Current page name</returns>
    public static string CurrentPageName() => (RouteValues != null)
        ? ConvertToString(IsApi() ? Config.ApiUrl + RouteValues["controller"] : RouteValues["action"]).ToLowerInvariant() // Returns action (file name) only
        : "";

    // Get refer URL
    public static string ReferUrl()
    {
        string url = Request?.Headers[HeaderNames.Referer].ToString() ?? "";
        if (!Empty(url)) {
            var p = Request?.Host.ToString() + Request?.PathBase.ToString() + "/";
            var i = url.LastIndexOf(p);
            if (i > -1)
                url = url.Substring(i + p.Length); // Remove path base
            return url;
        }
        return "";
    }

    // Get refer page name
    public static string ReferPage() => GetPageName(ReferUrl());

    // Get page name // DN
    // param url, must contain action only, e.g. /xxxlist, not /xxx/list // DN
    public static string GetPageName(string url)
    {
        if (!Empty(url)) {
            if (url.Contains("?"))
                url = url.Substring(0, url.LastIndexOf("?")); // Remove querystring first
            var p = Request?.Host.ToString();
            if (!Empty(p) && url.IndexOf(p) is var i && i > -1)
                url = url.Substring(i + p.Length); // Remove host
            p = Request?.PathBase.ToString();
            if (!Empty(p) && url.StartsWith(p))
                url = url.Substring(p.Length); // Remove path base
            return url.StartsWith("/") ? url.Split('/')[1] : url.Split('/')[0]; // Remove parameters
        }
        return "";
    }

    /// <summary>
    /// Get dashboard report page URL (without arguments)
    /// - Note: Since there are more than one pages in dashboard report, the value of $Page changes in the View of dashboard report
    /// </summary>
    /// <returns>Dashboard report page URL</returns>
    public static string CurrentDashboardPageUrl() => DashboardReport && CurrentPage != null ? AppPath(CurrentPage?.PageName ?? "") : AppPath(CurrentPageName()); // DN

    /// <summary>
    /// Get full URL
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="type">Type of the URL</param>
    /// <returns>Full URL</returns>
    public static string FullUrl(string? url = "", string type = "") // DN
    {
        if ((IsAbsoluteUrl(url) || Path.IsPathRooted(url)) && url is string && !url.StartsWith("/") || Request == null)
            return url ?? "";
        if (Empty(url)) {
            return String.Concat(Request.Scheme, "://",
                Request.Host.ToString(),
                Request.PathBase.ToString(),
                Request.Path.ToString(),
                Request.QueryString.ToString());
        } else {
            Config.FullUrlProtocols.TryGetValue(type, out string? protocol);
            return string.Concat(protocol ?? Request.Scheme, "://", Request.Host.ToString(), AppPath(url));
        }
    }

    /// <summary>
    /// Convert CSS file for RTL
    /// </summary>
    /// <param name="f">File name</param>
    /// <param name="rtl">Is RTL</param>
    /// <param name="min">Use minified version</param>
    /// <returns></returns>
    public static string CssFile(string f, bool? rtl = null, bool? min = null)
    {
        bool isRtl = rtl.HasValue ? rtl.Value : IsRTL;
        bool isMin = min.HasValue ? min.Value : Config.UseCompressedStylesheet;
        return isRtl
            ? (isMin ? Regex.Replace(f, @"(.css)$", ".rtl.min.css", RegexOptions.IgnoreCase) : Regex.Replace(f, @"(.css)$", ".rtl.css", RegexOptions.IgnoreCase))
            : (isMin ? Regex.Replace(f, @"(.css)$", ".min.css", RegexOptions.IgnoreCase) : f);
    }

    /// <summary>
    /// Check if HTTPS
    /// </summary>
    /// <returns>Whether request is using HTTPS</returns>
    public static bool IsHttps() => Request?.IsHttps ?? false;

    /// <summary>
    /// Get current URL
    /// </summary>
    /// <returns>Current URL</returns>
    public static string CurrentUrl() => String.Concat(
            Request?.PathBase.ToString(),
            Request?.Path.ToString(),
            Request?.QueryString.ToString()
        );

    /// <summary>
    /// Get application path (relative to host)
    /// </summary>
    /// <param name="url">URL to process</param>
    /// <param name="useArea">Use Area or not</param>
    /// <returns>Result URL</returns>
    public static string AppPath(string url = "", bool useArea = true) // DN
    {
        if (IsAbsoluteUrl(url) || Path.IsPathRooted(url) || url.StartsWith("#") || url.StartsWith("?")) // Includes "javascript:" and "cid:"
            return url;
        var pathBase = IncludeTrailingDelimiter(Request?.PathBase.ToString() ?? "", false);
        return Empty(url)
            ? pathBase
            : (url.StartsWith(pathBase) ? url : pathBase + url);
    }

    // Get URL
    public static string GetUrl(string url) => AppPath(url); // Use absolute path (not relative path) // DN

    /// <summary>
    /// IO methods (delegates)
    /// </summary>

    // Is directory
    public static Func<string, bool> IsDirectory { get; set; } = (path) => (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;

    // Directory exists
    public static Func<string, bool> DirectoryExists { get; set; } = Directory.Exists;

    // Deletes the specified directory and any subdirectories and files in the directory
    public static Action<string> DirectoryDelete { get; set; } = (path) => Directory.Delete(path, true);

    // Directory move
    public static Action<string, string> DirectoryMove { get; set; } = Directory.Move;

    // Directory create
    public static Func<string, bool> DirectoryCreate { get; set; } = (path) => Directory.CreateDirectory(path) != null;

    // File exists
    public static Func<string?, bool> FileExists { get; set; } = File.Exists;

    // File delete
    public static Action<string> FileDelete { get; set; } = File.Delete;

    // File copy
    public static Action<string, string, bool> FileCopy { get; set; } = File.Copy;

    // File copy
    public static Action<string, string> FileMove { get; set; } = File.Move;

    // Read file as bytes
    public static Func<string, Task<byte[]>> FileReadAllBytes { get; set; } = (s) => File.ReadAllBytesAsync(s);

    // Open a file, read all lines of the file with UTF-8 encoding, and then close the file
    public static Func<string, Task<string>> FileReadAllText { get; set; } = (s) => File.ReadAllTextAsync(s);

    // Open a file, read all lines of the file with UTF-8 encoding, and then close the file
    public static Func<string, Encoding, Task<string>> FileReadAllTextWithEncoding { get; set; } = (s, enc) => File.ReadAllTextAsync(s, enc);

    // Open a text file, read all lines of the file, and then close the file
    public static Func<string, Task<string[]>> FileReadAllLines { get; set; } = (s) => File.ReadAllLinesAsync(s);

    // Write file with bytes
    public static Func<string, byte[], Task> FileWriteAllBytes { get; set; } = (s, data) => File.WriteAllBytesAsync(s, data);

    // Open a file, write the string to the file with UTF-8 encoding without a Byte-Order Mark (BOM), and then close the file
    public static Func<string, string, Task> FileWriteAllText { get; set; } = (s, data) => File.WriteAllTextAsync(s, data);

    // Open a text file, write all lines to the file, and then close the file
    public static Func<string, IEnumerable<string>, Task> FileWriteAllLines { get; set; } = (s, data) => File.WriteAllLinesAsync(s, data);

    // Create a StreamWriter that appends UTF-8 encoded text to an existing file, or to a new file if the specified file does not exist
    public static Func<string, StreamWriter> FileAppendText { get; set; } = File.AppendText;

    // Create or opens a file for writing UTF-8 encoded text
    public static Func<string, StreamWriter> FileCreateText { get; set; } = File.CreateText;

    // Open an existing file or create a new file for writing
    public static Func<string, FileStream> FileOpenWrite { get; set; } = File.OpenWrite;

    // Create file info
    public static Func<string, FileInfo> GetFileInfo { get; set; } = (file) => new FileInfo(file);

    // Create directory info
    public static Func<string, DirectoryInfo> GetDirectoryInfo { get; set; } = (path) => new DirectoryInfo(path);

    // Get the names of subdirectories (including their paths) in the specified directory
    public static Func<string, string[]> GetDirectories { get; set; } = (path) => Directory.GetDirectories(path);

    // Get the names of files (including their paths) in the specified directory
    public static Func<string, string[]> GetFiles { get; set; } = (path) => Directory.GetFiles(path);

    // Get the names of files (including their paths) that match the specified search pattern in the specified directory
    public static Func<string, string, string[]> SearchFiles { get; set; } = (path, searchPattern) => Directory.GetFiles(path, searchPattern);

    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="filePath">File to delete</param>
    public static void DeleteFile(string filePath)
    {
        try {
            if (FileExists(filePath)) {
                FileDelete(filePath);
                Collect(); // DN
            }
        } catch {}
    }

    /// <summary>
    /// Rename/Move file
    /// </summary>
    /// <param name="oldFile">Old file</param>
    /// <param name="newFile">New file</param>
    public static void MoveFile(string oldFile, string newFile)
    {
        Collect(); // DN
        FileMove(oldFile, newFile);
    }

    /// <summary>
    /// Copy file
    /// </summary>
    /// <param name="srcFile">Source file</param>
    /// <param name="destFile">Target file</param>
    public static void CopyFile(string srcFile, string destFile)
    {
        Collect(); // DN
        FileCopy(srcFile, destFile, false);
    }

    /// <summary>
    /// Copy file
    /// </summary>
    /// <param name="srcFile">Source file</param>
    /// <param name="destFile">Target file</param>
    /// <param name="overwrite">Overwrite or not</param>
    public static void CopyFile(string srcFile, string destFile, bool overwrite)
    {
        Collect(); // DN
        FileCopy(srcFile, destFile, overwrite);
    }

    /// <summary>
    /// Copy file
    /// </summary>
    /// <param name="folder">Target folder</param>
    /// <param name="fn">Target file name</param>
    /// <param name="file">Old file</param>
    /// <param name="overwrite">Overwrite or not</param>
    /// <returns></returns>
    public static bool CopyFile(string folder, string fn, string file, bool overwrite)
    {
        if (FileExists(file)) {
            string newfile = IncludeTrailingDelimiter(folder, true) + fn;
            if (CreateFolder(folder)) {
                try {
                    CopyFile(file, newfile, overwrite);
                    return true;
                } catch {
                    if (Config.Debug)
                        throw;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Create folder
    /// </summary>
    /// <param name="folder">Target folder</param>
    /// <returns>Whether folder is created successfully</returns>
    public static bool CreateFolder(string folder)
    {
        try {
            return DirectoryCreate(folder);
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Calculate Hash for field value
    /// </summary>
    /// <param name="value">Field value</param>
    /// <param name="fldType">Field type</param>
    /// <returns>Hash</returns>
    public static string GetFieldHash(object value, DataType fldType)
    {
        var hash = "";
        if (IsNull(value)) {
            return "";
        } else if (fldType == DataType.Blob) {
            var bytes = (byte[])value;
            if (Config.BlobFieldByteCount > 0 && bytes.Length > Config.BlobFieldByteCount) {
                hash = BitConverter.ToString(bytes, 0, Config.BlobFieldByteCount);
            } else {
                hash = BitConverter.ToString(bytes);
            }
        } else if (fldType == DataType.Memo) {
            hash = ConvertToString(value);
            if (Config.BlobFieldByteCount > 0 && hash.Length > Config.BlobFieldByteCount)
                hash = hash.Substring(0, Config.BlobFieldByteCount);
        } else {
            hash = ConvertToString(value);
        }
        return Md5(hash);
    }

    /// <summary>
    /// Create temp image file from binary data
    /// </summary>
    /// <param name="filedata">File data</param>
    /// <param name="cid">Output as cid URL, otherwise as base64 URL</param>
    /// <returns>cid or base64 URL</returns>
    public static async Task<string> TempImage(byte[]? filedata, bool cid = false)
    {
        if (filedata == null)
            return ""; // DN
        string f = UploadTempPath() + Path.GetRandomFileName();
        using var ms = new MemoryStream(filedata);
        try {
            var img = new MagickImageInfo(ms); // DN
            f = img.Format switch {
                MagickFormat.Gif => Path.ChangeExtension(f, ".gif"),
                MagickFormat.Jpeg => Path.ChangeExtension(f, ".jpg"),
                MagickFormat.Jpg => Path.ChangeExtension(f, ".jpg"),
                MagickFormat.Png => Path.ChangeExtension(f, ".png"),
                MagickFormat.Bmp => Path.ChangeExtension(f, ".bmp"),
                _ => f
            };
            using var fs = new FileStream(f, FileMode.Create, FileAccess.Write);
            ms.WriteTo(fs);
        } catch {
            return "";
        }
        string tmpimage = Path.GetFileName(f);
        TempImages.Add(tmpimage);
        if (cid)
            return "cid:" + Path.GetFileNameWithoutExtension(tmpimage); // Temp image as cid URL
        else
            return await ImageFileToBase64Url(f); // Temp image as base64 URL
    }

    /// <summary>
    /// Get image tag from base64 data URL (data:mime type;base64,image data)
    /// </summary>
    /// <param name="imageFile">Image file</param>
    /// <returns>Base 64 URL</returns>
    public static async Task<string> ImageFileToBase64Url(string imageFile)
    {
        if (!File.Exists(imageFile)) // File not found, ignore
            return imageFile;
        byte[] bytes = await FileReadAllBytes(imageFile);
        return ImageToBase64Url(bytes);
    }

    /// <summary>
    /// Get image tag from base64 data URL (data:mime type;base64,image data)
    /// </summary>
    /// <param name="bytes">Image data</param>
    /// <returns>Base 64 URL</returns>
    public static string ImageToBase64Url(byte[] bytes)
    {
        return "data:" + ContentType(bytes) + ";base64," + Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Extract data from base64 data URL (data:mime type;base64,image data)
    /// </summary>
    /// <param name="dataUrl">Data URL</param>
    /// <returns>Base 64 URL</returns>
    public static byte[]? DataFromBase64Url(string dataUrl)
    {
        return dataUrl.StartsWith("data:") && dataUrl.Contains(";base64,")
            ? Convert.FromBase64String(dataUrl.Substring(dataUrl.IndexOf(";base64,") + 8))
            : null;
    }

    /// <summary>
    /// Get temp image from base64 data URL (data:mime type;base64,image data)
    /// </summary>
    /// <param name="dataUrl">Data URL</param>
    /// <returns>Temp image URL</returns>
    public static async Task<string> TempImageFromBase64Url(string dataUrl)
    {
        byte[]? data = DataFromBase64Url(dataUrl);
        if (data != null)
            dataUrl = await CreateUploadFile(UploadTempPath() + Random(), data);
        return dataUrl;
    }

    /// <summary>
    /// Garbage collection
    /// </summary>
    public static void Collect()
    {
        // Force garbage collection
        GC.Collect();
        // Wait for all finalizers to complete before continuing.
        // Without this call to GC.WaitForPendingFinalizers,
        // the worker loop below might execute at the same time
        // as the finalizers.
        // With this call, the worker loop executes only after
        // all finalizers have been called.
        GC.WaitForPendingFinalizers();
        // Do another garbage collection
        GC.Collect();
    }

    /// <summary>
    /// Invoke static method of project class
    /// </summary>
    /// <param name="name">Name of method</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Returned value of the method</returns>
    public static object? Invoke(string name, object?[]? parameters = null) =>
        typeof(cityfmcodetests).GetMethod(name)?.Invoke(null, parameters);

    /// <summary>
    /// Get method
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="name">Method name</param>
    /// <returns>The method info</returns>
    public static MethodInfo? GetMethod(object? obj, string name) =>
        obj?.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

    /// <summary>
    /// Get property
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="name">Property name</param>
    /// <returns>The property info</returns>
    public static PropertyInfo? GetProperty(object? obj, string name) =>
        obj?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

    /// <summary>
    /// Invoke method of an object
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="name">Method name</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Returned value of the method</returns>
    public static object? Invoke(object? obj, string name, object?[]? parameters = null) =>
        GetMethod(obj, name)?.Invoke(obj, parameters);

    /// <summary>
    /// Get property of project class
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>Property value</returns>
    public static object? GetPropertyValue(string name) =>
        typeof(cityfmcodetests).GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance)?.GetValue(null);

    /// <summary>
    /// Get property value of an object
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="name">Property name</param>
    /// <returns>Property value</returns>
    public static object? GetPropertyValue(object? obj, string name) =>
        obj?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance)?.GetValue(obj);

    /// <summary>
    /// Get property value of an object
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="name">Property name</param>
    /// <param name="value">Property value</param>
    public static void SetPropertyValue(object? obj, string name, object value) =>
        obj?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance)?.SetValue(obj, value);

    /// <summary>
    /// Create a HTTP client
    /// </summary>
    /// <returns>HttpClient</returns>
    public static HttpClient GetHttpClient() => HttpClientFactory.CreateClient();

    /// <summary>
    /// Download requested resource as string (async)
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="data">A collection of name/value pairs</param>
    /// <param name="method">HTTP method</param>
    /// <returns>Requested resource as string</returns>
    public static async Task<string> DownloadStringAsync(string url, IEnumerable<KeyValuePair<string, string>>? data = null, HttpMethod? method = null)
    {
        var client = GetHttpClient();
        if (data != null) {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get, url) {
                Content = new FormUrlEncodedContent(data)
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStringAsync();
            } else {
                throw new WebException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        return await client.GetStringAsync(url);
    }

    /// <summary>
    /// Download requested resource as string
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="data">A collection of name/value pairs</param>
    /// <param name="method">HTTP method</param>
    /// <returns>Requested resource as string</returns>
    public static string DownloadString(string url, IEnumerable<KeyValuePair<string, string>>? data = null, HttpMethod? method = null) =>
        DownloadStringAsync(url, data).GetAwaiter().GetResult();

    /// <summary>
    /// Download requested resource as byte array (async)
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="data">A collection of name/value pairs</param>
    /// <param name="method">HTTP method</param>
    /// <returns>Requested resource as byte[]</returns>
    public static async Task<byte[]> DownloadDataAsync(string url, IEnumerable<KeyValuePair<string, string>>? data = null, HttpMethod? method = null)
    {
        var client = GetHttpClient();
        if (data != null) {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get, url) {
                Content = new FormUrlEncodedContent(data)
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadAsByteArrayAsync();
            } else {
                throw new WebException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        return await client.GetByteArrayAsync(url);
    }

    /// <summary>
    /// Download requested resource as byte array
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="data">A collection of name/value pairs</param>
    /// <param name="method">HTTP method</param>
    /// <returns>Requested resource as byte[]</returns>
    public static byte[] DownloadData(string url, IEnumerable<KeyValuePair<string, string>>? data = null, HttpMethod? method = null) =>
        DownloadDataAsync(url, data).GetAwaiter().GetResult();

    // Add query string to URL
    public static string UrlAddQuery(string url, string qry) => Empty(qry) ? url : url + (url.Contains("?") ? "&" : "?") + qry;

    /// <summary>
    /// Allow list
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>Whether user has permission to list the table</returns>
    public static bool AllowList(string tableName) => ResolveSecurity().AllowList(tableName);

    /// <summary>
    /// Allow view
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>Whether user has permission to view records from the table</returns>
    public static bool AllowView(string tableName) => ResolveSecurity().AllowView(tableName);

    /// <summary>
    /// Allow add
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>Whether user has permission to add records to the table</returns>
    public static bool AllowAdd(string tableName) => ResolveSecurity().AllowAdd(tableName);

    /// <summary>
    /// Allow edit
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>Whether user has permission to edit records of the table</returns>
    public static bool AllowEdit(string tableName) => ResolveSecurity().AllowEdit(tableName);

    /// <summary>
    /// Allow delete
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>Whether user has permission to delete records from the table</returns>
    public static bool AllowDelete(string tableName) => ResolveSecurity().AllowDelete(tableName);

    /// <summary>
    /// Check date
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="format">Date format</param>
    /// <returns>Whether the value is valid</returns>
    public static bool CheckDate(string? value, string format = "")
    {
        if (Empty(value))
            return true;
        value = value.Trim();
        if (Regex.IsMatch(value, @"^([0-9]{4})-([0][1-9]|[1][0-2])-([0][1-9]|[1|2][0-9]|[3][0|1])( (0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:([0-5][0-9]))?)?$")) // Date/Time
            return true;
        return ParseDateTime(value, format) != null;
    }

    /// <summary>
    /// Check time
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="format">Time format</param>
    /// <returns>Whether the value is valid</returns>
    public static bool CheckTime(string? value, string format = "")
    {
        if (Empty(value))
            return true;
        value = value.Trim();
        if (Regex.IsMatch(value, @"^(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:([0-5][0-9]))?$")) // Time
            return true;
        return ParseDateTime(value, format) != null;
    }

    // Check integer
    public static bool CheckInteger(string? value) => Empty(value) || IsNumeric(value) && ParseInteger(value) != null;

    // Check number
    public static bool CheckNumber(string? value) => Empty(value) || IsNumeric(value) && ParseNumber(value) != null;

    // Check range
    public static bool CheckRange(string? value, object? min, object? max)
    {
        if (Empty(value))
            return true;
        if (IsNumeric(value) && (IsNumeric(min) || IsNumeric(max))) { // Number
            double? val = ParseNumber(value);
            if (min != null && val < ParseNumber(ConvertToString(min)) || max != null && val > ParseNumber(ConvertToString(max)))
                return false;
        } else if (min != null && String.Compare(value, ConvertToString(min)) < 0 || max != null && String.Compare(value, ConvertToString(max)) > 0) // String
            return false;
        return true;
    }

    // Check US phone number
    public static bool CheckPhone(string? value) => Empty(value) || Regex.IsMatch(value, "^\\(\\d{3}\\) ?\\d{3}( |-)?\\d{4}|^\\d{3}( |-)?\\d{3}( |-)?\\d{4}");

    // Check US zip code
    public static bool CheckZip(string? value) => Empty(value) || Regex.IsMatch(value, "^\\d{5}|^\\d{5}-\\d{4}");

    // Check credit card
    public static bool CheckCreditCard(string? value) => Empty(value) || (new Dictionary<string, string>
        {
            {"visa", "^4\\d{3}[ -]?\\d{4}[ -]?\\d{4}[ -]?\\d{4}"},
            {"mastercard", "^5[1-5]\\d{2}[ -]?\\d{4}[ -]?\\d{4}[ -]?\\d{4}"},
            {"discover", "^6011[ -]?\\d{4}[ -]?\\d{4}[ -]?\\d{4}"},
            {"amex", "^3[4,7]\\d{13}"},
            {"diners", "^3[0,6,8]\\d{12}"},
            {"bankcard", "^5610[ -]?\\d{4}[ -]?\\d{4}[ -]?\\d{4}"},
            {"jcb", "^[3088|3096|3112|3158|3337|3528]\\d{12}"},
            {"enroute", "^[2014|2149]\\d{11}"},
            {"switch", "^[4903|4911|4936|5641|6333|6759|6334|6767]\\d{12}"}
        }).Any(kvp => Regex.IsMatch(value, kvp.Value) && CheckSum(value));

    // Check sum
    public static bool CheckSum(string value)
    {
        byte digit;
        value = value.Replace("-", "").Replace(" ", "");
        int checksum = 0;
        for (int i = 2 - (value.Length % 2); i <= value.Length; i += 2)
            checksum = checksum + Convert.ToByte(value[i - 1]);
        for (int i = (value.Length % 2) + 1; i <= value.Length; i += 2) {
            digit = Convert.ToByte(Convert.ToByte(value[i - 1]) * 2);
            checksum = checksum + ((digit < 10) ? digit : (digit - 9));
        }
        return (checksum % 10 == 0);
    }

    // Check US social security number
    public static bool CheckSsn(string? value) => Empty(value) || Regex.IsMatch(value, "^(?!000)([0-6]\\d{2}|7([0-6]\\d|7[012]))([ -]?)(?!00)\\d\\d\\3(?!0000)\\d{4}");

    // Check email
    public static bool CheckEmail(string? value) => Empty(value) || Regex.IsMatch(value.Trim(), @"^[\w.%+-]+@[\w.-]+\.[A-Z]{2,18}$", RegexOptions.IgnoreCase);

    // Check emails
    public static bool CheckEmail(string? value, int count)
    {
        if (Empty(value))
            return true;
        string[] emails = value.Replace(",", ";").Split(';');
        if (count > 0 && emails.Length > count)
            return false;
        return emails.All(email => CheckEmail(email));
    }

    // Check GUID
    public static bool CheckGuid(string? value) => Empty(value) || Regex.IsMatch(value, @"^(\{\w{8}-\w{4}-\w{4}-\w{4}-\w{12}\}|\w{8}-\w{4}-\w{4}-\w{4}-\w{12})$");

    // Check file extension
    public static bool CheckFileType(string value, string? exts = null)
    {
        exts ??= Config.UploadAllowedFileExtensions;
        if (Empty(value) || Empty(exts))
            return true;
        var extension = Path.GetExtension(value).Substring(1);
        return exts.Split(',').Any(ext => SameText(ext, extension));
    }

    // Check empty string
    public static bool EmptyString([NotNullWhen(false)] object? value)
    {
        var str = ConvertToString(value);
        if (Regex.IsMatch(str, "&[^;]+;")) // Contains HTML entities
            str = HtmlDecode(str);
        return Empty(str);
    }

    // Partially hide a value
    // - name@domain.com => n**e@domain.com
    // - myname => m***me
    public static string PartialHideValue(string value)
    {
        // Handle empty value
        if (Empty(value))
            return String.Empty;

        // Handle email (split an email by "@")
        string[] ar = value.Split(new char[] { '@' }, 2);
        string name = ar.Length > 1 ? ar[0] : value;
        string domain = ar.Length > 1 ? ar[1] : String.Empty;

        // Get half the length of the first part
        int len = name.Length / 2;
        int len2 = len / 2;

        // Partially hide value by "*"
        return name.Substring(0, len2) + RepeatString("*", len) + name.Substring(len + len2) + (Empty(domain) ? String.Empty : "@" + domain);
    }

    // Repeat string
    public static string RepeatString(string value, int count) => new StringBuilder(value.Length * count).Insert(0, value, count).ToString();

    // Check masked password
    public static bool IsMaskedPassword([NotNullWhen(true)] object? value) => !Empty(value) && Regex.IsMatch(ConvertToString(value), @"^\*+$");

    // Check by regular expression
    public static bool CheckByRegEx(string? value, string pattern) => Empty(value) || Regex.IsMatch(value, pattern);

    // Check by regular expression
    public static bool CheckByRegEx(string? value, string pattern, RegexOptions options) => Empty(value) || Regex.IsMatch(value, pattern, options);

    // Check URL // DN
    public static bool CheckUrl(string? url) => !Empty(url) && Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    // Check special characters for user name
    public static bool CheckUsername(string? value) => !Empty(value) && Regex.IsMatch(value, @"[" + Regex.Escape(Config.InvalidUsernameCharacters) + "]");

    // Check special characters for password
    public static bool CheckPassword(string? value) => !Empty(value) && Regex.IsMatch(value, @"[" + Regex.Escape(Config.InvalidPasswordCharacters) + "]");

    // Get current date in standard format (yyyy/mm/dd)
    public static string StdCurrentDate() => DateTime.Today.ToString("yyyy'/'MM'/'dd");

    // Get date in standard format (yyyy/mm/dd)
    public static string StdDate(DateTime dt) => dt.ToString("yyyy'/'MM'/'dd");

    // Get current date and time in standard format (yyyy/mm/dd hh:mm:ss)
    public static string StdCurrentDateTime() => DateTime.Now.ToString("yyyy'/'MM'/'dd' 'HH':'mm':'ss");

    // Get date/time in standard format (yyyy/mm/dd hh:mm:ss)
    public static string StdDateTime(DateTime dt) => dt.ToString("yyyy'/'MM'/'dd' 'HH':'mm':'ss");

    // Get current date and time in standard format (yyyy-mm-dd hh:mm:ss)
    public static string DbCurrentDateTime() => DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

    // Encrypt password
    public static string EncryptPassword(string input) => Config.PasswordHash ? BCrypt.Net.BCrypt.HashPassword(input) : Md5(input);

    // Compare password
    public static bool ComparePassword(string pwd, string input)
    {
        if (Config.CaseSensitivePassword) {
            return Config.EncryptedPassword ? Verify(input, pwd) : SameString(pwd, input);
        } else {
            return Config.EncryptedPassword ? Verify(input.ToLower(), pwd) : SameText(pwd, input);
        }
        // Local function to verify password
        bool Verify(string value, string password) {
            try {
                return Config.PasswordHash ? BCrypt.Net.BCrypt.Verify(value, password) : EncryptPassword(value) == password;
            } catch {
                return false;
            }
        }
    }

    // MD5
    public static string Md5(string input)
    {
        var hasher = MD5.Create();
        byte[] data = hasher.ComputeHash(Config.Md5Encoding.GetBytes(input));
        var builder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
            builder.Append(data[i].ToString("x2"));
        return builder.ToString();
    }

    // CRC32
    public static uint Crc32(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        uint crc = 0xffffffff;
        uint poly = 0xedb88320;
        uint[] table = new uint[256];
        uint temp = 0;
        for (uint i = 0; i < table.Length; ++i) {
            temp = i;
            for (int j = 8; j > 0; --j) {
                if ((temp & 1) == 1) {
                    temp = (uint)((temp >> 1) ^ poly);
                } else {
                    temp >>= 1;
                }
            }
            table[i] = temp;
        }
        for (int i = 0; i < bytes.Length; ++i) {
            byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
            crc = (uint)((crc >> 8) ^ table[index]);
        }
        return ~crc;
    }

    /// <summary>
    /// Check if object is array
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Whether object is array</returns>
    public static bool IsArray([NotNullWhen(true)] object? obj) => obj is not null and Array;

    /// <summary>
    /// Check if value is numeric
    /// </summary>
    /// <param name="expression">Value to check</param>
    /// <returns>Whether value is numeric</returns>
    public static bool IsNumeric([NotNullWhen(true)] object? expression)
    {
        if (Empty(expression))
            return false;
        if (expression.GetType().IsPrimitive && expression is not bool and not char || expression is decimal)
            return true;
        string s = ConvertToString(expression).Replace(NativeDigits, LatinDigits);
        return TryParse(CultureInfo.GetCultureInfo("en-US").NumberFormat) || // English locale
            TryParse(CurrentNumberFormat); // Current language locale
        // Local function
        bool TryParse(IFormatProvider info) => System.Double.TryParse(s, NumberStyles.Any, info, out _) ||
            System.Int64.TryParse(s, NumberStyles.Any, info, out _) ||
            System.UInt64.TryParse(s, NumberStyles.Any, info, out _) ||
            System.Decimal.TryParse(s, NumberStyles.Any, info, out _);
    }

    /// <summary>
    /// Check if value is date
    /// </summary>
    /// <param name="expression">Value to check</param>
    /// <returns>Whether value is date</returns>
    public static bool IsDate([NotNullWhen(true)] object? expression) => expression is DateTime || ParseDateTime(ConvertToString(expression)) != null;

    /// <summary>
    /// Check if object is IList and IEnumerable
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Whether the object is IList and IEnumerable</returns>
    public static bool IsList([NotNullWhen(true)] object? obj) => obj is not null and IList and IEnumerable;

    /// <summary>
    /// Check if object is IList&lt;T&gt; and IEnumerable&lt;T&gt;
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Whether the object is IList&lt;T&gt; and IEnumerable&lt;T&gt;</returns>
    public static bool IsList<T>([NotNullWhen(true)] object? obj) => obj is not null and IList<T> and IEnumerable<T>;

    /// <summary>
    /// Check if object is an empty list (IList and IEnumerable)
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Whether the object is empty</returns>
    public static bool IsEmptyList([NotNullWhen(true)] object? obj) => IsList(obj) && ((IList)obj).Count == 0;

    /// <summary>
    /// Check if object is IDictionary
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Wether object is dictionary</returns>
    public static bool IsDictionary([NotNullWhen(true)] object? obj) => obj is not null and IDictionary;

    // Global random
    private static Random GlobalRandom = new ();

    /// <summary>
    /// Generate random number
    /// </summary>
    /// <returns>A random number</returns>
    public static int Random()
    {
        lock (GlobalRandom) {
            Random newRandom = new (GlobalRandom.Next());
            return newRandom.Next();
        }
    }

    /// <summary>
    /// Generate a random code with specified number of digits
    /// </summary>
    /// <param name="n">Number of digits</param>
    /// <returns>A random number</returns>
    public static string Random(int n)
    {
        lock (GlobalRandom) {
            Random newRandom = new (GlobalRandom.Next());
            string s = "";
            for (int i = 0; i < n; i++)
                s = String.Concat(s, newRandom.Next(10).ToString());
            return s;
        }
    }

    /// <summary>
    /// Get query value
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <returns>Value (empty string if name does not exist)</returns>
    public static string Get(string name) => Query.TryGetValue(name, out StringValues sv) ? String.Join(Config.MultipleOptionSeparator, sv.ToArray()) : String.Empty;

    /// <summary>
    /// Get query value as type T
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <typeparam name="T">Data type of the value</typeparam>
    /// <returns>Value (null if name does not exist)</returns>
    public static T Get<T>(string name) => Query.TryGetValue(name, out StringValues sv) ? ChangeType<T>(sv.ToString()) : default(T)!;

    /// <summary>
    /// Get query value
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <param name="value">Value</param>
    /// <returns>Has non empty value or not</returns>
    public static bool Get(string name, [NotNull] out StringValues value) => Query.TryGetValue(name, out value);

    /// <summary>
    /// Get form value
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <returns>Value (empty string if name does not exist)</returns>
    public static string Post(string name) => Form.TryGetValue(name, out StringValues sv) ? String.Join(Config.MultipleOptionSeparator, sv.ToArray()) : String.Empty;

    /// <summary>
    /// Get form value
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <param name="value">Value</param>
    /// <returns>Has non empty value or not</returns>
    public static bool Post(string name, [NotNull] out StringValues value) => Form.TryGetValue(name, out value);

    /// <summary>
    /// Get form value as type T
    /// </summary>
    /// <param name="name">Name of the value</param>
    /// <typeparam name="T">Data type of the value</typeparam>
    /// <returns>Value (null if name does not exist)</returns>
    public static T Post<T>(string name) => Form.TryGetValue(name, out StringValues sv) ? ChangeType<T>(sv.ToString()) : default(T)!;

    /// <summary>
    /// Change object to type T
    /// </summary>
    /// <param name="value">Object to change</param>
    /// <typeparam name="T">Data type</typeparam>
    /// <returns>Value of data type T</returns>
    public static T ChangeType<T>(object? value)
    {
        if (typeof(T) == typeof(int))
            return (T)Convert.ChangeType(ConvertToInt(value), typeof(T));
        else if (typeof(T) == typeof(bool))
            return (T)Convert.ChangeType(ConvertToBool(value), typeof(T));
        return value is IConvertible
            ? (T)Convert.ChangeType(value, typeof(T))
            : value != null
            ? (T)value
            : default(T) ?? throw new Exception($"Failed to convert '{ConvertToString(value)}' to {nameof(T)}");
    }

    /// <summary>
    /// Cookie
    /// </summary>
    public static HttpCookies Cookie;

    /// <summary>
    /// Session
    /// </summary>
    public static HttpSession Session;

    /// <summary>
    /// Write literal in View
    /// </summary>
    /// <param name="value">Value to write</param>
    public static void Write(object value) => View?.WriteLiteral(value);

    /// <summary>
    /// Write binary data to response (async)
    /// </summary>
    /// <param name="value">Value to write</param>
    /// <returns>Task</returns>
    public static Task BinaryWrite(byte[] value) => Response?.Body.WriteAsync(value, 0, value.Length) ?? Task.CompletedTask;

    /// <summary>
    /// Write string to response (async)
    /// </summary>
    /// <param name="str">String to write</param>
    /// <param name="enc">Encoding</param>
    /// <returns>Task</returns>
    public static Task ResponseWrite(string str, string? enc = null) => Response?.WriteAsync(str, Encoding.GetEncoding(enc ?? Config.Charset)) ?? Task.CompletedTask;

    /// <summary>
    /// Clear response body only (not headers)
    /// </summary>
    public static void ResponseClear()
    {
        if (Response?.Body.CanSeek ?? false)
            Response.Body.SetLength(0);
    }

    /// <summary>
    /// Export object info as JSON string
    /// </summary>
    /// <param name="list">Objects to export</param>
    /// <returns>JSON string</returns>
    public static string VarExport(params object[] list)
    {
        string str = "";
        foreach (object value in list) {
            try {
                str += "<pre>" + HtmlEncode(JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })) + "</pre>";
            } catch (Exception e) {
                str += "<pre>" + HtmlEncode(e.Message) + "</pre>";
                continue;
            }
        }
        return str;
    }

    /// <summary>
    /// Write object info for debugging
    /// </summary>
    /// <param name="list">Objects to export</param>
    public static void VarDump(params object[] list)
    {
        var str = VarExport(list);
        if (View != null) {
            Write(str);
        } else {
            var encoding = Encoding.GetEncoding(Config.Charset);
            byte[] data = encoding.GetBytes(str);
            Response?.Body.Write(data, 0, data.Length);
        }
    }

    /// <summary>
    /// End page and throw an exception
    /// </summary>
    /// <param name="list">Values to show</param>
    public static void End(params object[] list) =>
        throw new Exception(String.Join(", ", list.Select(value => value switch {
            string val => val, // String => Message
            not null => JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), // Object
            _ => null
        })));

    /// <summary>
    /// Validate API request (Always use JWT for API) // DN
    /// </summary>
    /// <returns>Valid or not</returns>
    public static bool ValidApiRequest()
    {
        if (IsApi()) {
            // Already authenticated by JwtBearerDefaults.AuthenticationScheme if IsApi(), see Program.cs
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get content file extension
    /// </summary>
    /// <param name="data">data to check</param>
    /// <returns>File extension</returns>
    public static string ContentExtension(byte[] data)
    {
        var ct = ContentType(data);
        return ContentTypeProvider.Mappings.FirstOrDefault(kvp => kvp.Value == ct).Key;
    }

    /// <summary>
    /// Add header
    /// </summary>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    public static void AddHeader(string name, string value)
    {
        if (!Empty(name) && Response != null) // If value is empty, header will be removed
            Response.Headers[name] = value;
    }

    /// <summary>
    /// Remove header
    /// </summary>
    /// <param name="name">Header name to be removed</param>
    public static void RemoveHeader(string name)
    {
        if (!Empty(name) && (Response?.Headers.ContainsKey(name) ?? false))
            Response.Headers.Remove(name);
    }

    /// <summary>
    /// Mobile detect
    /// </summary>
    public static MobileDetect DetectMobile
    {
        get {
            _mobileDetect ??= new MobileDetect();
            return _mobileDetect;
        }
        set => _mobileDetect = value;
    }

    /// <summary>
    /// Check if mobile device
    /// </summary>
    /// <returns>Whether it is mobile device</returns>
    public static bool IsMobile() => DetectMobile.IsMobile;

    /// <summary>
    /// Convert to JSON
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <returns>JSON string</returns>
    public static string ConvertToJson(object? value)
    {
        if (value == null || value.GetType().IsPrimitive || value is string or DateTime or decimal)
            return System.Text.Json.JsonSerializer.Serialize(value);
        return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// Convert to JSON
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="settings">Json serializer settings</param>
    /// <returns>JSON string</returns>
    public static string ConvertToJson(object? value, JsonSerializerSettings settings) => JsonConvert.SerializeObject(value, settings);

    /// <summary>
    /// Convert to JSON
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="convertors">Json convertors</param>
    /// <returns>JSON string</returns>
    public static string ConvertToJson(object? value, params JsonConverter[] convertors) => JsonConvert.SerializeObject(value, convertors);

    /// <summary>
    /// Convert a value to JSON value
    /// </summary>
    /// <param name="val">Value</param>
    /// <param name="type">Data type: "boolean", "string", "date" or "number"</param>
    /// <param name="attr">For (double quoted) HTML attribute</param>
    /// <returns>JSON string</returns>
    public static string ConvertToJson(object? val, string type, bool attr = false)
    {
        if (IsNull(val)) {
            return "null";
        } else if (SameText(type, "number")) {
            return ConvertToString(val);
        } else if (val is DateTime dt) {
            return "new Date(\"" + dt.ToString("o") + "\")";
        } else if (SameText(type, "date")) {
            return "new Date(\"" + val + "\")";
        } else if (SameText(type, "boolean") || val is bool) {
            return ConvertToBool(val) ? "true" : "false";
        } else if (SameText(type, "string") || val is string) {
            string s = ConvertToString(val);
            if (s.Contains("\0")) // Contains null byte
                return "\"binary\"";
            if (s.Length > Config.DataStringMaxLength)
                val = s.Substring(0, Config.DataStringMaxLength);
            if (attr)
                return "\"" + JsEncodeAttribute(s) + "\"";
            else
                return "\"" + JsEncode(s) + "\"";
        }
        return ConvertToJson(val);
    }

    /// <summary>
    /// Convert dictionary to JSON for HTML (double quoted) attributes
    /// </summary>
    /// <param name="dict">Dictionary</param>
    /// <returns>JSON string</returns>
    public static string ConvertToJsonAttribute(Dictionary<string, string> dict) =>
        JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.None);

    /// <summary>
    /// Serialize enum as JSON
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeEnum(object obj) =>
        JsonConvert.SerializeObject(obj, new Newtonsoft.Json.Converters.StringEnumConverter());

    /// <summary>
    /// Deserialize enum from JSON
    /// </summary>
    /// <param name="data">Data</param>
    /// <typeparam name="T">Enum type</typeparam>
    /// <returns>Enum value</returns>
    public static T? DeserializeEnum<T>(string data) =>
        JsonConvert.DeserializeObject<T>(data, new Newtonsoft.Json.Converters.StringEnumConverter());

    /// <summary>
    /// Create instance by class name
    /// </summary>
    /// <param name="name">Class name</param>
    /// <param name="args">Arguments</param>
    /// <param name="types">Types for the class</param>
    /// <returns></returns>
    public static dynamic? CreateInstance(string name, object?[]? args = null, Type[]? types = null)
    {
        var t = Type.GetType(Config.ProjectClassName + "+" + name);
        if (types != null && t != null)
            t = t.MakeGenericType(types);
        return t != null ? Activator.CreateInstance(t, args) : null;
    }

    // Get file IMG tag (for export to email/pdf/html only)
    public static string GetFileImgTag(List<string> files, string className = "")
    {
        return String.Join("<br>", files.Where(f => f.StartsWith("data:") || File.Exists(f)).Select(f => "<img class=\"ew-image" + (!Empty(className) ? " " + className : "") + "\" src=\"" + f + "\" alt=\"\">"));
    }

    // Get file anchor tag
    public static string GetFileATag(DbField fld, object? fileName)
    {
        string[] wrkfiles = {};
        string wrkpath = "";
        string fn = ConvertToString(fileName);
        if (fld.IsBlob) {
            if (!Empty(fld.Upload.DbValue))
                wrkfiles = new string[] {fn};
        } else if (fld.UploadMultiple) {
            wrkfiles = fn.Split(Config.MultipleUploadSeparator);
            var pos = wrkfiles[0].LastIndexOf("/");
            if (pos > -1) {
                wrkpath = wrkfiles[0].Substring(0, pos + 1); // Get path from first file name
                wrkfiles[0] = wrkfiles[0].Substring(pos + 1);
            }
        } else {
            if (!Empty(fld.Upload.DbValue))
                wrkfiles = new string[] {fn};
        }
        var elements = wrkfiles.Where(wrkfile => !Empty(wrkfile))
            .Select(wrkfile => HtmlElement("a", new { href = FullUrl(wrkpath + wrkfile, "href") }, fld.Caption));
        return String.Join("<br>", elements);
    }

    // Get file temp image
    public static async Task<string> GetFileTempImage(DbField fld, string val)
    {
        if (fld.UploadMultiple) {
            var files = val.Split(Config.MultipleUploadSeparator);
            string images = "";
            for (var i = 0; i < files.Length; i++) {
                if (files[i] != "") {
                    var tmpimage = await FileReadAllBytes(fld.PhysicalUploadPath + files[i]);
                    if (fld.ImageResize)
                        ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                    if (images != "") images += Config.MultipleUploadSeparator;
                    images += await TempImage(tmpimage);
                }
            }
            return images;
        } else {
            if (fld.IsBlob) {
                if (!IsNull(fld.Upload.DbValue)) { // DN
                    var tmpimage = (byte[])fld.Upload.DbValue;
                    if (fld.ImageResize)
                        ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                    return await TempImage(tmpimage);
                }
            } else if (val != "") {
                var tmpimage = await FileReadAllBytes(fld.PhysicalUploadPath + val);
                if (fld.ImageResize)
                    ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                return await TempImage(tmpimage);
            }
        }
        return "";
    }

    // Get file image
    public static async Task<byte[]?> GetFileImage(DbField fld, object? val, int width = 0, int height = 0, bool crop = false)
    {
        byte[]? image = null;
        string file = "";
        int wrkWidth = width;
        int wrkHeight = height;
        if (fld.DataType == DataType.Blob) {
            if (val is byte[] b) // DN
                image = b;
        } else if (fld.UploadMultiple) {
            var files = ConvertToString(val).Split(Config.MultipleUploadSeparator);
            file = files.Length > 0 ? fld.PhysicalUploadPath + files[0] : "";
        } else {
            file = fld.PhysicalUploadPath + ConvertToString(val);
        }
        if (FileExists(file))
            image = await FileReadAllBytes(file);
        if (image != null && width > 0) {
            List<Action<MagickImage>> plugins = new ();
            if (crop)
                plugins.Add(mi => mi.Crop(width, height, Gravity.Center));
            ResizeBinary(ref image, ref wrkWidth, ref wrkHeight, plugins);
        }
        return image;
    }

    // Get file upload URL
    public static string GetFileUploadUrl(DbField fld, string? val, bool resize = false)
    {
        if (!EmptyString(val)) {
            string fileUrl = Config.ApiUrl + Config.ApiFileAction;
            string fn;
            if (fld.DataType == DataType.Blob) {
                string tableVar = !EmptyString(fld.SourceTableVar) ? fld.SourceTableVar : fld.TableVar;
                fn = AppPath(fileUrl) + "/" + RawUrlEncode(tableVar) + "/" + RawUrlEncode(fld.Param) + "/" + val +
                    "?session=" + Encrypt(Session.SessionId) + "&" + Config.TokenName + "=" + CurrentToken;
                if (resize)
                    fn += "&resize=1&width=" + fld.ImageWidth + "&height=" + fld.ImageHeight;
            } else {
                bool encrypt = Config.EncryptFilePath;
                string path = (encrypt || resize) ? fld.PhysicalUploadPath : fld.HrefPath;
                string key = Config.RandomKey + Session.SessionId;
                if (encrypt) {
                    fn = AppPath(fileUrl) + "/" + Encrypt(path + val, key);
                    if (resize)
                        fn += "?width=" + fld.ImageWidth + "&height=" + fld.ImageHeight;
                } else if (resize) {
                    fn = AppPath(fileUrl) + "/" + Encrypt(path + val, key) + "?width=" + fld.ImageWidth + "&height=" + fld.ImageHeight; // Encrypt the physical path
                } else {
                    fn = IsRemote(path) ? path : AppPath(UrlEncodeFilePath(path));
                    fn += UrlEncodeFilePath(val);
                }
                fn += (fn.Contains("?") ? "&" : "?");
                fn += "session=" + Encrypt(Session.SessionId) + "&" + Config.TokenName + "=" + CurrentToken;
            }
            return fn;
        }
        return "";
    }

    // URL-encode file name
    public static string UrlEncodeFilename(string fn)
    {
        string path, filename;
        if (fn.Contains("?")) {
            var arf = fn.Split('?');
            fn = arf[1];
            var ar = fn.Split('&');
            for (var i = 0; i < ar.Length; i++) {
                var p = ar[i];
                if (p.StartsWith("fn=")) {
                    ar[i] = "fn=" + UrlEncode(p.Substring(3));
                    break;
                }
            }
            return arf[0] + "?" + String.Join("&", ar);
        }
        if (fn.Contains("/")) {
            path = Path.GetDirectoryName(fn)?.Replace("\\", "/") ?? "";
            filename = Path.GetFileName(fn);
        } else {
            path = "";
            filename = fn;
        }
        if (path != "")
            path = IncludeTrailingDelimiter(path, false);
        return path + UrlEncode(filename).Replace("+", " "); // Do not encode spaces
    }

    /// <summary>
    /// Check if absolute URL
    /// </summary>
    /// <param name="url">URL to check</param>
    /// <param name="uri">Output Uri</param>
    /// <returns>Uri instance</returns>
    public static bool IsAbsoluteUrl(string? url, out Uri? uri) => Uri.TryCreate(url, UriKind.Absolute, out uri);

    /// <summary>
    /// Check if absolute URL
    /// </summary>
    /// <param name="url">URL to check</param>
    /// <returns>Uri instance</returns>
    public static bool IsAbsoluteUrl(string? url) => Uri.TryCreate(url, UriKind.Absolute, out _);

    // URL Encode file path
    public static string UrlEncodeFilePath(string? path)
    {
        string scheme = IsAbsoluteUrl(path, out Uri? uri) ? uri?.Scheme ?? "" : "";
        var ar = (path ?? "").Split('/');
        for (var i = 0; i < ar.Length; i++) {
            if (ar[i] != scheme + ":")
                ar[i] = RawUrlEncode(ar[i]) ?? "";
        }
        return String.Join("/", ar);
    }

    // Get file view tag
    public static async Task<string> GetFileViewTag(DbField fld, string val, bool tooltip = false)
    {
        if (EmptyString(val))
            return "";
        string[] wrkfiles, wrknames;
        string tags = "";
        val = fld.HtmlDecode(val);
        if (fld.IsBlob) {
            wrknames = new string[] { val };
            wrkfiles = new string[] { val };
        } else if (fld.UploadMultiple) {
            wrknames = val.Split(Config.MultipleUploadSeparator);
            wrkfiles = fld.HtmlDecode(ConvertToString(fld.Upload.DbValue)).Split(Config.MultipleUploadSeparator);
        } else {
            wrknames = new string[] { val };
            wrkfiles = new string[] { fld.HtmlDecode(ConvertToString(fld.Upload.DbValue)) };
        }
        bool multiple = (wrkfiles.Length > 1);
        string href = tooltip ? "" : ConvertToString(fld.HrefValue).Trim();
        bool isLazy = tooltip ? false : IsLazy();
        dynamic page = CurrentPage;
        int wrkcnt = 0;
        bool showBase64Image = IsExport("html");
        bool skipImage = IsExport("excel") && !Config.UseExcelExtension || IsExport("word") && !Config.UseWordExtension;
        // Export image if:
        // - Excel + EPPlus extension / Word + Html2OpenXml extension / Pdf, or
        // - Non report + custom template + Pdf/Email
        bool showTempImage = IsExport("excel") && Config.UseExcelExtension ||
            IsExport("word") && Config.UseWordExtension ||
            IsExport("pdf") ||
            page.Type != "REPORT" && (IsExport("pdf") || IsExport("email"));
        foreach (string wrkfile in wrkfiles) {
            string fn = "", tag = "";
            if (showTempImage) {
                fn = await GetFileTempImage(fld, wrkfile);
            } else if (skipImage) {
                fn = "";
            } else {
                fn = GetFileUploadUrl(fld, wrkfile, fld.ImageResize);
            }
            if (fld.ViewTag == "IMAGE" && (fld.IsBlobImage || IsImageFile(wrkfile))) { // Image
                fld.ViewAttrs.AppendClass(fld.ImageCssClass);
                if (showBase64Image) {
                    tag = GetFileImgTag(new List<string> { await ImageFileToBase64Url(await GetFileTempImage(fld, wrkfile)) });
                } else {
                    if (IsLazy())
                        fld.ViewAttrs.AppendClass("ew-lazy");
                    if (Empty(href) && !fld.UseColorbox) {
                        if (!Empty(fn)) {
                            if (isLazy)
                                tag = "<img loading=\"lazy\" alt=\"\" src=\"data:image/png;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=\" data-src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + ">"; // DN
                            else
                                tag = "<img alt=\"\" src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + ">"; // DN
                        }
                    } else {
                        if (fld.UploadMultiple && href.Contains("%u"))
                            fld.HrefValue = AppPath(href.Replace("%u", GetFileUploadUrl(fld, wrkfile)));
                        if (!Empty(fn)) {
                            if (isLazy)
                                tag = "<a" + fld.LinkAttributes + "><img loading=\"lazy\" alt=\"\" src=\"data:image/png;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=\" data-src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + "></a>"; // DN
                            else
                                tag = "<a" + fld.LinkAttributes + "><img alt=\"\" src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + "></a>"; // DN
                        }
                    }
                }
            } else { // Non image
                string url = "", name = "", ext = "";
                if (fld.IsBlob) {
                    url = href;
                    name = !Empty(fld.Upload.FileName) ? fld.Upload.FileName : fld.Caption;
                    ext = fld.Upload.DbValue is byte[] b ? ContentExtension(b).Replace(".", "") : "";
                } else {
                    url = GetFileUploadUrl(fld, wrkfile);
                    name = (wrkcnt < wrknames.Length) ? wrknames[wrkcnt] : wrknames[wrknames.Length - 1];
                    ext = Path.GetExtension(wrkfile).Replace(".", "");
                }
                bool isPdf = SameText(ext, "pdf");
                if (!Empty(url)) {
                    fld.LinkAttrs.RemoveClass("ew-lightbox"); // Remove colorbox class
                    if (fld.UploadMultiple && href.Contains("%u"))
                        fld.HrefValue = AppPath(href.Replace("%u", url));
                    if (Config.EmbedPdf && isPdf) {
                        string pdfFile = fld.PhysicalUploadPath +  wrkfile;
                        tag = "<a" + fld.LinkAttributes + ">" + name + "</a>";
                        if (fld.IsBlob || IsRemote(pdfFile) || FileExists(pdfFile))
                            tag = "<div class=\"ew-pdfobject\" data-url=\"" + AppPath(url) + "\">" + tag + "</div>";
                    } else {
                        if (!Empty(ext))
                            fld.LinkAttrs["data-extension"] = ext;
                        tag = "<a" + fld.LinkAttributes + ">" + name + "</a>"; // DN
                    }
                }
            }
            if (!Empty(tag))
                tags += tag;
            wrkcnt++;
        }
        if (multiple && !Empty(tags))
            tags = "<div class=\"d-flex flex-row ew-images\">" + tags + "</div>";
        return tags;
    }

    // Get image view tag
    public static string GetImageViewTag(DbField fld, string val)
    {
        if (!EmptyString(val)) {
            string href = ConvertToString(fld.HrefValue);
            string image = val;
            string fn = val;
            if (val != "" && !val.Contains("://") && !val.Contains("\\") && !val.Contains("javascript:"))
                fn = GetImageUrl(fld, val, fld.ImageResize);
            if (IsImageFile(val)) { // Image
                fld.ViewAttrs.AppendClass(fld.ImageCssClass);
                if (IsLazy())
                    fld.ViewAttrs.AppendClass("ew-lazy");
                if (href == "" && !fld.UseColorbox) {
                    if (fn != "") {
                        if (IsLazy())
                            image = "<img loading=\"lazy\" alt=\"\" src=\"data:image/png;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=\" data-src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + ">";
                        else
                            image = "<img alt=\"\" src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + ">";
                    }
                } else {
                    if (fn != "") {
                        if (IsLazy())
                            image = "<a" + fld.LinkAttributes + "><img loading=\"lazy\" alt=\"\" src=\"data:image/png;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=\" data-src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + "></a>";
                        else
                            image = "<a" + fld.LinkAttributes + "><img alt=\"\" src=\"" + AppPath(fn) + "\"" + fld.ViewAttributes + "></a>";
                    }
                }
            } else {
                string name = val;
                if (href != "")
                    image = "<a" + fld.LinkAttributes + ">" + name + "</a>";
                else
                    image = name;
            }
            return image;
        }
        return "";
    }

    // Get image URL
    public static string GetImageUrl(DbField fld, string val, bool resize)
    {
        bool encrypt = Config.EncryptFilePath;
        if (!EmptyString(val)) {
            string fn;
            string key = Config.RandomKey + Session.SessionId;
            string fileUrl = Config.ApiUrl + Config.ApiFileAction;
            string path = (encrypt || resize) ? fld.PhysicalUploadPath : fld.HrefPath;
            if (encrypt) {
                fn = AppPath(fileUrl) + "/" + Encrypt(path + val, key) + "?session=" + Encrypt(Session.SessionId) + "&" + Config.TokenName + "=" + CurrentToken;
                if (resize)
                    fn += "&width=" + fld.ImageWidth + "&height=" + fld.ImageHeight;
            } else if (resize) {
                fn = AppPath(fileUrl) + "/" + Encrypt(path + val, key) + "?session=" + Encrypt(Session.SessionId) + "&" + Config.TokenName + "=" + CurrentToken +
                    "&width=" + fld.ImageWidth + "&height=" + fld.ImageHeight;
            } else {
                fn = IsRemote(path) ? path : UrlEncodeFilePath(path);
                fn += UrlEncodeFilePath(val);
            }
            return fn;
        }
        return "";
    }

    // Check if image file
    public static bool IsImageFile(string fn)
    {
        if (!Empty(fn)) {
            fn = ImageNameFromUrl(fn);
            var ext = Path.GetExtension(fn).Replace(".", "");
            return Config.ImageAllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }
        return false;
    }

    // Check if lazy loading images
    public static bool IsLazy() => Config.LazyLoad && (!IsExport() || IsExport("print"));

    // Get image file name from URL
    public static string ImageNameFromUrl(string fn)
    {
        if (!Empty(fn) && fn.Contains("?")) { // Thumbnail URL
            string? p = fn.Split('?')[1].Split('&').FirstOrDefault(x => x.StartsWith("fn="));
            if (p != null)
                return UrlDecode(p.Substring(3));
        }
        return fn;
    }

    /// <summary>
    /// Current page
    /// </summary>
    public static dynamic CurrentTable => CurrentPage;

    /// <summary>
    /// Current table name
    /// </summary>
    public static string CurrentTableName() => CurrentTable?.TableName ?? "";

    /// <summary>
    /// Get table name by table variable name
    /// </summary>
    /// <param name="tblVar">Table variable name</param>
    /// <returns>Table Name</returns>
    public static string GetTableName(string tblVar) =>
        Config.UserLevelTablePermissions.First(p => p.TableVar == tblVar)?.TableName ?? tblVar;

    // Static file options
    public static StaticFileOptions StaticFileSettings;

    // File extension content type provider
    public static FileExtensionContentTypeProvider ContentTypeProvider => (FileExtensionContentTypeProvider)StaticFileSettings.ContentTypeProvider;

    /// <summary>
    /// Get query builder
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="main">Whether to use the main connection</param>
    /// <returns>Query</returns>
    public static QueryBuilder? QueryBuilder(string table, bool main = false) => Resolve(table)?.GetQueryBuilder(main);

    /// <summary>
    /// Execute INSERT
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="data">Data (Anonymous type) to be inserted</param>
    /// <returns>The number of rows affected</returns>
    public static int ExecuteInsert(string table, object data) => QueryBuilder(table)?.Insert(data) ?? -1;

    /// <summary>
    /// Execute INSERT (async)
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="data">Data (Anonymous type) to be inserted</param>
    /// <returns>The number of rows affected</returns>
    public static Task<int> ExecuteInsertAsync(string table, object data) => QueryBuilder(table)?.InsertAsync(data) ?? Task.FromResult(-1);

    /// <summary>
    /// Execute UPDATE
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="where">Where constraint (Anonymous type)</param>
    /// <param name="data">Data (Anonymous type) to be updated</param>
    /// <returns>The number of rows affected</returns>
    public static int ExecuteUpdate(string table, object where, object data) => QueryBuilder(table)?.Where(where).Update(data) ?? -1;

    /// <summary>
    /// Execute UPDATE (async)
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="where">Where constraint (Anonymous type)</param>
    /// <param name="data">Data (Anonymous type) to be updated</param>
    /// <returns>The number of rows affected</returns>
    public static Task<int> ExecuteUpdateAsync(string table, object where, object data) => QueryBuilder(table)?.Where(where).UpdateAsync(data) ?? Task.FromResult(-1);

    /// <summary>
    /// Execute DELETE
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="where">Where constraint (Anonymous type)</param>
    /// <returns>The number of rows affected</returns>
    public static int ExecuteDelete(string table, object where) => QueryBuilder(table)?.Where(where).Delete() ?? -1;

    /// <summary>
    /// Execute DELETE (async)
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="where">Where constraint (Anonymous type)</param>
    /// <returns>The number of rows affected</returns>
    public static Task<int> ExecuteDeleteAsync(string table, object where) => QueryBuilder(table)?.Where(where).DeleteAsync() ?? Task.FromResult(-1);

    /// <summary>
    /// Execute SQL
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The number of rows affected</returns>
    public static int Execute(string sql, string dbid = "DB") => GetConnection2(dbid).ExecuteNonQuery(sql) ?? -1;

    /// <summary>
    /// Execute a command
    /// </summary>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The number of rows affected.</returns>
    public static int Execute(string sql, object param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, string dbid = "DB") =>
        GetConnection2(dbid).Execute(sql, param, transaction, commandTimeout, commandType) ?? -1;

    /// <summary>
    /// Execute SQL (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The number of rows affected</returns>
    public static async Task<int> ExecuteAsync(string sql, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteNonQueryAsync(sql) ?? -1;

    /// <summary>
    /// Execute a command (async)
    /// </summary>
    /// <param name="sql">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this query.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteAsync(sql, param, transaction, commandTimeout, commandType) ?? -1;

    /// <summary>
    /// Execute SQL and return first value of first row
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty.</returns>
    public static object ExecuteScalar(string sql, string dbid = "DB") =>
        GetConnection2(dbid).ExecuteScalar(sql);

    /// <summary>
    /// Execute parameterized SQL that selects a single value
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The first cell selected as <see cref="object"/>.</returns>
    public static object ExecuteScalar(string sql, object param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, string dbid = "DB") =>
        GetConnection2(dbid).ExecuteScalar(sql, param, transaction, commandTimeout, commandType);

    /// <summary>
    /// Execute SQL and return first value of first row (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty.</returns>
    public static async Task<object> ExecuteScalarAsync(string sql, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteScalarAsync(sql);

    /// <summary>
    /// Execute parameterized SQL that selects a single value (async)
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <param name="transaction">The transaction to use for this command.</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
    /// <param name="commandType">Is it a stored proc or a batch?</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The first cell selected as <see cref="object"/>.</returns>
    public static async Task<object> ExecuteScalarAsync(string sql, object param, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);

    /// <summary>
    /// Execute SQL and return first row as dictionary
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The row as dictionary</returns>
    public static Dictionary<string, object> ExecuteRow(string sql, string dbid = "DB") =>
        GetConnection2(dbid).GetRow(sql);

    /// <summary>
    /// Execute SQL and return first row as dictionary (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The row as dictionary</returns>
    public static async Task<Dictionary<string, object>> ExecuteRowAsync(string sql, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.GetRowAsync(sql);

    /// <summary>
    /// Execute SQL and return rows as list of dictionary
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The row as dictionary</returns>
    public static List<Dictionary<string, object>> ExecuteRows(string sql, string dbid = "DB") =>
        GetConnection2(dbid).GetRows(sql);

    /// <summary>
    /// Execute SQL and return rows as list of dictionary (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The row as dictionary</returns>
    public static async Task<List<Dictionary<string, object>>> ExecuteRowsAsync(string sql, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.GetRowsAsync(sql);

    /// <summary>
    /// Executes query and returns all rows as JSON
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="options">
    /// Options: (Dictionary or Anonymous Type)
    /// "header" (bool) Output JSON header, default: true
    /// "array" (bool) Output as array
    /// "firstonly" (bool) Output first row only
    /// </param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The records as JSON</returns>
    public static string ExecuteJson(string sql, dynamic? options = null, string dbid = "DB") =>
        GetConnection2(dbid).ExecuteJson(sql, options) ?? "null";

    /// <summary>
    /// Executes the query and returns all rows as JSON (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="options">
    /// Options: (Dictionary or Anonymous Type)
    /// "header" (bool) Output JSON header, default: true
    /// "array" (bool) Output as array
    /// "firstonly" (bool) Output first row only
    /// </param>
    /// <param name="dbid">Database ID</param>
    /// <returns>The records as JSON</returns>
    public static async Task<string> ExecuteJsonAsync(string sql, dynamic? options = null, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteJsonAsync(sql, options) ?? "null";

    /// <summary>
    /// Get query result in HTML table
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="options">
    /// Options: (Dictionary or Anonymous Type)
    /// "fieldcaption" (bool|Dictionary)
    ///   true: Use caption and use language object, or
    ///   false: Use field names directly, or
    ///   Dictionary of fieid caption for looking up field caption by field name
    /// "horizontal" (bool) Whether HTML table is horizontal, default: false
    /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
    /// "tableclass" (string) CSS class names of the table, default: "table table-bordered ew-db-table"
    /// </param>
    /// <param name="dbid"> Language object, default: the global Language object</param>
    /// <returns>The records as IHtmlContent</returns>
    public static IHtmlContent ExecuteHtml(string sql, dynamic? options = null, string dbid = "DB") =>
        GetConnection2(dbid).ExecuteHtml(sql, options);

    /// <summary>
    /// Get query result in HTML table (async)
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="options">
    /// Options: (Dictionary or Anonymous Type)
    /// "fieldcaption" (bool|Dictionary)
    ///   true: Use caption and use language object, or
    ///   false: Use field names directly, or
    ///   Dictionary of fieid caption for looking up field caption by field name
    /// "horizontal" (bool) Whether HTML table is horizontal, default: false
    /// "tablename" (string|List&lt;string&gt;) Table name(s) for the language object
    /// "tableclass" (string) CSS class names of the table, default: "table table-bordered ew-db-table"
    /// </param>
    /// <param name="dbid"> Language object, default: the global Language object</param>
    /// <returns>Tasks that returns records as IHtmlContent</returns>
    public static async Task<IHtmlContent> ExecuteHtmlAsync(string sql, dynamic? options = null, string dbid = "DB") =>
        await (await GetConnection2Async(dbid))?.ExecuteHtmlAsync(sql, options);

    /// <summary>
    /// CSS styles for exporting report // DN
    /// </summary>
    /// <returns>CSS styles</returns>
    public static string ExportStyles => SameText(ExportType, "pdf")
        ? LoadText(Config.PdfStylesheetFilename ?? Config.ProjectStylesheetFilename).GetAwaiter().GetResult()
        : LoadText(Config.ProjectStylesheetFilename).GetAwaiter().GetResult();

    /// <summary>
    /// Attributes for drill down
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="id">ID</param>
    /// <param name="hdr">Header</param>
    /// <param name="popover">Is popover</param>
    /// <returns>Attributes</returns>
    public static Dictionary<string, object> DrillDownAttributes(string url, string id, string hdr, bool popover = true) { // DN
        Dictionary<string, object> attrs = new ();
        if (Empty(url))
            return attrs;
        if (popover) {
            attrs.Add("data-ew-action", "drilldown");
            attrs.Add("data-url", url.Replace("&amp;", "&")); // Replace &amp; to &
            attrs.Add("data-id", id);
            attrs.Add("data-hdr", hdr);
        } else {
            attrs.Add("data-ew-action", "redirect");
            attrs.Add("data-url", url.Replace("?d=1&", "?d=2&")); // Change d parameter to 2
        }
        return attrs;
    }

    /// <summary>
    /// Annotation record struct // DN
    /// </summary>
    public record struct Annotation(
        double Value,
        double EndValue,
        string BorderColor,
        object Label,
        int BorderWidth,
        double Alpha,
        string ParentYAxis,
        string? ScaleID = null,
        double? XMin = null,
        double? XMax = null,
        double? YMin = null,
        double? YMax = null,
        bool Display = true,
        bool AdjustScaleRange = true,
        string DrawTime = "afterDatasetsDraw",
        string XScaleID = "x",
        string YScaleID = "y",
        int[]? BorderDash = null,
        int BorderDashOffset = 0,
        int Radius = 10, // Box/Point
        int Rotation = 0, // Ellipse
        string? BackgroundColor = null, // Box/Ellipse/Point
        string Type = "line" // Default as "line"
    );

    /// <summary>
    /// Get Opacity
    /// </summary>
    /// <param name="alpha">Alpha (0-100)</param>
    /// <returns>Opacity (0.0 - 1.0)</returns>
    public static double? GetOpacity(object alpha)
    {
        double? a = ConvertToNullableDouble(alpha);
        return a switch {
            null => null,
            > 100 => 1.0,
            <= 0 => 0.5, // Use default
            _ => a / 100
        };
    }

    /// <summary>
    /// Get Opacity
    /// </summary>
    /// <param name="color">Color</param>
    /// <param name="opacity">Opacity (0.0 - 1.0)</param>
    /// <returns>Rgba color</returns>
    public static string GetRgbaColor(string color, double? opacity = null)
    {
        // Check opacity
        if (opacity == null)
            return color;
        if (opacity > 1)
            opacity = 1.0;
        else if (opacity < 0)
            opacity = 0.0;

        // Check if color has 6 or 3 characters and get values
        var m = Regex.Match(color, @"^#?(\w{2})(\w{2})(\w{2})");
        string[]? hex = null;
        if (m.Success) { // 123456
            hex = new string[] { m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value };
        } else {
            m = Regex.Match(color, @"^#?(\w)(\w)(\w)");
            if (m.Success) // 123 => 112233
                hex = new string[] { m.Groups[1].Value + m.Groups[1].Value, m.Groups[2].Value + m.Groups[2].Value, m.Groups[3].Value + m.Groups[3].Value };
        }
        if (hex == null) // Unknown
            return color;

        // Convert hexadec to rgb
        var rgb = hex.Select(v => Convert.ToInt32(v, 16).ToString());

        // Check if opacity is set and return rgb(a) color string
        return "rgba(" + String.Join(",", rgb) + "," + opacity.ToString() + ")";
    }

    /// <summary>
    /// Dropdown display values
    /// </summary>
    /// <param name="v">Value</param>
    /// <param name="t">Type</param>
    /// <param name="fmt">Format</param>
    /// <returns>Display value</returns>
    public static string GetDropDownDisplayValue(object? v, string t, object fmt)
    {
        if (SameString(v, Config.NullValue))
            return Language.Phrase("NullLabel");
        else if (SameString(v, Config.EmptyValue))
            return Language.Phrase("EmptyLabel");
        else if (SameText(t, "boolean"))
            return BooleanName(v);
        string[] ar = ConvertToString(v).Split('|');
        return t.ToLower() switch {
            "y" or "year" => ConvertToString(v),
            "q" or "quarter" => ar.Length >= 2 ? QuarterName(ar[1]) + " " + ar[0] : ConvertToString(v),
            "m" or "month" => ar.Length >= 2 ? MonthName(ar[1]) + " " + ar[0] : ConvertToString(v),
            "w" or "week" => ar.Length >= 2 ? Language.Phrase("Week") + " " + ar[1] + " " + ar[0] : ConvertToString(v),
            "d" or "day" => ar.Length >= 3 ? FormatDateTime(new DateTime(ConvertToInt(ar[0]), ConvertToInt(ar[1]), ConvertToInt(ar[2])), fmt) : ConvertToString(v),
            "date" => IsDate(v) ? FormatDateTime(v, fmt) : ConvertToString(v),
            _ => ConvertToString(v)
        };
    }

    /// <summary>
    /// Get dropdown edit value
    /// </summary>
    /// <param name="fld">Field</param>
    /// <param name="v">Value</param>
    /// <returns>Edit value</returns>
    public static List<Dictionary<string, object>> GetDropDownEditValue(ReportField fld, object v) { // DN
        string val = ConvertToString(v).Trim();
        List<Dictionary<string, object>> list = new ();
        if (val != "") {
            string[] arwrk = fld.IsMultiSelect ? val.Split(',') : new string[] { val };
            foreach (string wrk in arwrk)
                list.Add(new () {
                    { "lf", wrk }, { "df", GetDropDownDisplayValue(wrk, (fld.DateFilter != "") ? fld.DateFilter : "date", fld.FormatPattern) }
                });
        }
        return list;
    }

    /// <summary>
    /// Get filter value for dropdown
    /// </summary>
    /// <param name="fld">Field</param>
    /// <param name="sep">Separator</param>
    /// <returns>Filter value</returns>
    public static string GetFilterDropDownValue(ReportField fld, string sep = ", ")
    {
        string value = "", result = "";
        if (!IsList(fld.AdvancedSearch.SearchValue)) {
            value = ConvertToString(fld.AdvancedSearch.SearchValue);
            if (SameString(value, Config.InitValue) || IsNull(value))
                result = (sep == ",") ? "" : Language.Phrase("PleaseSelect"); // Output empty string as value for input tag
        }
        return result;
    }

    /// <summary>
    /// Get Boolean Name
    /// Treat "T", "True", "Y", "Yes", "1" as True
    /// </summary>
    /// <param name="v">Value</param>
    /// <returns>Display value from language file</returns>
    public static string BooleanName(object? v)
    {
        if (IsNull(v))
            return Language.Phrase("NullLabel");
        else if (SameText(v, "true") || SameText(v, "yes") || SameText(v, "t") || SameText(v, "y") || SameText(v, "1"))
            return Language.Phrase("BooleanYes");
        return Language.Phrase("BooleanNo");
    }

    /// <summary>
    /// Quarter name
    /// </summary>
    /// <param name="q">Quarter (1-4)</param>
    /// <returns>Quarter string</returns>
    public static string QuarterName(object? q) => (Empty(q) || ConvertToInt(q) is int i && (i < 1 || i > 4))
        ? ConvertToString(q)
        : !Empty(Config.QuarterPattern) // Reserve for future use
        ? FormatDateTime(new DateTime(DateTime.Today.Year, i * 3, 1), Config.QuarterPattern)
        : Language.Phrase("Quarter").Replace("%q", i.ToString()); // Note: Add language phrase Quarter for multi language

    /// <summary>
    /// Month name
    /// </summary>
    /// <param name="m">Month (1-12)</param>
    /// <returns>Month string</returns>
    public static string MonthName(object? m) => !(Empty(m) || ConvertToInt(m) is int i && (i < 1 || i > 12))
        ? FormatDateTime(new DateTime(DateTime.Today.Year, i, 1), Config.MonthPattern)
        : ConvertToString(m);

    // Join List<string> // DN
    public static string JoinList(List<string> list, string sep, DataType fldType, string dbid = "DB") =>
        IsList(list) ? String.Join(sep, list.Select(str => QuotedValue(str, fldType, dbid))) : "";

    // Get current year
    public static int CurrentYear() => DateTime.Today.Year;

    // Get current quarter
    public static int CurrentQuarter() => Convert.ToInt32(Math.Ceiling(Convert.ToDouble(DateTime.Today.Month / 3)));

    // Get current month
    public static int CurrentMonth() => DateTime.Today.Month;

    // Get current day
    public static int CurrentDay() => DateTime.Today.Day;

    // Build Report SQL
    public static string BuildReportSql(string select, string where, string groupBy, string having, string orderBy, string filter, string sort)
    {
        string dbWhere = where;
        AddFilter(ref dbWhere, filter);
        string dbOrderBy = UpdateSortFields(orderBy, sort, 1);
        string sql = select;
        if (!Empty(dbWhere))
            sql += " WHERE " + dbWhere;
        if (!Empty(groupBy))
            sql += " GROUP BY " + groupBy;
        if (!Empty(having))
            sql += " HAVING " + having;
        if (!Empty(dbOrderBy))
            sql += " ORDER BY " + dbOrderBy;
        return sql;
    }

    /// <summary>
    /// Update sort fields
    /// </summary>
    /// <param name="orderBy">ORDER BY clause</param>
    /// <param name="sort">Sort clause</param>
    /// <param name="opt">Option: 1 = merge all sort fields, 2 = merge orderBy fields only</param>
    /// <returns>ORDER BY clause</returns>
    public static string UpdateSortFields(string orderBy, string sort, int opt)
    {
        if (Empty(orderBy)) {
            return (opt == 1) ? sort : "";
        } else if (Empty(sort)) {
            return orderBy;
        } else { // Merge sort field list
            List<string[]> orderBys = GetSortFields(orderBy);
            List<string[]> sortFields = GetSortFields(sort);
            foreach (var sortfld in sortFields) {
                // Get sort field
                string sortField = sortfld[0];
                string orderField = "";
                foreach (var orderfld in orderBys) {
                    // Replace field
                    orderField = orderfld[0];
                    if (SameString(orderField, sortField)) {
                        orderfld[1] = sortfld[1];
                        break;
                    }
                }

                // Append field
                if (opt == 1 && !SameString(orderField, sortField))
                    orderBys.Add(sortfld);
            }
            return String.Join(", ", orderBys.Select(fld => fld[0] + " " + fld[1]));
        }
    }

    // Get sort fields as array of [fieldName, sortDirection]
    public static List<string[]> GetSortFields(string flds)
    {
        List<string> fldList = new ();
        string temp = "";
        foreach (string fld in flds.Split(',')) {
            temp += fld;
            if (temp.Split('(').Length == temp.Split(')').Length) { // Make sure not inside parentheses
                fldList.Add(temp);
                temp = "";
            } else {
                temp += ",";
            }
        }
        return fldList.Select(fld => {
            string orderFld = fld.Trim();
            string orderType = "ASC";
            if (orderFld.ToUpper().EndsWith(" ASC")) {
                orderFld = orderFld.Substring(0, orderFld.Length - 4).Trim();
                orderType = "ASC";
            } else if (orderFld.ToUpper().EndsWith(" DESC")) {
                orderFld = orderFld.Substring(0, orderFld.Length - 5).Trim();
                orderType = "DESC";
            }
            return new [] { orderFld, orderType };
        }).ToList();
    }

    // Construct a crosstab field name
    public static string CrosstabFieldExpression(string smrytype, string smryfld, string colfld, string datetype, object? val, object qc, string alias = "", string dbid = "DB")
    {
        string fld = "", wrkval, wrkqc;
        if (SameString(val, Config.NullValue)) {
            wrkval = "NULL";
            wrkqc = "";
        } else if (SameString(val, Config.EmptyValue)) {
            wrkval = "";
            wrkqc = ConvertToString(qc);
        } else {
            wrkval = ConvertToString(val);
            wrkqc = ConvertToString(qc);
        }
        switch (smrytype) {
            case "SUM":
                fld = smrytype + "(" + smryfld + "*" + SqlDistinctFactor(colfld, datetype, wrkval, wrkqc, dbid) + ")";
                break;
            case "COUNT":
                fld = "SUM(" + SqlDistinctFactor(colfld, datetype, wrkval, wrkqc, dbid) + ")";
                break;
            case "MIN":
            case "MAX":
                string dbtype = GetConnectionType(dbid);
                string aggwrk = SqlDistinctFactor(colfld, datetype, wrkval, wrkqc, dbid);
                fld = smrytype + "(IF(" + aggwrk + "=0,NULL," + smryfld + "))";
                if (dbtype == "ACCESS") {
                    fld = smrytype + "(IIf(" + aggwrk + "=0,NULL," + smryfld + "))";
                } else if (dbtype == "MSSQL" || dbtype == "ORACLE" || dbtype == "SQLITE") {
                    fld = smrytype + "(CASE " + aggwrk + " WHEN 0 THEN NULL ELSE " + smryfld + " END)";
                } else if (dbtype == "MYSQL" || dbtype == "POSTGRESQL") {
                    fld = smrytype + "(IF(" + aggwrk + "=0,NULL," + smryfld + "))";
                }
                break;
            case "AVG":
                string sumwrk = "SUM(" + smryfld + "*" + SqlDistinctFactor(colfld, datetype, wrkval, wrkqc, dbid) + ")";
                if (!Empty(alias))
                    sumwrk += " AS " + QuotedName("sum_" + alias, dbid);
                string cntwrk = "SUM(" + SqlDistinctFactor(colfld, datetype, wrkval, wrkqc, dbid) + ")";
                if (!Empty(alias))
                    cntwrk += " AS " + QuotedName("cnt_" + alias, dbid);
                return sumwrk + ", " + cntwrk;
        }
        if (!Empty(alias))
            fld += " AS " + QuotedName(alias, dbid);
        return fld;
    }

    /// <summary>
    /// Construct SQL Distinct factor
    /// - ACCESS
    /// y: IIf(Year(FieldName)=1996,1,0)
    /// q: IIf(DatePart(""q"",FieldName,1,0)=1,1,0))
    /// m: (IIf(DatePart(""m"",FieldName,1,0)=1,1,0)))
    /// others: (IIf(FieldName=val,1,0)))
    /// - MSSQL
    /// y: (1-ABS(SIGN(Year(FieldName)-1996)))
    /// q: (1-ABS(SIGN(DatePart(q,FieldName)-1)))
    /// m: (1-ABS(SIGN(DatePart(m,FieldName)-1)))
    /// d: (CASE Convert(VarChar(10),FieldName,120) WHEN '1996-1-1' THEN 1 ELSE 0 END)
    /// - MySQL
    /// y: IF(YEAR(FieldName)=1996,1,0))
    /// q: IF(QUARTER(FieldName)=1,1,0))
    /// m: IF(MONTH(FieldName)=1,1,0))
    /// SQLITE
    /// y: (CASE CAST(STRFTIME('%Y',FieldName) AS INTEGER) WHEN 1996 THEN 1 ELSE 0 END)
    /// q: (CASE (CAST(STRFTIME('%m',FieldName) AS INTEGER)+2)/3 WHEN 1 THEN 1 ELSE 0 END)
    /// m: (CASE CAST(STRFTIME('%m',FieldName) AS INTEGER) WHEN 1 THEN 1 ELSE 0 END)
    /// - PostgreSQL
    /// y: IF(EXTRACT(YEAR FROM FieldName)=1996,1,0))
    /// q: IF(EXTRACT(QUARTER FROM FieldName)=1,1,0))
    /// m: IF(EXTRACT(MONTH FROM FieldName)=1,1,0))
    /// Oracle
    /// y: DECODE(TO_CHAR(FieldName,'YYYY'),'1996',1,0)
    /// q: DECODE(TO_CHAR(FieldName,'Q'),'1',1,0)
    /// m: DECODE(TO_CHAR(FieldName,'MM'),LPAD('1',2,'0'),1,0)
    /// </summary>
    /// <param name="fld">Field name</param>
    /// <param name="dateType">Date type</param>
    /// <param name="val">Value</param>
    /// <param name="qc">Quote charactor</param>
    /// <param name="dbid">Database ID</param>
    /// <returns></returns>
    public static string SqlDistinctFactor(string fld, string dateType, string val, string qc, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        if (dbtype == "ACCESS") {
            if (dateType == "y" && IsNumeric(val)) {
                return "IIf(Year(" + fld + ")=" + val + ",1,0)";
            } else if ((dateType == "q" || dateType == "m") && IsNumeric(val)) {
                return "IIf(DatePart(\"" + dateType + "\"," + fld + ")=" + val + ",1,0)";
            } else if (dateType == "d") {
                return "IIf(FORMAT(" + fld + ",'yyyy-mm-dd')=" + qc + AdjustSql(val, dbid) + qc + ",1,0)";
            } else if (dateType == "dt") {
                return "IIf(FORMAT(" + fld + ",'yyyy-mm-dd hh:nn:ss')=" + qc + AdjustSql(val, dbid) + qc + ",1,0)";
            } else {
                if (val == "NULL")
                    return "IIf(" + fld + " IS NULL,1,0)";
                else
                    return "IIf(" + fld + "=" + qc + AdjustSql(val, dbid) + qc + ",1,0)";
            }
        } else if (dbtype == "MSSQL") {
            if (dateType == "y" && IsNumeric(val)) {
                return "(1-ABS(SIGN(Year(" + fld + ")-" + val + ")))";
            } else if ((dateType == "q" || dateType == "m") && IsNumeric(val)) {
                return "(1-ABS(SIGN(DatePart(" + dateType + "," + fld + ")-" + val + ")))";
            } else if (dateType == "d") {
                return "(CASE FORMAT(" + fld + ",'yyyy-MM-dd') WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else if (dateType == "dt") {
                return "(CASE FORMAT(" + fld + ",'yyyy-MM-dd HH:mm:ss') WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else {
                if (val == "NULL")
                    return "(CASE WHEN " + fld + " IS NULL THEN 1 ELSE 0 END)";
                else
                    return "(CASE " + fld + " WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            }
        } else if (dbtype == "MYSQL") {
            if (dateType == "y" && IsNumeric(val)) {
                return "IF(YEAR(" + fld + ")=" + val + ",1,0)";
            } else if (dateType == "q" && IsNumeric(val)) {
                return "IF(QUARTER(" + fld + ")=" + val + ",1,0)";
            } else if (dateType == "m" && IsNumeric(val)) {
                return "IF(MONTH(" + fld + ")=" + val + ",1,0)";
            } else if (dateType == "d") {
                return "(CASE DATE_FORMAT(" + fld + ", '%Y-%m-%d') WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else if (dateType == "dt") {
                return "(CASE DATE_FORMAT(" + fld + ", '%Y-%m-%d %H:%i:%s') WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else {
                if (val == "NULL") {
                    return "IF(" + fld + " IS NULL,1,0)";
                } else {
                    return "IF(" + fld + "=" + qc + AdjustSql(val, dbid) + qc + ",1,0)";
                }
            }
        } else if (dbtype == "SQLITE") {
            if (dateType == "y" && IsNumeric(val)) {
                return "(CASE CAST(STRFTIME('%Y', " + fld + ") AS INTEGER) WHEN " + val + " THEN 1 ELSE 0 END)";
            } else if (dateType == "q" && IsNumeric(val)) {
                return "(CASE (CAST(STRFTIME('%m', " + fld + ") AS INTEGER)+2)/3 WHEN " + val + " THEN 1 ELSE 0 END)";
            } else if (dateType == "m" && IsNumeric(val)) {
                return "(CASE CAST(STRFTIME('%m', " + fld + ") AS INTEGER) WHEN " + val + " THEN 1 ELSE 0 END)";
            } else if (dateType == "d") {
                return "(CASE STRFTIME('%Y-%m-%d', " + fld + ") WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else if (dateType == "dt") {
                return "(CASE STRFTIME('%Y-%m-%d %H:%M:%S', " + fld + ") WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            } else {
                if (val == "NULL")
                    return "(CASE WHEN " + fld + " IS NULL THEN 1 ELSE 0 END)";
                else
                    return "(CASE " + fld + " WHEN " + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END)";
            }
        } else if (dbtype == "POSTGRESQL") {
            if (dateType == "y" && IsNumeric(val)) {
                return "CASE WHEN TO_CHAR(" + fld + ",'YYYY')='" + val + "' THEN 1 ELSE 0 END";
            } else if (dateType == "q" && IsNumeric(val)) {
                return "CASE WHEN TO_CHAR(" + fld + ",'Q')='" + val + "' THEN 1 ELSE 0 END";
            } else if (dateType == "m" && IsNumeric(val)) {
                return "CASE WHEN TO_CHAR(" + fld + ",'MM')=LPAD('" + val + "',2,'0') THEN 1 ELSE 0 END";
            } else if (dateType == "d") {
                return "CASE WHEN TO_CHAR(" + fld + ",'YYYY') || '-' || LPAD(TO_CHAR(" + fld + ",'MM'),2,'0') || '-' || LPAD(TO_CHAR(" + fld + ",'DD'),2,'0')='" + val + "' THEN 1 ELSE 0 END";
            } else if (dateType == "dt") {
                return "CASE WHEN TO_CHAR(" + fld + ",'YYYY') || '-' || LPAD(TO_CHAR(" + fld + ",'MM'),2,'0') || '-' || LPAD(TO_CHAR(" + fld + ",'DD'),2,'0') || ' ' || LPAD(TO_CHAR(" + fld + ",'HH24'),2,'0') || ':' || LPAD(TO_CHAR(" + fld + ",'MI'),2,'0') || ':' || LPAD(TO_CHAR(" + fld + ",'SS'),2,'0')='" + val + "' THEN 1 ELSE 0 END";
            } else {
                if (val == "NULL") {
                    return "CASE WHEN " + fld + " IS NULL THEN 1 ELSE 0 END";
                } else {
                    return "CASE WHEN " + fld + "=" + qc + AdjustSql(val, dbid) + qc + " THEN 1 ELSE 0 END";
                }
            }
        } else if (dbtype == "ORACLE") {
            if (dateType == "y" && IsNumeric(val)) {
                return "DECODE(TO_CHAR(" + fld + ",'YYYY'),'" + val + "',1,0)";
            } else if (dateType == "q" && IsNumeric(val)) {
                return "DECODE(TO_CHAR(" + fld + ",'Q'),'" + val + "',1,0)";
            } else if (dateType == "m" && IsNumeric(val)) {
                return "DECODE(TO_CHAR(" + fld + ",'MM'),LPAD('" + val + "',2,'0'),1,0)";
            } else if (dateType == "d") {
                val = FormatDateTime(val, 5, "en-US"); // DN
                return "DECODE(" + fld + ",TO_DATE(" + qc + AdjustSql(val, dbid) + qc + ",'YYYY-MM-DD'),1,0)";
            } else if (dateType == "dt") {
                val = FormatDateTime(val, 9, "en-US"); // DN
                return "DECODE(" + fld + ",TO_DATE(" + qc + AdjustSql(val, dbid) + qc + ",'YYYY-MM-DD HH24:MI:SS'),1,0)";
            } else {
                if (val == "NULL") {
                    return "(CASE WHEN " + fld + " IS NULL THEN 1 ELSE 0 END)";
                } else {
                    return "DECODE(" + fld + "," + qc + AdjustSql(val, dbid) + qc + ",1,0)";
                }
            }
        }
        return "";
    }

    // Evaluate summary value
    public static object? SummaryValue(object? val1, object? val2, string ityp)
    {
        switch (ityp) {
            case "SUM":
            case "COUNT":
            case "AVG":
                if (IsNullOrNotNumeric(val2))
                    return IsNullOrNotNumeric(val1) ? 0 : val1; // DN
                else
                    return ConvertToDouble(val1) + ConvertToDouble(val2);
            case "MIN":
                if (IsNullOrNotNumeric(val2))
                    return val1; // Skip null and non-numeric
                else if (IsNullOrNotNumeric(val1)) // Initialize for first valid value
                    return val2;
                else if (ConvertToDouble(val1) < ConvertToDouble(val2))
                    return val1;
                else
                    return val2;
            case "MAX":
                if (IsNullOrNotNumeric(val2))
                    return val1; // Skip null and non-numeric
                else if (IsNullOrNotNumeric(val1)) // Initialize for first valid value
                    return val2;
                else if (ConvertToDouble(val1) > ConvertToDouble(val2))
                    return val1;
                else
                    return val2;
        }
        return null;
    }

    /// <summary>
    /// Is Null or not Numeric
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>bool</returns>
    public static bool IsNullOrNotNumeric([NotNullWhen(false)] object? obj) => IsNull(obj) || !IsNumeric(obj);

    // Match filter value
    public static bool MatchedFilterValue(object obj, object value) =>
        (obj is List<string> list) ? list.Any(val => SameString(val, value)) : SameString(obj, value);

    /// <summary>
    /// Render repeat column table
    /// </summary>
    /// <param name="totalCount">Total count</param>
    /// <param name="rowCount">Zero based row count</param>
    /// <param name="repeatCount">Repeat count</param>
    /// <param name="renderType">Render type (1|2)</param>
    /// <returns></returns>
    public static string RepeatColumnTable(int totalCount, int rowCount, int repeatCount, int renderType)
    {
        string wrk = "";
        if (renderType == 1) { // Render control start
            if (rowCount == 0)
                wrk += "<table class=\"ew-item-table\">";
            if (rowCount % repeatCount == 0)
                wrk += "<tr>";
            wrk += "<td>";
        } else if (renderType == 2) { // Render control end
            wrk += "</td>";
            if (rowCount % repeatCount == repeatCount - 1) {
                wrk += "</tr>";
            } else if (rowCount == totalCount - 1) {
                for (int i = (rowCount % repeatCount) + 1; i < repeatCount; i++)
                    wrk += "<td>&nbsp;</td>";
                wrk += "</tr>";
            }
            if (rowCount == totalCount - 1)
                wrk += "</table>";
        }
        return wrk;
    }

    // Check if the value is selected
    public static bool IsSelectedValue(List<string> list, object value, int fldType)
    {
        if (list == null || !list.Any()) // DN
            return true;
        return list.Any(val => (ConvertToString(value).StartsWith("@@") || val.StartsWith("@@")) && SameString(val, value) || // Popup filters
            SameString(value, Config.NullValue) && SameString(value, val) ||
            CompareValue(val, value, fldType));
    }

    // Check if advanced filter value
    public static bool IsAdvancedFilterValue(StringValues sv) => sv.ToArray().All(v => v?.StartsWith("@@") ?? false);

    // Compare values based on field type
    public static bool CompareValue(object v1, object v2, int fldType)
    {
        switch (fldType) {
            case 20:
            case 3:
            case 2:
            case 16:
            case 17:
            case 18:
            case 19:
            case 21: // adBigInt, adInteger, adSmallInt, adTinyInt, adUnsignedTinyInt, adUnsignedSmallInt, adUnsignedInt, adUnsignedBigInt
                if (IsNumeric(v1) && IsNumeric(v2))
                    return ConvertToInt(v1) == ConvertToInt(v2);
                break;
            case 4:
            case 5:
            case 131:
            case 6: // adSingle, adDouble, adNumeric, adCurrency
                if (IsNumeric(v1) && IsNumeric(v2))
                    return ConvertToDouble(v1) == ConvertToDouble(v2);
                break;
            case 7:
            case 133:
            case 134:
            case 135: // adDate, adDBDate, adDBTime, adDBTimeStamp
                if (IsDate(v1) && IsDate(v2))
                    return Convert.ToDateTime(v1) == Convert.ToDateTime(v2);
                break;
            case 11:
                return ConvertToBool(v1) == ConvertToBool(v2);
        }
        return SameString(v1, v2); // Treat as string
    }

    // Register filter group
    public static void RegisterFilterGroup(ReportField fld, string groupName)
    {
        if (Config.ReportAdvancedFilters.TryGetValue(groupName, out Dictionary<string, string>? filters))
            foreach (var (id, methodName) in filters)
                RegisterFilter(fld, "@@" + id, Language.Phrase(id), methodName);
    }

    // Register filter
    public static void RegisterFilter(ReportField fld, string id, string name, string methodName = "")
    {
        string wrkid = id.StartsWith("@@") ? id : "@@" + id;
        fld.AdvancedFilters.Add(new (wrkid, name, methodName));
    }

    // Unregister filter
    public static void UnregisterFilter(ReportField fld, string id)
    {
        string wrkid = id.StartsWith("@@") ? id : "@@" + id;
        var filter = fld.AdvancedFilters.Single(f => f.ID == wrkid);
        if (filter != null)
            fld.AdvancedFilters.Remove(filter);
    }

    // Return date value
    public static object DateValue(string fldOpr, string fldVal, int valType, string dbid = "DB")
    {
        string wrkVal = "";
        string[] arWrk;
        int m, q, y;

        // Compose date string
        switch (fldOpr.ToLower()) {
            case "year":
                if (valType == 1)
                    wrkVal = fldVal + "-01-01";
                else if (valType == 2)
                    wrkVal = fldVal + "-12-31";
                break;
            case "quarter":
                arWrk = fldVal.Split('|');
                y = ConvertToInt(arWrk[0]);
                q = ConvertToInt(arWrk[1]);
                if (y == 0 || q == 0) {
                    wrkVal = "0000-00-00";
                } else {
                    if (valType == 1) {
                        m = (q - 1) * 3 + 1;
                        wrkVal = y + "-" + ConvertToString(m).PadLeft(2, '0') + "-01";
                    } else if (valType == 2) {
                        m = (q - 1) * 3 + 3;
                        wrkVal = y + "-" + ConvertToString(m).PadLeft(2, '0') + "-" + DaysInMonth(y, m);
                    }
                }
                break;
            case "month":
                arWrk = fldVal.Split('|');
                y = ConvertToInt(arWrk[0]);
                m = ConvertToInt(arWrk[1]);
                if (y == 0 || m == 0) {
                    wrkVal = "0000-00-00";
                } else {
                    if (valType == 1) {
                        wrkVal = y + "-" + ConvertToString(m).PadLeft(2, '0') + "-01";
                    } else if (valType == 2) {
                        wrkVal = y + "-" + ConvertToString(m).PadLeft(2, '0') + "-" + DaysInMonth(y, m);
                    }
                }
                break;
            case "day":
            default:
                wrkVal = fldVal.Replace("|", "-");
                wrkVal = Regex.Replace(wrkVal, @"\s+\d{2}\:\d{2}(\:\d{2})$", ""); // Remove trailing time
                break;
        }

        // Add time if necessary
        if (Regex.IsMatch(wrkVal, @"(\d{4}|\d{2})-(\d{1,2})-(\d{1,2})$")) { // Date without time
            if (valType == 1)
                wrkVal += " 00:00:00";
            else if (valType == 2)
                wrkVal += " 23:59:59";
        }

        // Check if datetime
        if (Regex.IsMatch(wrkVal, @"(\d{4}|\d{2})-(\d{1,2})-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})")) { // Date and time
            string dbType = GetConnectionType(dbid);
            return (!SameText(dbType, "MYSQL") && !SameText(dbType, "SQLITE")) ? wrkVal.Replace("-", "/") : wrkVal;
        }
        return "";
    }

    // Is past
    public static string IsPast(string fldExpression, string dbid = "DB")
    {
        string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string dbType = GetConnectionType(dbid);
        if (!SameText(dbType, "MYSQL") && !SameText(dbType, "SQLITE"))
            dt = dt.Replace("-", "/");
        return "(" + fldExpression + " < " + QuotedValue(dt, DataType.Date, dbid) + ")";
    }

    // Is future
    public static string IsFuture(string fldExpression, string dbid = "DB")
    {
        string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string dbType = GetConnectionType(dbid);
        if (!SameText(dbType, "MYSQL") && !SameText(dbType, "SQLITE"))
            dt = dt.Replace("-", "/");
        return "(" + fldExpression + " > " + QuotedValue(dt, DataType.Date, dbid) + ")";
    }

    /// <summary>
    /// Is between
    /// </summary>
    /// <param name="fldExpression">Field expression</param>
    /// <param name="dt1">Begin date (&gt;=)</param>
    /// <param name="dt2">End date (&lt;)</param>
    /// <param name="dbid">DB ID</param>
    /// <returns>WHERE clause</returns>
    public static string IsBetween(string fldExpression, string dt1, string dt2, string dbid = "DB")
    {
        string dbType = GetConnectionType(dbid);
        if (!SameText(dbType, "MYSQL") && !SameText(dbType, "SQLITE")) {
            dt1 = dt1.Replace("-", "/");
            dt2 = dt2.Replace("-", "/");
        }
        return "(" + fldExpression + " >= " + QuotedValue(dt1, DataType.Date, dbid) +
            " AND " + fldExpression + " < " + QuotedValue(dt2, DataType.Date, dbid) + ")";
    }

    // Is last 30 days
    public static string IsLast30Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddDays(-29);
        DateTime dt2 = DateTime.Today.AddDays(+1);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last 14 days
    public static string IsLast14Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddDays(-13);
        DateTime dt2 = DateTime.Today.AddDays(+1);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last 7 days
    public static string IsLast7Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddDays(-6);
        DateTime dt2 = DateTime.Today.AddDays(+1);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next 7 days
    public static string IsNext7Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddDays(+7);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next 14 days
    public static string IsNext14Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddDays(+14);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next 30 days
    public static string IsNext30Days(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddDays(+30);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is yesterday
    public static string IsYesterday(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddDays(-1);
        DateTime dt2 = DateTime.Today;
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is DT
    public static string IsToday(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddDays(+1);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is tomorrow
    public static string IsTomorrow(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddDays(+1);
        DateTime dt2 = DateTime.Today.AddDays(+2);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last month
    public static string IsLastMonth(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddMonths(-1);
        DateTime dt2 = DateTime.Today;
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is this month
    public static string IsThisMonth(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddMonths(+1);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next month
    public static string IsNextMonth(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddMonths(+1);
        DateTime dt2 = DateTime.Today.AddMonths(+2);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last 2 weeks
    public static string IsLast2Weeks(string fldExpression, string dbid = "DB")
    {
        DateTime dt = DateTime.Today;
        DateTime dt1 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek - 14);
        DateTime dt2 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last week
    public static string IsLastWeek(string fldExpression, string dbid = "DB")
    {
        DateTime dt = DateTime.Today;
        DateTime dt1 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek - 7);
        DateTime dt2 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is this week
    public static string IsThisWeek(string fldExpression, string dbid = "DB")
    {
        DateTime dt = DateTime.Today;
        DateTime dt1 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek);
        DateTime dt2 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek + 7);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next week
    public static string IsNextWeek(string fldExpression, string dbid = "DB")
    {
        DateTime dt = DateTime.Today;
        DateTime dt1 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek + 7);
        DateTime dt2 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek + 14);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next 2 weeks
    public static string IsNext2Weeks(string fldExpression, string dbid = "DB")
    {
        DateTime dt = DateTime.Today;
        DateTime dt1 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek + 7);
        DateTime dt2 = DateTime.Today.AddDays(-1 * (int)dt.DayOfWeek + 21);
        string sdt1 = dt1.ToString("yyyy-MM-dd");
        string sdt2 = dt2.ToString("yyyy-MM-dd");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is last year
    public static string IsLastYear(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddYears(-1);
        DateTime dt2 = DateTime.Today;
        string sdt1 = dt1.ToString("yyyy-01-01");
        string sdt2 = dt2.ToString("yyyy-01-01");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is this year
    public static string IsThisYear(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today;
        DateTime dt2 = DateTime.Today.AddYears(1);
        string sdt1 = dt1.ToString("yyyy-01-01");
        string sdt2 = dt2.ToString("yyyy-01-01");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Is next year
    public static string IsNextYear(string fldExpression, string dbid = "DB")
    {
        DateTime dt1 = DateTime.Today.AddYears(1);
        DateTime dt2 = DateTime.Today.AddYears(2);
        string sdt1 = dt1.ToString("yyyy-01-01");
        string sdt2 = dt2.ToString("yyyy-01-01");
        return IsBetween(fldExpression, sdt1, sdt2, dbid);
    }

    // Get number of days in a month
    public static int DaysInMonth(int y, int m) => DateTime.DaysInMonth(y, m);

    /// <summary>
    /// Get group value
    /// - Get the group value based on field type, group type and interval
    /// - fldType: field type
    /// * 1: numeric, 2: date, 3: string
    /// - gt: group type
    /// * numeric: i = interval, n = normal
    /// * date: d = Day, w = Week, m = Month, q = Quarter, y = Year
    /// * string: f = first nth character, n = normal
    /// - intv: interval
    /// </summary>
    /// <param name="fld">Field</param>
    /// <param name="val">Value</param>
    /// <returns></returns>
    public static object GroupValue(ReportField fld, object val)
    {
        var fldType = fld.Type;
        var grp = fld.GroupByType;
        var intv = fld.GroupInterval;
        switch (fldType) {
            // Case adBigInt, adInteger, adSmallInt, adTinyInt, adSingle, adDouble, adNumeric, adCurrency, adUnsignedTinyInt, adUnsignedSmallInt, adUnsignedInt, adUnsignedBigInt (numeric)
            case 20:
            case 3:
            case 2:
            case 16:
            case 4:
            case 5:
            case 131:
            case 6:
            case 17:
            case 18:
            case 19:
            case 21:
                if (!IsNumeric(val))
                    return val;
                int i = Convert.ToInt32(intv);
                if (i <= 0)
                    i = 10;
                return (grp == "i") ? Convert.ToInt64(Convert.ToDouble(val) / i) : val;
            // Case adDate, adDBDate, adDBTime, adDBTimeStamp (date) // DN
            case 7:
            case 133:
            case 134:
            case 135:
                if (!IsDate(val))
                    return val;
                var dt = Convert.ToDateTime(val);
                return grp switch {
                    "y" => dt.Year,
                    "q" => dt.Year + "|" + dt.Quarter().ToString(),
                    "m" => dt.Year + "|" + dt.Month.ToString("D2"),
                    "w" => dt.Year + "|" + dt.WeekOfYear().ToString("D2"),
                    "d" => dt.Year + "|" + dt.Month.ToString("D2") + "|" + dt.Day.ToString("D2"),
                    "h" => dt.Hour,
                    "min" => dt.Minute,
                    _ => val
                };
            // Case adLongVarChar, adLongVarWChar, adChar, adWChar, adVarChar, adVarWChar (string)
            case 201: // string
            case 203:
            case 129:
            case 130:
            case 200:
            case 202:
                int i2 = Convert.ToInt32(intv);
                if (i2 <= 0)
                    i2 = 1;
                string s = ConvertToString(val);
                return (grp == "f" && i2 <= s.Length) ? s.Substring(0, i2) : val; // DN
            default:
                return val; // ignore
        }
    }

    // Display group value
    public static string DisplayGroupValue(ReportField fld, object? val)
    {
        var fldType = fld.Type;
        var grp = fld.GroupByType;
        var intv = fld.GroupInterval;
        if (IsNull(val))
            return Language.Phrase("NullLabel");
        if (Empty(val))
            return Language.Phrase("EmptyLabel");
        switch (fldType) {
            // Case adBigInt, adInteger, adSmallInt, adTinyInt, adSingle, adDouble, adNumeric, adCurrency, adUnsignedTinyInt, adUnsignedSmallInt, adUnsignedInt, adUnsignedBigInt (numeric)
            case 20:
            case 3:
            case 2:
            case 16:
            case 4:
            case 5:
            case 131:
            case 6:
            case 17:
            case 18:
            case 19:
            case 21:
                var wrkIntv = Convert.ToInt32(intv);
                if (wrkIntv <= 0)
                    wrkIntv = 10;
                return (grp == "i")
                    ? ConvertToString(Convert.ToDouble(val) * wrkIntv) + " - " + ConvertToString((Convert.ToDouble(val) + 1) * wrkIntv - 1)
                    : ConvertToString(val);
            // Case adDate, adDBDate, adDBTime, adDBTimeStamp (date)
            case 7:
            case 133:
            case 134:
            case 135:
                var ar = ConvertToString(val).Split('|').Select(s => s.Trim()).ToArray();
                return grp switch {
                    "y" => ar[0],
                    "q" => (ar.Length < 2) ? ConvertToString(val) : FormatQuarter(ar[0], ar[1]),
                    "m" => (ar.Length < 2) ? ConvertToString(val) : FormatMonth(ar[0], ar[1]),
                    "w" => (ar.Length < 2) ? ConvertToString(val) : FormatWeek(ar[0], ar[1]),
                    "d" => (ar.Length < 3) ? ConvertToString(val) : FormatDay(ar[0], ar[1], ar[2]),
                    "h" => FormatHour(ar[0]),
                    "min" => FormatMinute(ar[0]),
                    _ => ConvertToString(val)
                };
            default: // String and others
                return ConvertToString(val); // Ignore
        }
    }

    // Format quarter
    public static Func<string, string, string> FormatQuarter = (string y, string q) => FormatDateTime(new DateTime(Convert.ToInt32(y), Convert.ToInt32(q) * 3, 1), "QQQ/yyyy");

    // Format month
    public static Func<string, string, string> FormatMonth = (string y, string m) => FormatDateTime(new DateTime(Convert.ToInt32(y), Convert.ToInt32(m), 1), "M/yyyy");

    // Format week
    public static Func<string, string, string> FormatWeek = (string y, string w) => (Language.Phrase("WeekShort") + w + CurrentDateTimeFormat.DateSeparator + y).Replace(LatinDigits, NativeDigits);

    // Format day
    public static Func<string, string, string, string> FormatDay = (string y, string m, string d) => FormatDateTime(new DateTime(Convert.ToInt32(y), Convert.ToInt32(m), Convert.ToInt32(d)), 0);

    // Format hour
    public static Func<string, string> FormatHour = (string h) => FormatDateTime(DateTime.Today.AddHours(Convert.ToInt32(h)), "htt");

    // Format minute
    public static Func<string, string> FormatMinute = (string min) => min.Replace(LatinDigits, NativeDigits) + Language.Phrase("Minute");

    // Return detail filter SQL
    public static string DetailFilterSql(ReportField? fld, string expr, object? val, string dbid = "DB")
    {
        if (fld == null || Empty(expr))
            return "";
        DataType fldType = fld.DataType;
        List<object?> list = IsList(val) ? (List<object?>)val : new () { val };
        if (!Empty(fld.GroupSql))
            fldType = DataType.String;
        return String.Join(" OR ", list.Select(val => expr + (IsNull(val) ? " IS NULL" : " = " + QuotedValue(val, fldType, dbid))));
    }

    // Return Advanced Filter SQL
    public static object AdvancedFilterSql(Dictionary<string, AdvancedFilter> dict, string fn, object val, string dbid = "DB")
    {
        if (dict == null || val == null)
            return "";
        foreach (var filter in dict.Values) {
            if (SameString(val, filter.ID) && filter.Enabled) // DN
                return Invoke(filter.MethodName, new object[] { fn, dbid }) ?? "";
        }
        return "";
    }

    // Match string lists as string
    public static bool SameLists(object list1, object list2)
    {
        if (!IsList(list1) && !IsList(list2))
            return true;
        else if (list1 is List<string> l1 && list2 is List<string> l2)
            return SameString(String.Join(",", l1.OrderBy(s => s)), String.Join(",", l2.OrderBy(s => s)));
        return false;
    }

    // Get base URL
    public static string BaseUrl() => Request != null
        ? String.Concat(Request.Scheme, "://", Request.Host.ToString(), Request.PathBase.ToString())
        : "";

    // Load drop down list
    public static void LoadDropDownList(List<string>? list, object val)
    {
        var ar = new List<string>(); // DN
        if (val is IEnumerable<string> values)
            ar.AddRange(values);
        else if (!SameString(val, Config.InitValue) && !SameString(val, Config.AllValue) && !Empty(val))
            ar.Add(ConvertToString(val));
        if (list != null)
            list.Clear();
        else
            list = new ();
        foreach (var v in ar) {
            if (v != Config.InitValue && !Empty(v) && !v.StartsWith("@@"))
                list.Add(v);
        }
    }

    // Get quick search keywords
    public static List<string> GetQuickSearchKeywords(string search, string searchType)
    {
        List<string> list = new ();
        if (!SameString(searchType, "=")) {
            // Match quoted keywords (i.e.: "...")
            int pos = 0; // DN
            foreach (Match match in Regex.Matches(search, @"""([^""]*)""", RegexOptions.IgnoreCase)) {
                int p = match.Index - pos; // DN
                pos = p + match.Length; // DN
                string str = search.Substring(0, p);
                search = search.Substring(p + match.Length);
                if (str.Trim().Length > 0)
                    list.AddRange(str.Trim().Split(' '));
                list.Add(match.Groups[1].Value); // Save quoted keyword
            }
            // Match individual keywords
            if (search.Trim().Length > 0)
                list.AddRange(search.Trim().Split(' '));
        } else {
            list.Add(search);
        }
        return list;
    }

    // Get quick search filter
    public static string GetQuickSearchFilter(List<DbField> flds, List<string> keywords, string searchType, bool searchAnyFields, string dbid = "DB")
    {
        // Search keyword in any fields
        string filter = "";
        if ((SameText(searchType, "OR") || SameText(searchType, "AND")) && searchAnyFields) {
            foreach (string keyword in keywords) {
                if (!Empty(keyword)) {
                    string thisFilter = "";
                    foreach (DbField fld in flds)
                        AddFilter(ref thisFilter, GetQuickSearchFilterForField(fld, new () { keyword }, searchType, dbid), "OR");
                    AddFilter(ref filter, thisFilter, searchType);
                }
            }
        } else {
            foreach (DbField fld in flds)
                AddFilter(ref filter, GetQuickSearchFilterForField(fld, keywords, searchType, dbid), "OR");
        }
        return filter;
    }

    // Get quick search filter for field
    public static string GetQuickSearchFilterForField(DbField fld, List<string> keywords, string searchType, string dbid)
    {
        string defCond = (searchType == "OR") ? "OR" : "AND";
        var sqls = new List<string>(); // List for SQL parts
        var conds = new List<string>(); // List for search conditions
        int cnt = keywords.Count;
        int j = 0; // Number of SQL parts
        for (int i = 0; i < cnt; i++) {
            string keyword = keywords[i];
            keyword = keyword.Trim();
            string[] ar;
            if (!Empty(Config.BasicSearchIgnorePattern)) {
                keyword = Regex.Replace(keyword, Config.BasicSearchIgnorePattern, "\\");
                ar = keyword.Split('\\');
            } else {
                ar = new string[] { keyword };
            }
            foreach (var kw in ar) {
                if (!Empty(kw)) {
                    string wrk = "";
                    if (kw == "OR" && searchType == "") {
                        if (j > 0)
                            conds[j - 1] = "OR";
                    } else if (kw == Config.NullValue) {
                        wrk = fld.Expression + " IS NULL";
                    } else if (kw == Config.NotNullValue) {
                        wrk = fld.Expression + " IS NOT NULL";
                    } else if (fld.IsVirtual && fld.Visible) {
                        wrk = fld.VirtualExpression + Like(QuotedValue(kw, DataType.String, dbid, "LIKE"), dbid);
                    } else if (fld.DataType != DataType.Number || IsNumeric(kw)) {
                        wrk = fld.BasicSearchExpression + Like(QuotedValue(kw, DataType.String, dbid, "LIKE"), dbid);
                    }
                    if (!Empty(wrk)) {
                        sqls.Add(wrk); // DN
                        conds.Add(defCond); // DN
                        j++;
                    }
                }
            }
        }
        cnt = sqls.Count;
        bool quoted = false;
        string sql = "";
        if (cnt > 0) {
            for (int i = 0; i < cnt - 1; i++) {
                if (conds[i] == "OR") {
                    if (!quoted)
                        sql += "(";
                    quoted = true;
                }
                sql += sqls[i];
                if (quoted && conds[i] != "OR") {
                    sql += ")";
                    quoted = false;
                }
                sql += " " + conds[i] + " ";
            }
            sql += sqls[cnt - 1];
            if (quoted)
                sql += ")";
        }
        return sql;
    }

    /// <summary>
    /// Get extended filter
    /// </summary>
    /// <param name="fld">Field object</param>
    /// <param name="def">Use default value</param>
    /// <param name="dbid">Database ID</param>
    /// <returns>Extended filter (WHERE clause)</returns>
    public static string GetExtendedFilter(ReportField fld, bool def = false, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        string fldName = fld.Name;
        string fldExpression = fld.Expression;
        DataType fldDataType = fld.DataType;
        string fldDateTimeFormat = ConvertToString(fld.DateTimeFormat); // DN
        object? fldVal = def ? fld.AdvancedSearch.SearchValueDefault : fld.AdvancedSearch.SearchValue;
        string fldOpr = def ? fld.AdvancedSearch.SearchOperatorDefault : fld.AdvancedSearch.SearchOperator;
        string fldCond = def ? fld.AdvancedSearch.SearchConditionDefault : fld.AdvancedSearch.SearchCondition;
        object? fldVal2 = def ? fld.AdvancedSearch.SearchValue2Default : fld.AdvancedSearch.SearchValue2;
        string fldOpr2 = def ? fld.AdvancedSearch.SearchOperator2Default : fld.AdvancedSearch.SearchOperator2;
        fldVal = ConvertSearchValue(fldVal, fldOpr, fld);
        fldVal2 = ConvertSearchValue(fldVal2, fldOpr2, fld);
        fldOpr = ConvertSearchOperator(fldOpr, fld, fldVal);
        fldOpr2 = ConvertSearchOperator(fldOpr2, fld, fldVal2);
        string wrk = "";
        if ((new[] { "BETWEEN", "NOT BETWEEN" }).Contains(fldOpr)) {
            bool isValidValue = fldDataType != DataType.Number || fld.VirtualSearch || IsNumericSearchValue(fldVal, fldOpr, fld) && IsNumericSearchValue(fldVal2, fldOpr2, fld);
            if (!Empty(fldVal) && !Empty(fldVal2) && isValidValue)
                wrk = fldExpression + " " + fldOpr + " " + QuotedValue(fldVal, fldDataType, dbid) +
                    " AND " + QuotedValue(fldVal2, fldDataType, dbid);
        } else {
            // Handle first value
            if (!Empty(fldVal) && IsValidOperator(fldOpr))
                wrk = SearchFilter(fldExpression, fldOpr, fldVal, fldDataType, dbid);
            // Handle second value
            string wrk2 = "";
            if (!Empty(fldVal2) && !Empty(fldOpr2) && IsValidOperator(fldOpr2))
                wrk2 = SearchFilter(fldExpression, fldOpr2, fldVal2, fldDataType, dbid);
            // Combine SQL
            AddFilter(ref wrk, wrk2, fldCond == "OR" ? "OR" : "AND");
        }
        return wrk;
    }

    // Return date search string
    public static string GetDateFilterSql(string fldExpr, string fldOpr, string fldVal, DataType fldType, string dbid = "DB")
    {
        if (fldOpr == "Year" && fldVal != "") { // Year filter
            return GroupSql(fldExpr, "y", 0, dbid) + " = " + fldVal;
        } else {
            var wrkVal1 = DateValue(fldOpr, fldVal, 1, dbid);
            var wrkVal2 = DateValue(fldOpr, fldVal, 2, dbid);
            if (!Empty(wrkVal1) && !Empty(wrkVal2))
                return fldExpr + " BETWEEN " + QuotedValue(wrkVal1, fldType, dbid) + " AND " + QuotedValue(wrkVal2, fldType, dbid);
            return "";
        }
    }

    // Group filter
    public static string GroupSql(string fldExpr, string grpType, int grpInt = 0, string dbid = "DB")
    {
        string dbtype = GetConnectionType(dbid);
        return grpType switch {
            "f" => // First n characters
                dbtype switch {
                    "ACCESS" => "MID(" + fldExpr + ",1," + grpInt + ")",
                    "MSSQL" => "SUBSTRING(" + fldExpr + ",1," + grpInt + ")",
                    "MYSQL" => "SUBSTRING(" + fldExpr + ",1," + grpInt + ")",
                    _ => "SUBSTR(" + fldExpr + ",1," + grpInt + ")" // SQLite / PostgreSQL / Oracle
                },
            "i" => // Interval
                dbtype switch {
                    "ACCESS" => "(" + fldExpr + "\\" + grpInt + ")",
                    "MSSQL" => "(" + fldExpr + "/" + grpInt + ")",
                    "MYSQL" => "(" + fldExpr + " DIV " + grpInt + ")",
                    "SQLITE" => "CAST(" + fldExpr + "/" + grpInt + " AS TEXT)",
                    "POSTGRESQL" => "(" + fldExpr + "/" + grpInt + ")",
                    _ => "FLOOR(" + fldExpr + "/" + grpInt + ")" // Oracle
                },
            "y" => // Year
                dbtype switch {
                    "ACCESS" => "YEAR(" + fldExpr + ")",
                    "MSSQL" => "YEAR(" + fldExpr + ")",
                    "MYSQL" => "YEAR(" + fldExpr + ")",
                    "SQLITE" => "CAST(STRFTIME('%Y'," + fldExpr + ") AS INTEGER)",
                    _ => "TO_CHAR(" + fldExpr + ",'YYYY')" // PostgreSQL / Oracle
                },
            "xq" => // Quarter
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'q')",
                    "MSSQL" => "DATEPART(QUARTER," + fldExpr + ")",
                    "MYSQL" => "QUARTER(" + fldExpr + ")",
                    "SQLITE" => "CAST(STRFTIME('%m'," + fldExpr + ") AS INTEGER)+2)/3",
                    _ => "TO_CHAR(" + fldExpr + ",'Q')" // PostgreSQL / Oracle
                },
            "q" => // Quarter (with year)
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'yyyy|q')",
                    "MSSQL" => "(STR(YEAR(" + fldExpr + "),4) + '|' + STR(DATEPART(QUARTER," + fldExpr + "),1))",
                    "MYSQL" => "CONCAT(CAST(YEAR(" + fldExpr + ") AS CHAR(4)), '|', CAST(QUARTER(" + fldExpr + ") AS CHAR(1)))",
                    "SQLITE" => "(CAST(STRFTIME('%Y'," + fldExpr + ") AS TEXT) || '|' || CAST((CAST(STRFTIME('%m'," + fldExpr + ") AS INTEGER)+2)/3 AS TEXT))",
                    _ => "(TO_CHAR(" + fldExpr + ",'YYYY') || '|' || TO_CHAR(" + fldExpr + ",'Q'))" // PostgreSQL / Oracle
                },
            "xm" => // Month
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'mm')",
                    "MSSQL" => "MONTH(" + fldExpr + ")",
                    "MYSQL" => "MONTH(" + fldExpr + ")",
                    "SQLITE" => "CAST(STRFTIME('%m'," + fldExpr + ") AS INTEGER)",
                    _ => "TO_CHAR(" + fldExpr + ",'MM')" // PostgreSQL / Oracle
                },
            "m" => // Month (with year)
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'yyyy|mm')",
                    "MSSQL" => "(STR(YEAR(" + fldExpr + "),4) + '|' + REPLACE(STR(MONTH(" + fldExpr + "),2,0),' ','0'))",
                    "MYSQL" => "CONCAT(CAST(YEAR(" + fldExpr + ") AS CHAR(4)), '|', CAST(LPAD(MONTH(" + fldExpr + "),2,'0') AS CHAR(2)))",
                    "SQLITE" => "CAST(STRFTIME('%Y|%m'," + fldExpr + ") AS TEXT)",
                    _ => "(TO_CHAR(" + fldExpr + ",'YYYY') || '|' || TO_CHAR(" + fldExpr + ",'MM'))" // PostgreSQL / Oracle
                },
            "w" =>
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'yyyy|ww')",
                    "MSSQL" => "(STR(YEAR(" + fldExpr + "),4) + '|' + REPLACE(STR(DATEPART(WEEK," + fldExpr + "),2,0),' ','0'))",
                    "MYSQL" => "CONCAT(CAST(YEAR(" + fldExpr + ") AS CHAR(4)), '|', CAST(LPAD(WEEK(" + fldExpr + ",0),2,'0') AS CHAR(2)))",
                    "SQLITE" => "CAST(STRFTIME('%Y|%W'," + fldExpr + ") AS TEXT)",
                    _ => "(TO_CHAR(" + fldExpr + ",'YYYY') || '|' || TO_CHAR(" + fldExpr + ",'WW'))"
                },
            "d" =>
                dbtype switch {
                    "ACCESS" => "FORMAT(" + fldExpr + ", 'yyyy|mm|dd')",
                    "MSSQL" => "(STR(YEAR(" + fldExpr + "),4) + '|' + REPLACE(STR(MONTH(" + fldExpr + "),2,0),' ','0') + '|' + REPLACE(STR(DAY(" + fldExpr + "),2,0),' ','0'))",
                    "MYSQL" => "CONCAT(CAST(YEAR(" + fldExpr + ") AS CHAR(4)), '|', CAST(LPAD(MONTH(" + fldExpr + "),2,'0') AS CHAR(2)), '|', CAST(LPAD(DAY(" + fldExpr + "),2,'0') AS CHAR(2)))",
                    "SQLITE" => "CAST(STRFTIME('%Y|%m|%d'," + fldExpr + ") AS TEXT)",
                    _ => "(TO_CHAR(" + fldExpr + ",'YYYY') || '|' || LPAD(TO_CHAR(" + fldExpr + ",'MM'),2,'0') || '|' || LPAD(TO_CHAR(" + fldExpr + ",'DD'),2,'0'))"
                },
            "h" =>
                dbtype switch {
                    "ACCESS" => "HOUR(" + fldExpr + ")",
                    "MSSQL" => "HOUR(" + fldExpr + ")",
                    "MYSQL" => "HOUR(" + fldExpr + ")",
                    "SQLITE" => "CAST(STRFTIME('%H'," + fldExpr + ") AS INTEGER)",
                    _ => "TO_CHAR(" + fldExpr + ",'HH24')"
                },
            "min" =>
                dbtype switch {
                    "ACCESS" => "MINUTE(" + fldExpr + ")",
                    "MSSQL" => "MINUTE(" + fldExpr + ")",
                    "MYSQL" => "MINUTE(" + fldExpr + ")",
                    "SQLITE" => "CAST(STRFTIME('%M'," + fldExpr + ") AS INTEGER)",
                    _ => "TO_CHAR(" + fldExpr + ",'MI')"
                },
            _ => ""
        };
    }

    // Check HTML for export
    public static string CheckHtml(string html)
    {
        string p1 = "class=\"ew-table\"", p2 = " data-page-break=\"before\"";
        string p = Regex.Escape(p1) + "|" + Regex.Escape(p2) + "|" + Regex.Escape(Config.PageBreakHtml);
        foreach (Match match in Regex.Matches(html, p)) {
            if (match.Value == p1) { // If table, break
                break;
            } else if (match.Value == Config.PageBreakHtml) { // If page breaks (no table before), remove and continue
                html = (new Regex(Regex.Escape(match.Value))).Replace(html, "", 1);
                continue;
            } else if (match.Value == p2) { // If page breaks (no table before), remove and break
                html = (new Regex(Regex.Escape(match.Value))).Replace(html, "", 1);
                break;
            }
        }
        return html;
    }

    // Get file IMG tag (overload)
    public static string GetFileImgTag(ReportField fld, string fn)
    {
        if (!Empty(fn)) {
            if (fld.DataType != DataType.Blob) {
                var wrkfiles = fn.Split(Config.MultipleUploadSeparator);
                return String.Join("<br>", wrkfiles.Select(wrkfile => "<img src=\"" + wrkfile + "\" alt=\"\">"));
            } else {
                return "<img src=\"" + fn + "\" alt=\"\">";
            }
        }
        return "";
    }

    // Get file temp image (overload)
    public static async Task<string> GetFileTempImage(ReportField fld, string val)
    {
        if (fld.DataType == DataType.Blob) {
            if (!Empty(fld.DbValue)) {
                byte[] tmpimage = (byte[])fld.DbValue;
                if (fld.ImageResize)
                    ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                return await TempImage(tmpimage);
            }
        } else {
            if (!Empty(val)) {
                var files = val.Split(Config.MultipleUploadSeparator);
                string images = "";
                for (var i = 0; i < files.Count(); i++) {
                    if (files[i] != "") {
                        var tmpimage = await FileReadAllBytes(fld.PhysicalUploadPath + files[i]);
                        if (fld.ImageResize)
                            ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                        if (images != "")
                            images += Config.MultipleUploadSeparator;
                        images += await TempImage(tmpimage);
                    }
                }
                return String.Join(Config.MultipleUploadSeparator, files.Where(file => file != "").Select(async file => {
                    var tmpimage = await FileReadAllBytes(fld.PhysicalUploadPath + file);
                    if (fld.ImageResize)
                        ResizeBinary(ref tmpimage, ref fld.ImageWidth, ref fld.ImageHeight);
                    return await TempImage(tmpimage);
                }));
            }
        }
        return "";
    }

    /// <summary>
    /// Merge source dictionary to target dictionary
    /// </summary>
    /// <param name="to">Target dictionary</param>
    /// <param name="from">Source dictionary</param>
    /// <returns>New dictionary</returns>
    public static Dictionary<string, dynamic?> Merge(IDictionary<string, dynamic?> to, IDictionary<string, dynamic?>? from)
    {
        if (from != null) {
            foreach (var (k, v) in from) {
                to[k] = to.TryGetValue(k, out dynamic? d1) &&
                    (d1 is IDictionary<string, dynamic?> || IsAnonymousType(d1)) &&
                    (v is IDictionary<string, dynamic?> || IsAnonymousType(v))
                    ? Merge(d1, v)
                    : v;
            }
        }
        return (Dictionary<string, dynamic?>)to;
    }

    /// <summary>
    /// Merge source dictionary to target dictionary
    /// </summary>
    /// <param name="to">Target anonymous object</param>
    /// <param name="from">Source anonymous object</param>
    /// <returns>New dictionary</returns>
    public static Dictionary<string, dynamic?> Merge(object to, object? from)
    {
        if (IsAnonymousType(to))
            to = ConvertToDictionary<dynamic?>(to);
        if (IsAnonymousType(from))
            from = ConvertToDictionary<dynamic?>(from);
        return Merge((IDictionary<string, dynamic?>)to, (IDictionary<string, dynamic?>?)from);
    }

    // Captcha
    protected static ICaptcha? _captcha = null;

    // Captcha instance
    public static ICaptcha CurrentCaptcha => (_captcha ??= CreateInstance(Config.CaptchaClass))!;

    // Two Factor Authentication instance and class
    protected static ITwoFactorAuthentication? _2fa = null;

    public static ITwoFactorAuthentication Current2FA => (_2fa ??= CreateInstance(TwoFactorAuthenticationClass))!;

    public static string TwoFactorAuthenticationClass => Config.TwoFactorAuthenticationType
        switch {
            "email" => "EmailTwoFactorAuthentication",
            "sms" => "SmsTwoFactorAuthentication",
            _ => "GoogleTwoFactorAuthentication"
        };

    // Conn
    public static dynamic Conn
    {
        get => HttpData.Get<dynamic>("CONN")!;
        set => HttpData["CONN"] = value;
    }

    // Security
    public static AdvancedSecurity Security
    {
        get => HttpData.Get<AdvancedSecurity>("SECURITY")!;
        set => HttpData["SECURITY"] = value;
    }

    // Current Form
    public static HttpForm CurrentForm
    {
        get => HttpData.Get<HttpForm>("CURRENT_FORM")!;
        set => HttpData["CURRENT_FORM"] = value;
    }

    // Language
    public static Lang Language
    {
        get => HttpData.Get<Lang>("LANGUAGE")!;
        set => HttpData["LANGUAGE"] = value;
    }

    // Current Breadcrumb
    public static Breadcrumb CurrentBreadcrumb
    {
        get => HttpData.Get<Breadcrumb>("CURRENT_BREADCRUMB")!;
        set => HttpData["CURRENT_BREADCRUMB"] = value;
    }

    // Top Menu
    public static Task<string> TopMenu
    {
        get => HttpData.Get<Task<string>>("TOP_MENU")!;
        set => HttpData["TOP_MENU"] = value;
    }

    // Side Menu
    public static Task<string> SideMenu
    {
        get => HttpData.Get<Task<string>>("SIDE_MENU")!;
        set => HttpData["SIDE_MENU"] = value;
    }

    // Current Language
    public static string CurrentLanguage
    {
        get => HttpData.Get<string>("CURRENT_LANGUAGE")!;
        set => HttpData["CURRENT_LANGUAGE"] = value;
    }

    // Skip Header Footer
    public static bool SkipHeaderFooter
    {
        get => HttpData.Get<bool>("SKIP_HEADER_FOOTER")!;
        set => HttpData["SKIP_HEADER_FOOTER"] = value;
    }

    // Start Time
    public static long StartTime
    {
        get => HttpData.Get<long>("START_TIME")!;
        set => HttpData["START_TIME"] = value;
    }

    // Current Page
    public static dynamic CurrentPage
    {
        get => HttpData.Get<dynamic>("CURRENT_PAGE")!;
        set => HttpData["CURRENT_PAGE"] = value;
    }

    // Current Grid
    public static dynamic CurrentGrid
    {
        get => HttpData.Get<dynamic>("CURRENT_GRID")!;
        set => HttpData["CURRENT_GRID"] = value;
    }

    // Export Type
    public static string ExportType
    {
        get => HttpData.Get<string>("EXPORT_TYPE")!;
        set => HttpData["EXPORT_TYPE"] = value;
    }

    // Export Id
    public static string ExportId
    {
        get => HttpData.Get<string>("EXPORT_ID")!;
        set => HttpData["EXPORT_ID"] = value;
    }

    // Download File Name
    public static string DownloadFileName
    {
        get => HttpData.Get<string>("DOWNLOAD_FILE_NAME")!;
        set => HttpData["DOWNLOAD_FILE_NAME"] = value;
    }

    // Client Send Error
    public static string ClientSendError
    {
        get => HttpData.Get<string>("CLIENT_SEND_ERROR")!;
        set => HttpData["CLIENT_SEND_ERROR"] = value;
    }

    // Debug Message
    public static string DebugMessage
    {
        get => HttpData.Get<string>("DEBUG_MESSAGE")!;
        set => HttpData["DEBUG_MESSAGE"] = value;
    }

    // Current Token
    public static string CurrentToken
    {
        get => HttpData.Get<string>("CURRENT_TOKEN")!;
        set => HttpData["CURRENT_TOKEN"] = value;
    }

    // Temp Images
    public static List<string> TempImages
    {
        get => HttpData.Resolve<List<string>>("TEMP_IMAGES");
        set => HttpData["TEMP_IMAGES"] = value;
    }

    // Current Number Format
    public static NumberFormatInfo CurrentNumberFormat
    {
        get => HttpData.Get<NumberFormatInfo>("CURRENT_NUMBER_FORMAT")!;
        set => HttpData["CURRENT_NUMBER_FORMAT"] = value;
    }

    // Current Date Time Format
    public static DateTimeFormatInfo CurrentDateTimeFormat
    {
        get => HttpData.Get<DateTimeFormatInfo>("CURRENT_DATE_TIME_FORMAT")!;
        set => HttpData["CURRENT_DATE_TIME_FORMAT"] = value;
    }

    // Currency Format
    public static string CurrencyFormat
    {
        get => HttpData.Get<string>("CURRENCY_FORMAT")!;
        set => HttpData["CURRENCY_FORMAT"] = value;
    }

    // Number Format
    public static string NumberFormat
    {
        get => HttpData.Get<string>("NUMBER_FORMAT")!;
        set => HttpData["NUMBER_FORMAT"] = value;
    }

    // Percent Format
    public static string PercentFormat
    {
        get => HttpData.Get<string>("PERCENT_FORMAT")!;
        set => HttpData["PERCENT_FORMAT"] = value;
    }

    // Numbering System
    public static string NumberingSystem
    {
        get => HttpData.Get<string>("NUMBERING_SYSTEM")!;
        set => HttpData["NUMBERING_SYSTEM"] = value;
    }

    // Client Variables
    public static Dictionary<string, object> ClientVariables
    {
        get => HttpData.Resolve<Dictionary<string, object>>("CLIENT_VARIABLES");
        set => HttpData["CLIENT_VARIABLES"] = value;
    }

    // Login Status
    public static Dictionary<string, object> LoginStatus
    {
        get => HttpData.Resolve<Dictionary<string, object>>("LOGIN_STATUS");
        set => HttpData["LOGIN_STATUS"] = value;
    }

    // Profile
    public static UserProfile Profile
    {
        get => HttpData.Get<UserProfile>("PROFILE")!;
        set => HttpData["PROFILE"] = value;
    }

    // Is Drill Down In Panel
    public static bool IsDrillDownInPanel
    {
        get => HttpData.Get<bool>("IS_DRILL_DOWN_IN_PANEL")!;
        set => HttpData["IS_DRILL_DOWN_IN_PANEL"] = value;
    }

    // Dashboard Report
    public static bool DashboardReport
    {
        get => HttpData.Get<bool>("DASHBOARD_REPORT")!;
        set => HttpData["DASHBOARD_REPORT"] = value;
    }

    // Dashboard Page
    public static dynamic DashboardPage
    {
        get => HttpData.Get<dynamic>("DASHBOARD_PAGE")!;
        set => HttpData["DASHBOARD_PAGE"] = value;
    }

    // Chart
    public static DbChart Chart
    {
        get => HttpData.Get<DbChart>("CHART")!;
        set => HttpData["CHART"] = value;
    }

    // Report Options
    public static Dictionary<string, object> ReportOptions
    {
        get => HttpData.Resolve<Dictionary<string, object>>("REPORT_OPTIONS");
        set => HttpData["REPORT_OPTIONS"] = value;
    }

    // Report Parameters
    public static Dictionary<string, string> ReportParameters
    {
        get => HttpData.Resolve<Dictionary<string, string>>("REPORT_PARAMETERS");
        set => HttpData["REPORT_PARAMETERS"] = value;
    }

    // Report Table Class
    public static string ReportTableClass
    {
        get => HttpData.Get<string>("REPORT_TABLE_CLASS")!;
        set => HttpData["REPORT_TABLE_CLASS"] = value;
    }
}
