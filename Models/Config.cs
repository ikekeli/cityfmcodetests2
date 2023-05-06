namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    // Configuration
    public static partial class Config
    {
        // Init
        public static void Init()
        {
            // Authentications
            Authentications = new () {
                {"Google", new () {
                    Enabled = false,
                    Id = Configuration["Google:Id"] ?? "",
                    Color = Configuration["Google:Color"] ?? "",
                    Secret = Configuration["Google:Secret"] ?? ""
                }},
                {"Facebook", new () {
                    Enabled = false,
                    Id = Configuration["Facebook:Id"] ?? "",
                    Color = Configuration["Facebook:Color"] ?? "",
                    Secret = Configuration["Facebook:Secret"] ?? ""
                }},
                {"Microsoft", new () { // Use "Microsoft" as key, not "Azure"
                    Enabled = false,
                    Id = Configuration["Azure:Id"] ?? "",
                    Color = Configuration["Azure:Color"] ?? "",
                    Secret = Configuration["Azure:Secret"] ?? ""
                }},
                {"Saml", new () {
                    Enabled = false,
                    Color = Configuration["Saml:Color"] ?? "",
                }}
            }; // DN

            // SMTP server
            SmtpServer = Configuration["Smtp:Server"] ?? ""; // SMTP server
            SmtpServerPort = ConvertToInt(Configuration["Smtp:Port"]); // SMTP server port
            SmtpSecureOption = Configuration["Smtp:SecureOption"] ?? "None"; // Default is None
            SmtpServerUsername = Configuration["Smtp:Username"] ?? ""; // SMTP server user name
            SmtpServerPassword = Configuration["Smtp:Password"] ?? ""; // SMTP server password

            // Config Init event
            ConfigInit();
        }

        // Config Init

        // Config Init event
        public static void ConfigInit() {
            // Enter your code here
        }

        // Debug
        public static bool Debug { get; set; } = false;

        public static string DebugMessageTemplate { get; set; } = @"<div class=""card card-danger ew-debug""><div class=""card-header""><h3 class=""card-title"">%t</h3><div class=""card-tools""><button type=""button"" class=""btn btn-tool"" data-card-widget=""collapse""><i class=""fa-solid fa-minus""></i></button></div></div><div class=""card-body"">%s</div></div>";

        // Log SQL to file
        public static bool LogSql = false;

        // Product version
        public const string ProductVersion = "20.2.0";

        // Project
        public const string ProjectNamespace = "Zaharuddin";

        public const string ProjectClassName = "Zaharuddin.Models.cityfmcodetests"; // DN

        public static string PathDelimiter = ConvertToString(Path.DirectorySeparatorChar); // Physical path delimiter // DN

        public static short UnformatYear = 50; // Unformat year

        public const string ProjectName = "cityfmcodetests"; // Project name

        public static string ControllerName { get; set; } = "Home"; // Controller name // DN

        public const string ProjectId = "{C1706DD2-6C6C-44D6-BA9D-C75600D81EB6}"; // Project ID (GUID)

        public static string RandomKey = "rRE3sCzJBzfdbBa9"; // Random key for encryption

        public static string EncryptionKey = ""; // Encryption key for data protection

        public static string ProjectStylesheetFilename = "css/cityfmcodetests.css"; // Project stylesheet file name (relative to wwwroot)

        public static bool UseCompressedStylesheet = false; // Compressed stylesheet

        public static string FontAwesomeStylesheet = "plugins/fontawesome-free/css/all.min.css"; // Font Awesome Free stylesheet

        public static string Charset = "utf-8"; // Project charset

        public static string EmailCharset = Charset; // Email charset

        public static string EmailKeywordSeparator = ""; // Email keyword separator

        public static string CompositeKeySeparator = ","; // Composite key separator

        public static Dictionary<string, string> ExportTableCellSyles = new() // Export table cell CSS styles, use inline style for Gmail
        {
            { "border", "1px solid #dddddd" },
            { "padding", "5px" }
        };

        public static bool HighlightCompare { get; set; } = true; // Case-insensitive

        public static int FontSize = 14;

        public static bool Cache = false; // Cache // DN

        public static bool LazyLoad = true; // Lazy loading of images

        public static string RelatedProjectId = "";

        public static bool CheckOldUserLevels = false; // Check old Dynamic User Level Security settings

        public static bool DeleteUploadFiles = false; // Delete uploaded file on deleting record

        public static string FileNotFound = "/9j/4AAQSkZJRgABAQAAAQABAAD/7QAuUGhvdG9zaG9wIDMuMAA4QklNBAQAAAAAABIcAigADEZpbGVOb3RGb3VuZAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wgARCAABAAEDAREAAhEBAxEB/8QAFAABAAAAAAAAAAAAAAAAAAAACP/EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhADEAAAAD+f/8QAFBABAAAAAAAAAAAAAAAAAAAAAP/aAAgBAQABPwB//8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAgBAgEBPwB//8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAgBAwEBPwB//9k="; // 1x1 jpeg with IPTC data "2#040"="FileNotFound"

        public static string BodyClass = "hold-transition layout-fixed layout-navbar-fixed layout-footer-fixed"; // CSS class(es) for <body> tag

        public static string BodyStyle = ""; // CSS style for <body> tag

        public static string SidebarClass = "main-sidebar sidebar-dark-navy"; // CSS class(es) for sidebar

        public static string NavbarClass = "main-header navbar navbar-expand navbar-navy navbar-dark border-bottom-0"; // CSS class(es) for navbar

        public static string ClassPrefix = "_"; // Prefix for invalid CSS class names

        public static bool UseJavascriptMessage = true; // Use JavaScript message (toast)

        // External JavaScripts
        public static List<string> JavaScriptFiles = new ()
        {
        };

        // External StyleSheets
        public static List<string> StylesheetFiles = new ()
        {
        };

        // Authentication configuration for Google/Facebook
        public static Dictionary<string, AuthenticationProvider> Authentications = new ();

        // Database time zone
        // Difference to Greenwich time (GMT) with colon between hours and minutes, e.g. +02:00
        public static string DbTimeZone = "";

        // Password (hashed and case-sensitivity)
        // Note: If you enable hashed password, make sure that the passwords in your
        // user table are stored as hash of the clear text password. If you also use
        // case-insensitive password, convert the clear text passwords to lower case
        // first before calculating hash. Otherwise, existing users will not be able
        // to login. Hashed password is irreversible, it will be reset during password recovery.
        public static bool EncryptedPassword { get; set; } = false; // Encrypted password

        public static bool CaseSensitivePassword { get; set; } = false; // Case Sensitive password

        public static Encoding Md5Encoding { get; set; } = Encoding.Unicode; // Encoding for computing MD5 hash // DN

        // Remove XSS use HtmlSanitizer
        // Note: If you want to allow these keywords, remove them from the following array at your own risks.
        public static bool RemoveXss { get; set; } = true;

        // Check Token
        public static bool CheckToken = true; // Check post token by AntiforgeryToken // DN

        // Session timeout time
        public static int SessionTimeout = 20; // Session timeout time (minutes)

        // Session keep alive interval
        public static int SessionKeepAliveInterval = 0; // Session keep alive interval (seconds)

        public static int SessionTimeoutCountdown = 60; // Session timeout count down interval (seconds)

        // Session names
        public const string SessionStatus = ProjectName + "_Status"; // Login status

        public const string SessionUserName = SessionStatus + "_UserName"; // User name

        public const string SessionUserLoginType = SessionStatus + "_UserLoginType"; // User login type

        public const string SessionUserId = SessionStatus + "_UserID"; // User ID

        public const string SessionUserProfile = SessionStatus + "_UserProfile"; // User Profile

        public const string SessionUserProfileUserName = SessionUserProfile + "_UserName";

        public const string SessionUserProfilePassword = SessionUserProfile + "_Password";

        public const string SessionUserProfileLoginType = SessionUserProfile + "_LoginType";

        public const string SessionUserProfileSecret = SessionUserProfile + "_Secret";

        public const string SessionUserLevelId = SessionStatus + "_UserLevel"; // User level ID

        public const string SessionUserLevelList = SessionStatus + "_UserLevelList"; // User Level List

        public const string SessionUserLevelListLoaded = SessionStatus + "_UserLevelListLoaded"; // User Level List Loaded

        public const string SessionUserLevel = SessionStatus + "_UserLevelValue"; // User level

        public const string SessionParentUserId = SessionStatus + "_ParentUserID"; // Parent user ID

        public const string SessionSysAdmin = ProjectName + "_SysAdmin"; // System admin

        public const string SessionProjectId = ProjectName + "_ProjectID"; // User Level project ID

        public const string SessionUserLevelArrays = ProjectName + "_UserLevelArrays"; // User level List // DN

        public const string SessionUserLevelPrivArrays = ProjectName + "_UserLevelPrivArrays"; // User level privilege List // DN

        public const string SessionUserLevelMessage = ProjectName + "_UserLevelMessage"; // User Level messsage

        public const string SessionMessage = ProjectName + "_Message"; // System message

        public const string SessionFailureMessage = ProjectName + "_FailureMessage"; // Failure message

        public const string SessionSuccessMessage = ProjectName + "_SuccessMessage"; // Success message

        public const string SessionWarningMessage = ProjectName + "_WarningMessage"; // Warning message

        public const string SessionMessageHeading = ProjectName + "_MessageHeading"; // Message heading

        public const string SessionInlineMode = ProjectName + "_InlineMode"; // Inline mode

        public const string SessionBreadcrumb = ProjectName + "_Breadcrumb"; // Breadcrumb

        public const string SessionHistory = ProjectName + "_History"; // History (Breadcrumb)

        public const string SessionTempImages = ProjectName + "_TempImages"; // Temp images

        public const string SessionDebugMessage = ProjectName + "_DebugMessage"; // Debug message

        public const string SessionLastRefreshTime = ProjectName + "_LastRefreshTime"; // Last refresh time

        public const string SessionCaptchaCode = ProjectName + "_Captcha"; // Captcha code

        // Language settings
        public static string LanguageFolder = "lang/";

        public static List<dynamic> LanguageFile = new ()
        {
            new { Id = "en-US", File = "english.xml" }
        };

        public static string LanguageDefaultId = "en-US";

        public const string SessionLanguageId = ProjectName + "_LanguageId"; // Language ID

        public static string LocaleFolder = "locale/";

        // Page token
        public const string TokenNameKey = "csrf_name";

        public const string TokenName = "__RequestVerificationToken"; // DO NOT CHANGE! // DN

        public const string TokenValueKey = "__RequestVerificationToken"; // DO NOT CHANGE! // DN

        public const string SessionToken = ProjectName + "_Token";

        // Use database transaction
        public static bool UseTransaction = true;

        // Query timeout for query factory // DN
        public static int QueryTimeout = 30;

        // Data types
        public static List<DataType> CustomTemplateDataTypes = new () { DataType.Number, DataType.Date, DataType.String, DataType.Boolean, DataType.Time }; // Data to be passed to Custom Template

        public static int DataStringMaxLength = 512;

        // Empty/Null/Not Null/Init/all values
        public const string EmptyValue = "##empty##";

        public const string InitValue = "##init##";

        public const string AllValue = "##all##";

        // Boolean values for ENUM('Y'/'N') or ENUM(1/0)
        public const string TrueString = "'Y'";

        public const string FalseString = "'N'";

        // List actions
        public const string ActionPostback = "P"; // Post back

        public const string ActionAjax = "A"; // Ajax

        public const string ActionMultiple = "M"; // Multiple records

        public const string ActionSingle = "S"; // Single record

        // Table parameters
        public const string TablePrefix = "||Report||"; // For backward compatibility only

        public const string TableRecordsPerPage = "recperpage"; // Records per page

        public const string TableStartRec = "start"; // Start record

        public const string TablePageNumber = "page"; // Page number

        public const string TableBasicSearch = "search"; // Basic search keyword

        public const string TableBasicSearchType = "searchtype"; // Basic search type

        public const string TableAdvancedSearch = "advsrch"; // Advanced search

        public const string TableSearchWhere = "searchwhere"; // Search where clause

        public const string TableWhere = "where"; // Table where

        public const string TableOrderBy = "orderby"; // Table order by

        public const string TableOrderByList = "orderbylist"; // Table order by (list page)

        public const string TableRules = "rules"; // Table rules (QueryBuilder)

        public const string TableDetailOrderBy = "detailorderby"; // Table detail order by (report page)

        public const string TableSort = "sort"; // Table sort

        public const string TableKey = "key"; // Table key

        public const string TableShowMaster = "showmaster"; // Table show master

        public const string TableMaster = "master"; // Table show master (alternate key)

        public const string TableShowDetail = "showdetail"; // Table show detail

        public const string TableMasterTable = "mastertable"; // Master table

        public const string TableDetailTable = "detailtable"; // Detail table

        public const string TableReturnUrl = "return"; // Return URL

        public const string TableExportReturnUrl = "exportreturn"; // Export return URL

        public const string TableGridAddRowCount = "gridaddcnt"; // Grid add row count

        // Page layout
        public const string PageLayout = "layout"; // Page layout

        public static List<string> PageLayouts = new () { // Supported page layouts
            "table",
            "cards"
        };

        // Page dashboard
        public const string PageDashboard = "dashboard"; // Page is dashboard

        // Log user ID or user name
        public static bool LogUserId = true; // Log user ID

        // Audit Trail
        public static bool AuditTrailToDatabase { get; set; } = false; // Write to database

        public static string AuditTrailDbId = "DB"; // DB ID

        public static string AuditTrailTableName = ""; // Table name

        public static string AuditTrailTableVar = ""; // Table var

        public static string AuditTrailFieldNameDateTime = ""; // DateTime field name

        public static string AuditTrailFieldNameScript = ""; // Script field name

        public static string AuditTrailFieldNameUser = ""; // User field name

        public static string AuditTrailFieldNameAction = ""; // Action field name

        public static string AuditTrailFieldNameTable = ""; // Table field name

        public static string AuditTrailFieldNameField = ""; // Field field name

        public static string AuditTrailFieldNameKeyvalue = ""; // Key Value field name

        public static string AuditTrailFieldNameOldvalue = ""; // Old Value field name

        public static string AuditTrailFieldNameNewvalue = ""; // New Value field name

        // Export Log
        public static string ExportPath = "export-c1706dd2-6c6c-44d6-ba9d-c75600d81eb6"; // Export folder

        public static string ExportLogDbId = "DB"; // DB ID

        public static string ExportLogTableName = ""; // Table name

        public static string ExportLogTableVar = ""; // Table var

        public static string ExportLogFieldNameFileId = "undefined"; // File id (GUID) field name

        public static string ExportLogFieldNameDateTime = "undefined"; // DateTime field name

        public static string ExportLogFieldNameDateTimeAlias = "datetime"; // DateTime field name Alias

        public static string ExportLogFieldNameUser = "undefined"; // User field name

        public static string ExportLogFieldNameExportType = "undefined"; // Export Type field name

        public static string ExportLogFieldNameExportTypeAlias = "type"; // Export Type field name Alias

        public static string ExportLogFieldNameTable = "undefined"; // Table field name

        public static string ExportLogFieldNameTableAlias = "tablename"; // Table field name Alias

        public static string ExportLogFieldNameKeyValue = "undefined"; // Key Value field name

        public static string ExportLogFieldNameFileName = "undefined"; // File name field name

        public static string ExportLogFieldNameFileNameAlias = "filename"; // File name field name Alias

        public static string ExportLogFieldNameRequest = "undefined"; // Request field name

        public static int ExportFilesExpiryTime = 0; // Files expiry time

        public static string ExportLogSearch = "search"; // Export log search

        public static string ExportLogLimit = "limit"; // Search by limit

        public static string ExportLogArchivePrefix = "export"; // Export log archive prefix

        // Push Notification keys
        public static string PushServerPublicKey = ""; // Public Key

        public static string PushServerPrivateKey = ""; // Private Key
        // Subscription table for Push Notification
        public static string SubscriptionDbId = "DB"; // Subscription DB ID

        public static string SubscriptionTable = "undefined"; // Subscription table

        public static string SubscriptionTableName = ""; // Subscription table name

        public static string SubscriptionTableVar = ""; // Subscription table var

        public static string SubscriptionFieldNameId = ""; // Subscription Id field name

        public static string SubscriptionFieldNameUser = ""; // Subscription User field name

        public static string SubscriptionFieldNameEndpoint = ""; // Subscription Endpoint field name

        public static string SubscriptionFieldNamePublicKey = ""; // Subscription Public Key field name

        public static string SubscriptionFieldNameAuthToken = ""; // Subscription Auth Token field name

        // Security
        public static bool EncryptionEnabled = false; // Encryption enabled

        public static string AdminUserName = ""; // Administrator user name

        public static string AdminPassword = ""; // Administrator password

        public static bool UseCustomLogin { get; set; } = true; // Use custom login (Windows/LDAP/User_CustomValidate)

        public static bool AllowLoginByUrl { get; set; } = false; // Allow login by URL

        public static bool PasswordHash { get; set; } = false; // Use BCrypt.Net-Next password hashing functions

        public static bool UseModalLogin { get; set; } = false; // Use modal login

        public static bool UseModalRegister = false; // Use modal register

        public static bool UseModalChangePassword = false; // Use modal change password

        public static bool UseModalResetPassword = false; // Use modal reset password

        public static int ResetPasswordTimeLimit = 60; // Reset password time limit (minutes)

        public static bool IsWindowsAuthentication = false; // Windows Authentication // DN

        public static string SamlAuthenticationType = "Federation"; // DN

        // User ID
        public static int DefaultUserIdAllowSecurity = 360;

        // User table/field names
        public static string UserTableName = "";

        public static string LoginUsernameFieldName = "";

        public static string LoginPasswordFieldName = "";

        public static string UserIdFieldName = "";

        public static string ParentUserIdFieldName = "";

        public static string UserLevelFieldName = "";

        public static string UserProfileFieldName = "";

        public static string RegisterActivateFieldName = "";

        public static string UserEmailFieldName = "";

        public static string UserPhoneFieldName = "";

        public static string UserImageFieldName = "";

        public static int UserImageSize = 40;

        public static bool UserImageCrop = true;

        // User Profile Constants
        public static string UserProfileSessionId = "SessionId";

        public static string UserProfileLastAccessedDateTime = "LastAccessedDateTime";

        public static int UserProfileConcurrentSessionCount = 1; // Maximum sessions allowed

        public static int UserProfileSessionTimeout = 20;

        public static string UserProfileLoginRetryCount = "LoginRetryCount";

        public static string UserProfileLastBadLoginDateTime = "LastBadLoginDateTime";

        public static int UserProfileMaxRetry = 3;

        public static int UserProfileRetryLockout = 20;

        public static string UserProfileLastPasswordChangedDate = "LastPasswordChangedDate";

        public static int UserProfilePasswordExpire = 90;

        public static string UserProfileLanguageId = "LanguageId";

        public static string UserProfileSearchFilters = "SearchFilters";

        public static string SearchFilterOption = "Client";

        public static string UserProfileImage = "UserImage";

        // Two factor authentication
        public static string UserProfileSecret = "Secret";

        public static string UserProfileSecretCreateDateTime = "SecretCreateDateTime";

        public static string UserProfileSecretVerifyDateTime = "SecretVerifyDateTime";

        public static string UserProfileSecretLastVerifyCode = "SecretLastVerifyCode";

        public static string UserProfileBackupCodes = "BackupCodes";

        public static string UserProfileOneTimePassword = "OTP";

        public static string UserProfileOtpAccount = "OTPAccount";

        public static string UserProfileOtpCreateDateTime = "OTPCreateDateTime";

        public static string UserProfileOtpVerifyDateTime = "OTPVerifyDateTime";

        // Auto hide pager
        public static bool AutoHidePager = false;

        public static bool AutoHidePageSizeSelector = false;

        // Email
        public static string SmtpServer = ""; // SMTP server

        public static int SmtpServerPort = 0; // SMTP server port

        public static string SmtpSecureOption = "None"; // SMTP secure options

        public static string SmtpServerUsername = ""; // SMTP server user name

        public static string SmtpServerPassword = ""; // SMTP server password

        public static string SenderEmail = ""; // Sender email

        public static string RecipientEmail = ""; // Recipient email

        public static int MaxEmailRecipient = 3;

        public static int MaxEmailSentCount = 3;

        public static string ExportEmailCounter = SessionStatus + "_EmailCounter";

        public static string EmailChangePasswordTemplate = "changepassword.html";

        public static string EmailNotifyTemplate = "notify.html";

        public static string EmailRegisterTemplate = "register.html";

        public static string EmailResetPasswordTemplate = "resetpassword.html";

        public static string EmailOneTimePasswordTemplate = "onetimepassword.html";

        public static string EmailTemplatePath = "html"; // Template path // DN

        // SMS
        public static string SmsClass { get; set; } = "Sms"; // Sms class // DN

        public static string SmsOneTimePasswordTemplate = "onetimepassword.txt";

        public static string SmsTemplatePath = "txt"; // Template path // DN
        // https://github.com/twcclegg/libphonenumber-csharp
        // - null => Use region code from locale (i.e. en-US => US)
        public static string? SmsRegionCode = null;

        // Remote file
        public static string RemoteFilePattern = @"^((https?\:)?|ftps?\:|s3:)\/\/";

        // File upload
        public static string UploadType = "POST"; // HTTP request method for the file uploads, e.g. "POST", "PUT

        // File handler // DN
        public static string FileUrl = "";

        // File upload
        public static string UploadTempPath = ""; // Upload temp path (absolute local physical path)

        public static string UploadTempHrefPath = ""; // Upload temp href path (absolute URL path for download)

        public static bool DownloadViaScript = false; // Download uploaded temp file via UploadHandler (DN)

        public static string UploadDestPath = "files/"; // Upload destination path

        public static string UploadHrefPath = ""; // Upload file href path (for download)

        public static string UploadTempFolderPrefix = "temp__"; // Upload temp folders prefix

        public static int UploadTempFolderTimeLimit = 1440; // Upload temp folder time limit (minutes)

        public static string UploadThumbnailFolder = "thumbnail"; // Temporary thumbnail folder

        public static int UploadThumbnailWidth = 200; // Temporary thumbnail max width

        public static int UploadThumbnailHeight = 0; // Temporary thumbnail max height

        public static int? MaxFileCount = null; // Max file count

        public static bool ImageCropper = false; // Upload cropper

        public static string UploadAllowedFileExtensions = "gif,jpg,jpeg,bmp,png,doc,docx,xls,xlsx,pdf,zip"; // Allowed file extensions

        public static List<string> ImageAllowedFileExtensions = new () { "gif","jpe","jpeg","jpg","png","bmp" }; // Allowed file extensions for images

        public static List<string> DownloadAllowedFileExtensions = new () {"csv","pdf","xls","doc","xlsx","docx"}; // Allowed file extensions for download (non-image)

        public static bool EncryptFilePath = true; // Encrypt file path

        public static int MaxFileSize = 2000000; // Max file size

        public static int ThumbnailDefaultWidth = 100; // Thumbnail default width

        public static int ThumbnailDefaultHeight = 0; // Thumbnail default height

        public static bool UploadConvertAccentedChars { get; set; } = false; // Convert accented chars in upload file name

        public static bool UseColorbox { get; set; } = true; // Use Colorbox

        public static char MultipleUploadSeparator = ','; // Multiple upload separator

        // Image resize
        public static bool ResizeIgnoreAspectRatio { get; set; } = false;

        public static bool ResizeLess { get; set; } = false;

        // Form hidden tag names (Note: DO NOT modify prefix "k_")
        public static string FormKeyCountName = "key_count";

        public static string FormRowActionName = "k_action";

        public static string FormBlankRowName = "k_blankrow";

        public static string FormOldKeyName = "k_oldkey";

        // Table actions
        public static string ListAction = "list"; // Table list action

        public static string ViewAction = "view"; // Table view action

        public static string AddAction = "add"; // Table add action

        public static string AddoptAction = "addopt"; // Table addopt action

        public static string EditAction = "edit"; // Table edit action

        public static string UpdateAction = "update"; // Table update action

        public static string DeleteAction = "delete"; // Table delete action

        public static string SearchAction = "search"; // Table search action

        public static string QueryAction = "query"; // Table search action

        public static string PreviewAction = "preview"; // Table preview action

        public static string CustomReportAction = "custom"; // Custom report action

        public static string SummaryReportAction = "summary"; // Summary report action

        public static string CrosstabReportAction = "crosstab"; // Crosstab report action

        public static string DashboardReportAction = "dashboard"; // Dashboard report action

        public static string CalendarReportAction = "calendar"; // Calendar report action

        // API
        public static string ApiUrl = "api/"; // API URL

        public static string ApiActionName = "action"; // API action name

        public static string ApiObjectName = "table"; // API object name
        // export related (start)
        public static string ApiExportName = "export"; // API export name

        public static string ApiExportSave = "save"; // API export save file

        public static string ApiExportOutput = "output"; // API export output file as inline/attachment

        public static string ApiExportDownload = "download"; // API export download file => disposition=attachment

        public static string ApiExportFileName = "filename"; // API export file name

        public static string ApiExportContentType = "contenttype"; // API export content type

        public static string ApiExportUseCharset = "usecharset"; // API export use charset in content type header

        public static string ApiExportUseBom = "usebom"; // API export use BOM

        public static string ApiExportCacheControl = "cachecontrol"; // API export cache control header

        public static string ApiExportDisposition = "disposition"; // API export disposition (inline/attachment)
        // export related (end)
        public static string ApiFieldName = "field"; // API field name

        public static string ApiKeyName = "key"; // API key name

        public static string ApiFileTokenName = "filetoken"; // API upload file token name

        public static string ApiLoginUsername = "username"; // API login user name

        public static string ApiLoginPassword = "password"; // API login password

        public static string ApiLoginSecurityCode = "securitycode"; // API login security code

        public static string ApiLoginExpire = "expire"; // API login expire (hours)

        public static string ApiLoginPermission = "permission"; // API login expire permission (hours)

        public static string ApiLookupPage = "page"; // API lookup page name

        public static string ApiUserlevelName = "userlevel"; // API userlevel name

        public static string ApiPushNotificationSubscribe = "subscribe"; // API push notification subscribe

        public static string ApiPushNotificationSend = "send"; // API push notification send

        public static string ApiPushNotificationDelete = "delete"; // API push notification delete

        public static string Api2FaShow = "show"; // API two factor authentication show

        public static string Api2FaVerify = "verify"; // API two factor authentication verify

        public static string Api2FaReset = "reset"; // API two factor authentication reset

        public static string Api2FaBackupCodes = "codes"; // API two factor authentication backup codes

        public static string Api2FaNewBackupCodes = "newcodes"; // API two factor authentication new backup codes

        public static string Api2FaSendOtp = "otp"; // API two factor authentication send one time password

        // API actions
        public const string ApiListAction = "list"; // API list action

        public const string ApiViewAction = "view"; // API view action

        public const string ApiAddAction = "add"; // API add action

        public const string ApiRegisterAction = "register"; // API register action

        public const string ApiEditAction = "edit"; // API edit action

        public const string ApiDeleteAction = "delete"; // API delete action

        public const string ApiLoginAction = "login"; // API login action

        public const string ApiFileAction = "file"; // API file action

        public const string ApiUploadAction = "upload"; // API upload action

        public const string ApiJqueryUploadAction = "jupload"; // API jQuery upload action

        public const string ApiSessionAction = "session"; // API get session action

        public const string ApiLookupAction = "lookup"; // API lookup action

        public const string ApiImportAction = "import"; // API import action

        public const string ApiExportAction = "export"; // API export action

        public const string ApiExportChartAction = "chart"; // API export chart action

        public const string ApiPermissionsAction = "permissions"; // API permissions action

        public static string ApiPushNotificationAction = "push"; // API push notification action

        public static string Api2FaAction = "2fa"; // API two factor authentication action

        public static List<string> ApiPageActions = new ()
        {
            ApiListAction,
            ApiViewAction,
            ApiAddAction,
            ApiEditAction,
            ApiDeleteAction,
            ApiFileAction,
            ApiExportAction
        };

        // List page inline/grid/modal settings
        public static bool UseAjaxActions = false;

        // Send push notification time limit
        public static int SendPushNotificationTimeLimit = 300;

        public static bool PushAnonymous = false;

        // Use two factor Authentication
        public static bool UseTwoFactorAuthentication = false;

        public static bool ForceTwoFactorAuthentication = false;

        public static string TwoFactorAuthenticationType = "google";

        public static string TwoFactorAuthenticationIssuer = ProjectName;

        public static TimeSpan TwoFactorAuthenticationDiscrepancy = TimeSpan.FromMinutes(5);

        public static int TwoFactorAuthenticationQrcodeSize = 3; // Number of pixels per QR Module (2 = ~120x120px QRCode, should be 10 or less)

        public static int TwoFactorAuthenticationPassCodeLength = 6;

        public static int TwoFactorAuthenticationBackupCodeLength = 8;

        public static int TwoFactorAuthenticationBackupCodeCount = 10;

        // Import records
        public static Encoding ImportCsvEncoding = Encoding.UTF8; // Import CSV encoding

        public static CultureInfo ImportCsvCulture = CultureInfo.InvariantCulture; // Import CSV culture

        public static char ImportCsvDelimiter = ','; // Import CSV delimiter character

        public static char ImportCsvTextQualifier = '"'; // Import CSV text qualifier character

        public static string ImportCsvEol = "\r\n"; // Import CSV end of line, default CRLF

        public static string ImportFileAllowedExtensions = "csv,xlsx"; // Import file allowed extensions

        public static bool ImportInsertOnly = true; // Import by insert only

        public static bool ImportUseTransaction = true; // Import use transaction

        public static int ImportMaxFailures = 1; // Import maximum number of failures

        // Audit trail
        public static string AuditTrailPath = ""; // Audit trail path (relative to wwwroot)

        // Export records
        public static bool ExportAll = true; // Export all records

        public static bool ExportOriginalValue { get; set; } = false; // True to export original value

        public static bool ExportFieldCaption { get; set; } = false; // True to export field caption

        public static bool ExportFieldImage { get; set; } = true; // True to export field image

        public static bool ExportCssStyles { get; set; } = true; // True to export css styles

        public static bool ExportMasterRecord { get; set; } = true; // True to export master record

        public static bool ExportMasterRecordForCsv { get; set; } = false; // True to export master record for CSV

        public static bool ExportDetailRecords { get; set; } = true; // True to export detail records

        public static bool ExportDetailRecordsForCsv { get; set; } = false; // True to export detail records for CSV

        // Export classes
        public static Dictionary<string, string> Export = new (StringComparer.OrdinalIgnoreCase)
        {
            { "email", "ExportEmail" },
            { "html", "ExportHtml" },
            { "word", "ExportWord" },
            { "excel", "ExportExcel" },
            { "pdf", "ExportPdf" },
            { "csv", "ExportCsv" },
            { "xml", "ExportXml" },
            { "json", "ExportJson" }
        };

        // Export report methods
        public static Dictionary<string, string> ExportReport = new (StringComparer.OrdinalIgnoreCase)
        {
            { "email", "ExportEmail" },
            { "html", "ExportHtml" },
            { "word", "ExportWord" },
            { "excel", "ExportExcel" },
            { "pdf", "ExportPdf" }
        };

        // Full URL protocols ("http" or "https")
        public static Dictionary<string, string?> FullUrlProtocols = new (StringComparer.OrdinalIgnoreCase)
        {
            {"href", null},
            {"upload", null},
            {"resetpwd", null},
            {"activate", null},
            {"auth", null},
            {"export", null},
        };

        // Named types // DN
        public static Dictionary<string, Type> NamedTypes = new ()
        {
            {"fxrate", typeof(Fxrate)},
            {"product", typeof(Product)},
            {"order", typeof(Order)},
        };

        // Database IDs // DN
        public static Dictionary<string, string> DbIds = new ()
        {
            {"fxrate", "DB"},
            {"product", "DB"},
            {"order", "DB"},
        };

        // Secondary connection name // DN
        public static string SecondaryConnectionName = "_2";

        // Top menu items // DN
        public static List<object[]> TopMenuItems = new ()
        {
        };

        // Menu items // DN
        public static List<object[]> MenuItems = new ()
        {
            new object[] { 1, "mi_fxrate", "1", "fxratelist", -1, "", true, false, false, "", "", false, true },
            new object[] { 2, "mi_order", "2", "orderlist", -1, "", true, false, false, "", "", false, true },
            new object[] { 4, "mi_product", "4", "productlist", -1, "", true, false, false, "", "", false, true }
        };

        // Boolean HTML attributes
        public static List<string> BooleanHtmlAttributes = new ()
        {
            "allowfullscreen",
            "allowpaymentrequest",
            "async",
            "autofocus",
            "autoplay",
            "checked",
            "controls",
            "default",
            "defer",
            "disabled",
            "formnovalidate",
            "hidden",
            "ismap",
            "itemscope",
            "loop",
            "multiple",
            "muted",
            "nomodule",
            "novalidate",
            "open",
            "readonly",
            "required",
            "reversed",
            "selected",
            "typemustmatch"
        };

        // HTML singleton tags
        public static List<string> HtmlSingletonTags = new ()
        {
            "area",
            "base",
            "br",
            "col",
            "command",
            "embed",
            "hr",
            "img",
            "input",
            "keygen",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr"
        };

        // Use ILIKE for PostgreSQL
        public static bool UseIlikeForPostgresql { get; set; } = true;

        // Use collation for MySQL
        public static string LikeCollationForMysql = "";

        // Use collation for MsSQL
        public static string LikeCollationForMssql = "";

        // Use collation for MsSQL
        public static string LikeCollationForSqlite = "";

        // Null / Not Null values
        public const string NullValue = "##null##";

        public const string NotNullValue = "##notnull##";

        /// <summary>
        /// Search multi value option
        /// 1 - no multi value
        /// 2 - AND all multi values
        /// 3 - OR all multi values
        /// </summary>
        /// <value></value>
        public static short SearchMultiValueOption { get; set; } = 3;

        // Advanced search
        public static string SearchOption = "AUTO";

        // Quick search
        public static string BasicSearchIgnorePattern = @"[\?,\^\*\(\)\[\]\""]"; // Ignore special characters

        public static bool BasicSearchAnyFields { get; set; } = false; // Search "All keywords" in any selected fields

        // Sort options
        public static string SortOption = "Tristate"; // Sort option (toggle/tristate)

        // Validate option
        public static bool ClientValidate { get; set; } = true;

        public static bool ServerValidate { get; set; } = false;

        public static string InvalidUsernameCharacters { get; set; } = "<>\"'&";

        public static string InvalidPasswordCharacters { get; set; } = "<>\"'&";

        // Blob field byte count for hash value calculation
        public static int BlobFieldByteCount { get; set; } = 256;

        // Native select-one
        public static bool UseNativeSelectOne { get; set; } = false;

        // Auto suggest max entries
        public static int AutoSuggestMaxEntries = 10;

        // Auto suggest for all display fields
        public static bool LookupAllDisplayFields = false;

        // Lookup page size
        public static int LookupPageSize = 100;

        // Filter page size
        public static int FilterPageSize = 100;

        // Auto fill original value
        public static bool AutoFillOriginalValue = false;

        // Lookup filter value separator
        public static char MultipleOptionSeparator = ',';

        public static bool UseLookupCache = true;

        public static int LookupCacheCount = 100;

        public static List<string> LookupCachePageIds = new () { "list", "grid" };

        // Page Title Style
        public static string PageTitleStyle = "Breadcrumbs";

        // Responsive table
        public static bool UseResponsiveTable = true;

        public static string ResponsiveTableClass = "table-responsive";

        // Fixed header table
        public static string FixedHeaderTableClass = "table-head-fixed";

        public static bool UseFixedHeaderTable = false;

        public static string FixedHeaderTableHeight = "mh-400px"; // CSS class for fixed header table height

        // Multi column list options position
        public static string MultiColumnListOptionsPosition = "bottom-start";

        // RTL
        public static List<string> RtlLanguages = new () { "ar", "fa", "he", "iw", "ug", "ur" };

        // Date/Time without seconds
        public static bool DatetimeWithoutSeconds = false;

        // Multiple selection
        public static string OptionHtmlTemplate = "<span class=\"ew-option\">{value}</span>"; // Note: class="ew-option" must match CSS style in project stylesheet

        public static string OptionSeparator = ", ";

        // Cookies
        public static int CookieExpires  = 365; // Cookie expiry in days

        public static DateTime CookieExpiryTime = DateTime.Today.AddDays(CookieExpires);

        public static string CookieSameSite = "Unspecified";

        public static bool CookieHttpOnly = true;

        public static bool CookieSecure = false;

        public static string CookieConsentClass = "toast-body bg-secondary"; // CSS class name for cookie consent

        public static string CookieConsentButtonClass = "btn btn-dark btn-sm"; // CSS class name for cookie consent buttons

        // Mime type // DN
        public static string DefaultMimeType = "application/octet-stream";

        /// <summary>
        /// Reports
        /// </summary>

        // Chart
        public static int ChartWidth = 600;

        public static int ChartHeight = 500;

        public static bool ChartShowBlankSeries { get; set; } = false; // Show blank series

        public static bool ChartShowZeroInStackChart { get; set; } = false; // Show zero in stack chart

        public static bool ChartShowMissingSeriesValuesAsZero { get; set; } = false; // Show missing series values as zero

        public static bool ChartScaleBeginWithZero { get; set; } = false; // Chart scale begin with zero

        public static double ChartScaleMinimumValue { get; set; } = 0; // Chart scale minimum value

        public static double ChartScaleMaximumValue { get; set; } = 0; // Chart scale maximum value

        public static bool ChartShowPercentage = false; // Show percentage in Pie/Doughnut charts

        // Drill down setting
        public static bool UseDrillDownPanel { get; set; } = true; // Use popup panel for drill down

        // Filter
        public static bool ShowCurrentFilter { get; set; } = false; // True to show current filter

        public static bool ShowDrillDownFilter { get; set; } = true; // True to show drill down filter

        // Table level constants
        public static string TableGroupPerPage = "recperpage";

        public static string TableStartGroup = "start";

        public static string TableSortChart = "sortchart"; // Table sort chart

        // Page break
        public static string PageBreakHtml = "<div style=\"page-break-after:always;\"></div>";

        // User permissions
        public static List<string> Privileges = new () {
            "add",
            "delete",
            "edit",
            "list",
            "view",
            "search",
            "import",
            "lookup",
            "export",
            "push",
            "admin" // Put "admin" at last for userpriv page
        };

        // Embed PDF documents
        public static bool EmbedPdf = true;

        // Advanced Filters
        public static Dictionary<string, Dictionary<string, string>> ReportAdvancedFilters = new (StringComparer.OrdinalIgnoreCase)
        {
            { "PastFuture", new () { { "Past", "IsPast" }, { "Future", "IsFuture" } } },
            { "RelativeDayPeriods", new () { { "Last30Days", "IsLast30Days" }, { "Last14Days", "IsLast14Days" }, { "Last7Days", "IsLast7Days" }, { "Next7Days", "IsNext7Days" }, { "Next14Days", "IsNext14Days" }, { "Next30Days", "IsNext30Days" } } },
            { "RelativeDays", new () { { "Yesterday", "IsYesterday" }, { "Today", "IsToday" }, { "Tomorrow", "IsTomorrow" } } },
            { "RelativeWeeks", new () { { "LastTwoWeeks", "IsLast2Weeks" }, { "LastWeek", "IsLastWeek" }, { "ThisWeek", "IsThisWeek" }, { "NextWeek", "IsNextWeek" }, { "NextTwoWeeks", "IsNext2Weeks" } } },
            { "RelativeMonths", new () { { "LastMonth", "IsLastMonth" }, { "ThisMonth", "IsThisMonth" }, { "NextMonth", "IsNextMonth" } } },
            { "RelativeYears", new () { { "LastYear", "IsLastYear" }, { "ThisYear", "IsThisYear" }, { "NextYear", "IsNextYear" } } }
        };

        // Float fields default decimal position
        public static int DefaultDecimalPrecision = 2;

        // Chart
        public static string DefaultChartRenderer = "";

        // Captcha class // DN
        public static string CaptchaClass { get; set; } = "CaptchaBase";

        // API version
        public static string ApiVersion { get; set; } = "v1";

        /// <summary>
        /// Get property/field by name
        /// </summary>
        /// <param name="name"></param>
        public static object? Get(string name) =>
            typeof(Config).GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null) ?? // Get property
            typeof(Config).GetField(name)?.GetValue(null); // Get field

        /// <summary>
        /// Float fields default number format
        /// See https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
        /// </summary>
        public static string DefaultNumberFormat { get; set; } = "#,##0.##";

        // Pace options
        public static Dictionary<string, object> PaceOptions { get; set; } = new ()
        {
            {
                "ajax", new Dictionary<string, object>() {
                    { "trackMethods", new List<string>() { "GET", "POST" } },
                    { "ignoreURLs", new List<string>() { "/session?" } }
                }
            }
        };

        /// <summary>
        /// Date time formats
        /// See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
        /// Note: "y" - The year, from 0 to 99. // DN
        /// </summary>
        public static Dictionary<int, string> DateFormats { get; set; } = new ()
        {
            { 4, "HH:mm" },
            { 5, "yyyy/MM/dd" },
            { 6, "MM/dd/yyyy" },
            { 7, "dd/MM/yyyy" },
            { 9, "yyyy/MM/dd HH:mm:ss" },
            { 10, "MM/dd/yyyy HH:mm:ss" },
            { 11, "dd/MM/yyyy HH:mm:ss" },
            { 109, "yyyy/MM/dd HH:mm" },
            { 110, "MM/dd/yyyy HH:mm" },
            { 111, "dd/MM/yyyy HH:mm" },
            { 12, "yy/MM/dd" },
            { 13, "MM/dd/yy" },
            { 14, "dd/MM/yy" },
            { 15, "yy/MM/dd HH:mm:ss" },
            { 16, "MM/dd/yy HH:mm:ss" },
            { 17, "dd/MM/yy HH:mm:ss" },
            { 115, "yy/MM/dd HH:mm" },
            { 116, "MM/dd/yy HH:mm" },
            { 117, "dd/MM/yy HH:mm" },
        };

        // Database date time formats
        public static Dictionary<string, Dictionary<string, string>> DbDateFormats { get; set; } = new ()
        {
            { "MYSQL", new () {
                    { "dd", "%d" },
                    { "d", "%e" },
                    { "HH", "%H" },
                    { "H", "%k" },
                    { "hh", "%h" },
                    { "h", "%l" },
                    { "MM", "%m" },
                    { "M", "%c" },
                    { "mm", "%i" },
                    { "m", "%i" },
                    { "ss", "%S" },
                    { "s", "%S" },
                    { "yy", "%y" },
                    { "y", "%Y" },
                    { "a", "%p" }
                }
            },
            { "POSTGRESQL", new () {
                    { "dd", "DD" },
                    { "d", "FMDD" },
                    { "HH", "HH24" },
                    { "H", "FMHH24" },
                    { "hh", "HH12" },
                    { "h", "FMHH12" },
                    { "MM", "MM" },
                    { "M", "FMMM" },
                    { "mm", "MI" },
                    { "m", "FMMI" },
                    { "ss", "SS" },
                    { "s", "FMSS" },
                    { "yy", "YY" },
                    { "y", "YYYY" },
                    { "a", "AM" }
                }
            },
            { "MSSQL", new () {
                    { "dd", "dd" },
                    { "d", "d" },
                    { "HH", "HH" },
                    { "H", "H" },
                    { "hh", "hh" },
                    { "h", "h" },
                    { "MM", "MM" },
                    { "M", "M" },
                    { "mm", "mm" },
                    { "m", "m" },
                    { "ss", "ss" },
                    { "s", "s" },
                    { "yy", "yy" },
                    { "y", "yyyy" },
                    { "a", "tt" }
                }
            },
            { "ORACLE", new () {
                    { "dd", "DD" },
                    { "d", "FMDD" },
                    { "HH", "HH24" },
                    { "H", "FMHH24" },
                    { "hh", "HH12" },
                    { "h", "FMHH12" },
                    { "MM", "MM" },
                    { "M", "FMMM" },
                    { "mm", "MI" },
                    { "m", "FMMI" },
                    { "ss", "SS" },
                    { "s", "FMSS" },
                    { "yy", "YY" },
                    { "y", "YYYY" },
                    { "a", "AM" }
                }
            },
            { "SQLITE", new () {
                    { "dd", "%d" },
                    { "d", "%d" },
                    { "HH", "%H" },
                    { "H", "%H" },
                    { "hh", "%I" },
                    { "h", "%I" },
                    { "MM", "%m" },
                    { "M", "%m" },
                    { "mm", "%M" },
                    { "m", "%M" },
                    { "ss", "%S" },
                    { "s", "%S" },
                    { "yy", "%y" },
                    { "y", "%Y" },
                    { "a", "%P" }
                }
            },
            { "ACCESS", new () {
                    { "dd", "dd" },
                    { "d", "d" },
                    { "HH", "HH" },
                    { "H", "H" },
                    { "hh", "hh" },
                    { "h", "h" },
                    { "MM", "MM" },
                    { "M", "M" },
                    { "mm", "mm" },
                    { "m", "m" },
                    { "ss", "ss" },
                    { "s", "s" },
                    { "yy", "yy" },
                    { "y", "yyyy" },
                    { "a", "tt" }
                }
            }
        };

        // Quarter name
        //public static string QuarterPattern { get; set; } = "QQQQ"; // Note: No Q format string, not used
        public static string QuarterPattern { get; set; } = "";

        // Month name
        public static string MonthPattern { get; set; } = "MMM";

        // Table client side variables
        public static List<string> TableClientVars { get; set; } = new ()
        {
            "TableName",
            "TableCaption"
        };

        // Field client side variables
        public static List<string> FieldClientVars { get; set; } = new ()
        {
            "Name",
            "Caption",
            "Visible",
            "Required",
            "IsInvalid",
            "Raw",
            "ClientFormatPattern",
            "ClientSearchOperators"
        };

        // Query builder search operators
        public static Dictionary<string, string> ClientSearchOperators { get; set; } = new()
        {
            { "=", "equal" },
            { "<>", "not_equal" },
            { "IN", "in" },
            { "NOT IN", "not_in" },
            { "<", "less" },
            { "<=", "less_or_equal" },
            { ">", "greater" },
            { ">=", "greater_or_equal" },
            { "BETWEEN", "between" },
            { "NOT BETWEEN", "not_between" },
            { "STARTS WITH", "begins_with" },
            { "NOT STARTS WITH", "not_begins_with" },
            { "LIKE", "contains" },
            { "NOT LIKE", "not_contains" },
            { "ENDS WITH", "ends_with" },
            { "NOT ENDS WITH", "not_ends_with" },
            { "IS EMPTY", "is_empty" },
            { "IS NOT EMPTY", "is_not_empty" },
            { "IS NULL", "is_null" },
            { "IS NOT NULL", "is_not_null" }
        };

        // Query builder search operators settings
        public static Dictionary<string, Dictionary<string, object>> QueryBuilderOperators = new ()
        {
            { "equal", new () {
                    { "type", "equal" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string", "number", "datetime", "boolean" } }
                }
            },
            { "not_equal", new () {
                    { "type", "not_equal" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string", "number", "datetime", "boolean" } }
                }
            },
            { "in", new () {
                    { "type", "in" },
                    { "nb_inputs", 1 },
                    { "multiple", true },
                    { "apply_to", new List<string> { "string", "number", "datetime" } }
                }
            },
            { "not_in", new () {
                    { "type", "not_in" },
                    { "nb_inputs", 1 },
                    { "multiple", true },
                    { "apply_to", new List<string> { "string", "number", "datetime" } }
                }
            },
            { "less", new () {
                    { "type", "less" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "less_or_equal", new () {
                    { "type", "less_or_equal" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "greater", new () {
                    { "type", "greater" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "greater_or_equal", new () {
                    { "type", "greater_or_equal" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "between", new () {
                    { "type", "between" },
                    { "nb_inputs", 2 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "not_between", new () {
                    { "type", "not_between" },
                    { "nb_inputs", 2 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "number", "datetime" } }
                }
            },
            { "begins_with", new () {
                    { "type", "begins_with" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "not_begins_with", new () {
                    { "type", "not_begins_with" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "contains", new () {
                    { "type", "contains" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "not_contains", new () {
                    { "type", "not_contains" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "ends_with", new () {
                    { "type", "ends_with" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "not_ends_with", new () {
                    { "type", "not_ends_with" },
                    { "nb_inputs", 1 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "is_empty", new () {
                    { "type", "is_empty" },
                    { "nb_inputs", 0 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "is_not_empty", new () {
                    { "type", "is_not_empty" },
                    { "nb_inputs", 0 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string" } }
                }
            },
            { "is_null", new () {
                    { "type", "is_null" },
                    { "nb_inputs", 0 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string", "number", "datetime", "boolean" } }
                }
            },
            { "is_not_null", new () {
                    { "type", "is_not_null" },
                    { "nb_inputs", 0 },
                    { "multiple", false },
                    { "apply_to", new List<string> { "string", "number", "datetime", "boolean" } }
                }
            }
        };

        // Value separator for IN operator
        public static string InOperatorValueSeparator = "|";

        // Value separator for BETWEEN operator
        public static string BetweenOperatorValueSeparator = "|";

        // Value separator for OR operator
        public static string OrOperatorValueSeparator = "||";

        // Config client side variables
        public static List<string> ConfigClientVars { get; set; } = new ()
        {
            "DEBUG",
            "SESSION_TIMEOUT_COUNTDOWN", // Count down time to session timeout (seconds)
            "SESSION_KEEP_ALIVE_INTERVAL", // Keep alive interval (seconds)
            "API_FILE_TOKEN_NAME", // API file token name
            "API_URL", // API file name
            "API_ACTION_NAME", // API action name
            "API_OBJECT_NAME", // API object name
            "API_LIST_ACTION", // API list action
            "API_VIEW_ACTION", // API view action
            "API_ADD_ACTION", // API add action
            "API_EDIT_ACTION", // API edit action
            "API_DELETE_ACTION", // API delete action
            "API_LOGIN_ACTION", // API login action
            "API_FILE_ACTION", // API file action
            "API_UPLOAD_ACTION", // API upload action
            "API_JQUERY_UPLOAD_ACTION", // API jQuery upload action
            "API_SESSION_ACTION", // API get session action
            "API_LOOKUP_ACTION", // API lookup action
            "API_LOOKUP_PAGE", // API lookup page name
            "API_IMPORT_ACTION", // API import action
            "API_EXPORT_ACTION", // API export action
            "API_EXPORT_CHART_ACTION", // API export chart action
            "PUSH_SERVER_PUBLIC_KEY", // Push Server Public Key
            "API_PUSH_NOTIFICATION_ACTION", // API push notification action
            "API_PUSH_NOTIFICATION_SUBSCRIBE", // API push notification subscribe
            "API_PUSH_NOTIFICATION_DELETE", // API push notification delete
            "API_2FA_ACTION", // API two factor authentication action
            "API_2FA_SHOW", // API two factor authentication show
            "API_2FA_VERIFY", // API two factor authentication verify
            "API_2FA_RESET", // API two factor authentication reset
            "API_2FA_BACKUP_CODES", // API two factor authentication backup codes
            "API_2FA_NEW_BACKUP_CODES", // API two factor authentication new backup codes
            "API_2FA_SEND_OTP", // API two factor authentication send one time password
            "TWO_FACTOR_AUTHENTICATION_TYPE", // Two factor authentication type
            "MULTIPLE_OPTION_SEPARATOR", // Multiple option separator
            "AUTO_SUGGEST_MAX_ENTRIES", // Auto-Suggest max entries
            "LOOKUP_PAGE_SIZE", // Lookup page size
            "FILTER_PAGE_SIZE", // Filter page size
            "MAX_EMAIL_RECIPIENT",
            "UPLOAD_THUMBNAIL_WIDTH", // Upload thumbnail width
            "UPLOAD_THUMBNAIL_HEIGHT", // Upload thumbnail height
            "MULTIPLE_UPLOAD_SEPARATOR", // Upload multiple separator
            "IMPORT_FILE_ALLOWED_EXTENSIONS", // Import file allowed extensions
            "USE_COLORBOX",
            "PROJECT_STYLESHEET_FILENAME", // Project style sheet
            "EMBED_PDF",
            "LAZY_LOAD",
            "REMOVE_XSS",
            "ENCRYPTED_PASSWORD",
            "INVALID_USERNAME_CHARACTERS",
            "INVALID_PASSWORD_CHARACTERS",
            "USE_RESPONSIVE_TABLE",
            "RESPONSIVE_TABLE_CLASS",
            "SEARCH_FILTER_OPTION",
            "OPTION_HTML_TEMPLATE",
            "PAGE_LAYOUT",
            "CLIENT_VALIDATE",
            "IN_OPERATOR_VALUE_SEPARATOR",
            "TABLE_BASIC_SEARCH",
            "TABLE_BASIC_SEARCH_TYPE",
            "TABLE_PAGE_NUMBER",
            "TABLE_SORT",
            "FORM_KEY_COUNT_NAME",
            "FORM_ROW_ACTION_NAME",
            "FORM_BLANK_ROW_NAME",
            "FORM_OLD_KEY_NAME",
            "IMPORT_MAX_FAILURES",
            "TWO_FACTOR_AUTHENTICATION_PASS_CODE_LENGTH",
            "USE_JAVASCRIPT_MESSAGE",
            "LIST_ACTION",
            "VIEW_ACTION",
            "EDIT_ACTION",
            "IS_WINDOWS_AUTHENTICATION" // DN
        };

        /// <summary>
        /// Additional JSON configuration files
        /// e.g. Config_Init server event
        /// JsonFiles.Add(new { Path = "foobar.json", Optional = false, ReloadOnChange = true });
        /// </summary>
        public static List<dynamic> JsonFiles = new ();
    }
} // End Partial class
