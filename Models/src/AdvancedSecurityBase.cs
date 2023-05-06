namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Advanced Security class
    /// </summary>
    public class AdvancedSecurityBase
    {
        private List<UserLevel> UserLevel = new ();

        private List<UserLevelPermission> UserLevelPriv = new ();

        public List<int> UserLevelID = new ();

        public List<string> UserID = new ();

        public List<string> ParentUserID = new ();

        public string CurrentUserLevelID;

        public int CurrentUserLevel;

        public string CurrentUserID;

        public List<Claim> Claims = new (); // DN

        private bool _isLoggedIn;

        private bool _isSysAdmin;

        private string _userName = "";

        // Init
        public AdvancedSecurityBase()
        {
            // Init User Level
            if (IsLoggedIn) {
                CurrentUserLevelID = SessionUserLevelID;
                if (CurrentUserLevelID == "")
                    CurrentUserLevelID = "-2";
                SetUserLevelID(CurrentUserLevelID);
            } else { // Anonymous user
                CurrentUserLevelID = "-2";
                UserLevelID.Add(Convert.ToInt32(CurrentUserLevelID));
            }
            Session[Config.SessionUserLevelList] = UserLevelList();

            // Init User ID
            CurrentUserID = ConvertToString(SessionUserID);
            SetParentUserID(ConvertToString(SessionParentUserID));

            // Load user level
            LoadUserLevel();
        }

        // Session User ID
        protected object SessionUserID {
            get => Session.TryGetValue(Config.SessionUserId, out string? userId) ? userId : CurrentUserID;
            set {
                CurrentUserID = ConvertToString(value).Trim();
                Session[Config.SessionUserId] = CurrentUserID;
            }
        }

        // Session parent User ID
        protected object SessionParentUserID {
            get => Session.TryGetValue(Config.SessionParentUserId, out string? parentUserId) ? parentUserId : CurrentParentUserID; // DN
            set {
                SetParentUserID(ConvertToString(value).Trim());
                Session[Config.SessionParentUserId] = CurrentParentUserID; // DN
            }
        }

        // Current Parent User ID
        public string CurrentParentUserID => String.Join(Config.MultipleOptionSeparator, ParentUserID);

        // Set Parent User ID to array
        protected void SetParentUserID(object v) {
            var ids = v switch {
                List<string> list => list,
                int i => new List<string> { ConvertToString(i) },
                string str => str.Split(Config.MultipleOptionSeparator).ToList(),
                _ => new List<string>()
            };
            ParentUserID = ids;
        }

        // Check if Parent User ID in array
        public bool HasParentUserID(object v)
        {
            var ids = v switch {
                List<string> list => list,
                int i => new List<string> { ConvertToString(i) },
                string str => str.Split(Config.MultipleOptionSeparator).ToList(),
                _ => new List<string>()
            };
            return ParentUserID.Intersect(ids).Any();
        }

        // Current user name
        public string CurrentUserName
        {
            get => Session.TryGetValue(Config.SessionUserName, out string? currentUserName) ? currentUserName : _userName;
            set {
                _userName = ConvertToString(value).Trim();
                Session[Config.SessionUserName] = _userName;
            }
        }

        // Session User Level ID
        protected string SessionUserLevelID {
            get => Session.TryGetValue(Config.SessionUserLevelId, out string? userLevelId) ? userLevelId : CurrentUserLevelID;
            set {
                CurrentUserLevelID = value;
                Session.SetString(Config.SessionUserLevelId, CurrentUserLevelID);
                SetUserLevelID(CurrentUserLevelID);
            }
        }

        // Set User Level ID to array
        protected void SetUserLevelID(object v) {
            var ids = v switch {
                List<int> list => list,
                int i => new List<int> { i },
                string str => str.Split(Config.MultipleOptionSeparator).Where(x => int.TryParse(x, out _)).Select(int.Parse).ToList(),
                _ => new List<int>()
            };
            UserLevelID = ids.Where(id => id >= -2).ToList();
        }

        // Check if User Level ID in array
        public bool HasUserLevelID(object v)
        {
            var ids = v switch {
                List<int> list => list,
                int i => new List<int> { i },
                string str => str.Split(Config.MultipleOptionSeparator).Where(x => int.TryParse(x, out _)).Select(int.Parse).ToList(),
                _ => new List<int>()
            };
            return UserLevelID.Intersect(ids).Any();
        }

        // Session User Level value
        protected int SessionUserLevel {
            get => Session.TryGetValue(Config.SessionUserLevel, out string? userLevel) ? Convert.ToInt32(userLevel) : CurrentUserLevel;
            set {
                CurrentUserLevel = value;
                Session.SetString(Config.SessionUserLevel, ConvertToString(CurrentUserLevel));
            }
        }

        // Get JWT Token
        public string JwtToken =>
            GetJwtToken(CurrentUserName, ConvertToString(SessionUserID), ConvertToString(SessionParentUserID), SessionUserLevelID);

        // Get JWT Token
        public string CreateJwtToken(int expire = 0, int permission = 0) =>
            GetJwtToken(CurrentUserName, ConvertToString(SessionUserID), ConvertToString(SessionParentUserID), SessionUserLevelID, expire, permission);

        // Can add
        public bool CanAdd
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Add);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Add;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Add;
                }
            }
        }

        // Can delete
        public bool CanDelete
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Delete);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Delete;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Delete;
                }
            }
        }

        // Can edit
        public bool CanEdit
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Edit);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Edit;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Edit;
                }
            }
        }

        // Can view
        public bool CanView
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.View);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.View;
                } else {
                    CurrentUserLevel &= ~(int)Allow.View;
                }
            }
        }

        // Can list
        public bool CanList
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.List);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.List;
                } else {
                    CurrentUserLevel &= ~(int)Allow.List;
                }
            }
        }

        // Can search
        public bool CanSearch
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Search);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Search;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Search;
                }
            }
        }

        // Can admin
        public bool CanAdmin
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Admin);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Admin;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Admin;
                }
            }
        }

        // Can import
        public bool CanImport
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Import);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Import;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Import;
                }
            }
        }

        // Can lookup
        public bool CanLookup
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Lookup);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Lookup;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Lookup;
                }
            }
        }

        // Can push
        public bool CanPush
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Push);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Push;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Push;
                }
            }
        }

        // Can export
        public bool CanExport
        {
            get => ((Allow)CurrentUserLevel).HasFlag(Allow.Export);
            set {
                if (value) {
                    CurrentUserLevel |= (int)Allow.Export;
                } else {
                    CurrentUserLevel &= ~(int)Allow.Export;
                }
            }
        }

        // Last URL
        public string LastUrl => Cookie["lasturl"];

        // Save last URL
        public void SaveLastUrl()
        {
            string s = CurrentUrl();
            if (LastUrl == s)
                s = "";
            if (!Regex.IsMatch(s, @"[?&]modal=1(&|$)")) // Query string does not contain "modal=1"
                Cookie["lasturl"] = s;
        }

        // Auto login
        public async Task<bool> AutoLoginAsync()
        {
            bool valid = false;
            var model = new LoginModel();
            if (!valid && SameString(Cookie["autologin"], "autologin")) {
                model.Username = Cookie["username"];
                model.Password = Cookie["password"];
                model.Username = Decrypt(model.Username);
                model.Password = Decrypt(model.Password);
                valid = await ValidateUser(model, true);
            }
            if (!valid && Config.AllowLoginByUrl && Get("username", out StringValues user) && !Empty(user)) {
                model.Username = RemoveXss(user);
                model.Password = RemoveXss(Get("password"));
                valid = await ValidateUser(model, true);
            }
            return valid;
        }

        // Auto login
        public bool AutoLogin() => AutoLoginAsync().GetAwaiter().GetResult();

        // Login user
        public void LoginUser(string? userName = null, object? userID = null, object? parentUserID = null, string? userLevel = null, int allowedPermission = 0)
        {
            if (userName != null) {
                CurrentUserName = userName;
                if (userName == Language.Phrase("UserAdministrator")) // Handle language phrase as well
                    userName = Config.AdminUserName;
            }
            if (userID != null)
                SessionUserID = userID;
            if (parentUserID != null)
                SessionParentUserID = parentUserID;
            if (userLevel != null) {
                SessionUserLevelID = userLevel;
                int level = ConvertToInt(userLevel);
                if (level > -2) {
                    _isLoggedIn = true;
                    Session[Config.SessionStatus] = "login";
                    _isSysAdmin = level == -1;
                }
                SetupUserLevel();
            }
            // Set allowed permission
            SetAllowedPermissions(allowedPermission);
        }

        // Logout user
        public void LogoutUser()
        {
            _isLoggedIn = false;
            Session[Config.SessionStatus] = "";
            CurrentUserName = "";
            SessionUserID = "";
            SessionParentUserID = "";
            SessionUserLevelID = "-2";
            SetupUserLevel();
        }

        #pragma warning disable 162, 168, 219, 1998
        // Validate user
        public async Task<bool> ValidateUser(LoginModel model, bool autologin, string provider = "")
        {
            string usr = model.Username;
            string pwd = model.Password;
            string securitycode = model.SecurityCode ?? "";
            try {
                string filter, sql;
                bool valid = false, providerValid = false, saveUserProfile = false;

                // Login by external provider // DN
                if (!Empty(provider)) {
                    if (Config.Authentications.TryGetValue(provider, out AuthenticationProvider? p) && p != null && p.Enabled) {
                        Profile = ResolveProfile();
                        if (Profile.TryGetValue(ClaimTypes.Email.Split('/').Last(), out string? email) && email != null)
                            usr = email;
                        providerValid = true;
                    } else {
                        if (Config.Debug)
                            End("Provider for " + provider + " not found or not enabled.");
                        return false;
                    }
                }

                // Custom validate
                bool customValid = false;

                // Call User Custom Validate event
                if (Config.UseCustomLogin) {
                    customValid = UserCustomValidate(ref usr, ref pwd);
                }

                // Handle provider login as custom login
                if (providerValid)
                    customValid = true;
                if (customValid) {
                    //Session[Config.SessionStatus] = "login"; // To be setup below
                    CurrentUserName = usr; // Load user name
                }

                // User Validated event
                if (customValid)
                    customValid = UserValidated(null);
                Profile.Save();
                if (customValid)
                    return customValid;
                if (!valid && !IsPasswordExpired()) {
                    _isLoggedIn = false;
                    Session[Config.SessionStatus] = ""; // Clear login status
                }
                return valid;
            } finally {
                model.Username = usr;
                model.Password = pwd;
            }
        }
        #pragma warning restore 162, 168, 219, 1998

        // No user level security
        public void SetupUserLevel()
        {
        }

        // Check import/lookup permissions
        protected void CheckPermissions()
        {
        }

        // Set allowed permissions
        protected void SetAllowedPermissions(int permission = 0)
        {
            if (permission > 0) {
                if (IsList(UserLevelPriv)) {
                    for (int i = 0; i < UserLevelPriv.Count; i++) {
                        var row = UserLevelPriv[i];
                        row.Permission &= permission;
                    }
                }
            }
        }

        // Add user permission
        public void AddUserPermission(string userLevelName, string tableName, int permission)
        {
            string UserLevelID = "";
            // Get user level ID from user name
            if (IsList(UserLevel))
                UserLevelID = ConvertToString(UserLevel.FirstOrDefault(row => SameString(userLevelName, row.Name))?.Id);
            if (IsList(UserLevelPriv) && !Empty(UserLevelID)) {
                var row = UserLevelPriv.FirstOrDefault(r => SameString(r.Table, Config.ProjectId + tableName) && SameString(r.Id, UserLevelID));
                if (row != null)
                    row.Permission |= permission; // Add permission
                else
                    UserLevelPriv.Add(new UserLevelPermission { Id = ConvertToInt(UserLevelID), Table = Config.ProjectId + tableName, Permission = permission });
            }
        }

        // Add user permission
        public void AddUserPermission(string userLevelName, string tableName, Allow permission) =>
            AddUserPermission(userLevelName, tableName, (int)permission);

        // Add user permission
        public void AddUserPermission(List<string> userLevelName, List<string> tableName, int permission) =>
            userLevelName.ForEach(_userLevelName => tableName.ForEach(_tableName => AddUserPermission(_userLevelName, _tableName, permission)));

        // Add user permission
        public void AddUserPermission(List<string> userLevelName, List<string> tableName, Allow permission) =>
            AddUserPermission(userLevelName, tableName, (int)permission);

        // Add user permission
        public void AddUserPermission(string userLevelName, List<string> tableName, int permission) =>
            tableName.ForEach(name => AddUserPermission(userLevelName, name, permission));

        // Add user permission
        public void AddUserPermission(string userLevelName, List<string> tableName, Allow permission) =>
            AddUserPermission(userLevelName, tableName, (int)permission);

        // Add user permission
        public void AddUserPermission(List<string> userLevelName, string tableName, int permission) =>
            userLevelName.ForEach(name => AddUserPermission(name, tableName, permission));

        // Add user permission
        public void AddUserPermission(List<string> userLevelName, string tableName, Allow permission) =>
            AddUserPermission(userLevelName, tableName, (int)permission);

        // Delete user permission
        public void DeleteUserPermission(string userLevelName, string tableName, int permission)
        {
            string UserLevelID = "";
            // Get user level ID from user name
            if (IsList(UserLevel))
                UserLevelID = ConvertToString(UserLevel.FirstOrDefault(row => SameString(userLevelName, row.Name))?.Id);
            if (IsList(UserLevelPriv) && !Empty(UserLevelID)) {
                var row = UserLevelPriv.FirstOrDefault(r => SameString(r.Table, Config.ProjectId + tableName) && SameString(r.Id, UserLevelID));
                if (row != null)
                    row.Permission &= ~permission; // Remove permission
            }
        }

        // Delete user permission
        public void DeleteUserPermission(string userLevelName, string tableName, Allow permission) =>
            DeleteUserPermission(userLevelName, tableName, (int)permission);

        // Delete user permission
        public void DeleteUserPermission(List<string> userLevelName, List<string> tableName, int permission) =>
            userLevelName.ForEach(_userLevelName => tableName.ForEach(_tableName => DeleteUserPermission(_userLevelName, _tableName, permission)));

        // Delete user permission
        public void DeleteUserPermission(List<string> userLevelName, List<string> tableName, Allow permission) =>
            DeleteUserPermission(userLevelName, tableName, (int)permission);

        // Delete user permission
        public void DeleteUserPermission(string userLevelName, List<string> tableName, int permission) =>
            tableName.ForEach(name => DeleteUserPermission(userLevelName, name, permission));

        // Delete user permission
        public void DeleteUserPermission(string userLevelName, List<string> tableName, Allow permission) =>
            DeleteUserPermission(userLevelName, tableName, (int)permission);

        // Delete user permission
        public void DeleteUserPermission(List<string> userLevelName, string tableName, int permission) =>
            userLevelName.ForEach(name => DeleteUserPermission(name, tableName, permission));

        // Delete user permission
        public void DeleteUserPermission(List<string> userLevelName, string tableName, Allow permission) =>
            DeleteUserPermission(userLevelName, tableName, (int)permission);

        // Load table permissions
        public void LoadTablePermissions(string tblVar)
        {
            string tblName = GetTableName(tblVar);
            if (IsLoggedIn)
                Invoke(this, "TablePermissionLoading");
            LoadCurrentUserLevel(Config.ProjectId + tblName);
            if (IsLoggedIn) {
                Invoke(this, "TablePermissionLoaded");
            }
            if (IsLoggedIn) {
                Invoke(this, "UserIdLoading");
                Invoke(this, "LoadUserID");
                Invoke(this, "UserIdLoaded");
            }
        }

        // Load current user level
        public void LoadCurrentUserLevel(string table)
        {
            // Load again if user level list changed
            if (!Empty(Session[Config.SessionUserLevelListLoaded]) &&
                !SameString(Session[Config.SessionUserLevelListLoaded], Session[Config.SessionUserLevelList])) { // Compare strings, not objects // DN
                Session[Config.SessionUserLevelPrivArrays] = null;
            }
            LoadUserLevel();
            SessionUserLevel = CurrentUserLevelPriv(table);
        }

        // Get current user privilege
        protected int CurrentUserLevelPriv(string tableName)
        {
            if (IsLoggedIn) {
                return (int)Allow.All;
            } else { // Anonymous
                return GetUserLevelPriv(tableName, -2);
            }
        }

        // Get user level ID by user level name
        public int GetUserLevelID(string userLevelName)
        {
            if (SameString(userLevelName, "Anonymous")) {
                return -2;
            } else if (SameString(userLevelName, Language?.Phrase("UserAnonymous"))) {
                return -2;
            } else if (SameString(userLevelName, "Administrator")) {
                return -1;
            } else if (SameString(userLevelName, Language?.Phrase("UserAdministrator"))) {
                return -1;
            } else if (SameString(userLevelName, "Default")) {
                return 0;
            } else if (SameString(userLevelName, Language?.Phrase("UserDefault"))) {
                return 0;
            } else if (!Empty(userLevelName)) {
                if (IsList(UserLevel)) {
                    var row = UserLevel.FirstOrDefault(r => SameString(r.Name, userLevelName));
                    if (row != null)
                        return row.Id;
                }
            }
            return -2; // Anonymous
        }

        // Add Add User Level by name
        public void AddUserLevel(string userLevelName)
        {
            if (Empty(userLevelName))
                return;
            int id = GetUserLevelID(userLevelName);
            AddUserLevelID(id);
        }

        // Add User Level by ID
        public void AddUserLevelID(int id)
        {
            if (!IsNumeric(id) || id < -1)
                return;
            if (!UserLevelID.Contains(id)) {
                UserLevelID.Add(id);
                Session[Config.SessionUserLevelList] = UserLevelList(); // Update session variable
            }
        }

        // Delete User Level by name
        public void DeleteUserLevel(string userLevelName)
        {
            if (Empty(userLevelName))
                return;
            int id = GetUserLevelID(userLevelName);
            DeleteUserLevelID(id);
        }

        // Delete User Level by ID
        public void DeleteUserLevelID(int id)
        {
            if (!IsNumeric(id) || id < -1)
                return;
            if (UserLevelID.Contains(id)) {
                UserLevelID.Remove(id);
                Session[Config.SessionUserLevelList] = UserLevelList(); // Update session variable
            }
        }

        // User level list
        public string UserLevelList() => String.Join(", ", UserLevelID);

        // User level list
        public bool UserLevelIDExists(int id) => IsList(UserLevel) && UserLevel.Exists(row => row.Id == id);

        // User level name list
        public string UserLevelNameList()
        {
            string list = "";
            return list;
        }

        // Get user privilege based on table name and user level
        public int GetUserLevelPriv(string tableName, int userLevelId)
        {
            if (userLevelId == -1) { // System Administrator
                return (int)Allow.All;
            } else if (userLevelId >= 0 || userLevelId == -2) {
                if (IsList(UserLevelPriv))
                    return UserLevelPriv.FirstOrDefault(row => SameString(row.Table, tableName) && row.Id == userLevelId)?.Permission ?? 0;
            }
            return 0;
        }

        // Get current user level name
        public string CurrentUserLevelName => GetUserLevelName(CurrentUserLevelID);

        // Get user level name based on user level
        public string GetUserLevelName(string? userLevelId, bool lang = true)
        {
            List<int> ids = userLevelId?.Split(Config.MultipleOptionSeparator)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .Where(x => x >= -2).ToList() ?? new ();
            var names = ids.Select(id => {
                if (id == -2) {
                    return lang ? Language.Phrase("UserAnonymous") : "Anonymous";
                } else if (id == -1) {
                    return lang ? Language.Phrase("UserAdministrator") : "Administrator";
                } else if (id == 0) {
                    return lang ? Language.Phrase("UserDefault") : "Default";
                } else {
                    if (IsList(UserLevel)) {
                        string name = UserLevel.FirstOrDefault(row => row.Id == id)?.Name ?? "";
                        string userLevelName = lang ? Language.Phrase(name) : "";
                        return (userLevelName != "") ? userLevelName : name;
                    }
                    return id.ToString();
                }
            });
            return String.Join(", ", names);
        }

        // Display all the User Level settings (for debug only)
        public void ShowUserLevelInfo()
        {
            if (IsList(UserLevel)) {
                Write("User Levels:<br>");
                VarDump(UserLevel);
            } else {
                Write("No User Level definitions." + "<br>");
            }
            if (IsList(UserLevelPriv)) {
                Write("User Level Privs:<br>");
                VarDump(UserLevelPriv);
            } else {
                Write("No User Level privilege settings." + "<br>");
            }
            Write("CurrentUserLevel = " + CurrentUserLevel + "<br>");
            Write("User Levels = " + UserLevelList() + "<br>");
        }

        // Check privilege for List page (for menu items)
        public bool AllowList(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.List);

        // Check privilege for View page (for Allow-View / Detail-View)
        public bool AllowView(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.View);

        // Check privilege for Add page (for Allow-Add / Detail-Add)
        public bool AllowAdd(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.Add);

        // Check privilege for Edit page (for Detail-Edit)
        public bool AllowEdit(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.Edit);

        // Check privilege for Delete page (for API)
        public bool AllowDelete(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.Delete);

        // Check privilege for lookup
        public bool AllowLookup(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.Lookup);

        // Check privilege for export
        public bool AllowExport(string tableName) => ConvertToBool((Allow)CurrentUserLevelPriv(tableName) & Allow.Export);

        // Check if user password expired
        public bool IsPasswordExpired => SameString(Session[Config.SessionStatus], "passwordexpired");

        // Set session password expired
        public void SetSessionPasswordExpired() => Session[Config.SessionStatus] = "passwordexpired";

        // Check if user password reset
        public bool IsPasswordReset => SameString(Session[Config.SessionStatus], "passwordreset");

        // Check if user is logging in (after changing password)
        public bool IsLoggingIn => SameString(Session[Config.SessionStatus], "loggingin");

        // Check if user is logging in (2FA)
        public bool IsLoggingIn2FA => SameString(Session[Config.SessionStatus], "loggingin2fa");

        // Check if user is logged in
        public bool IsLoggedIn => UseSession ? SameString(Session[Config.SessionStatus], "login") : _isLoggedIn;

        // Check if user is system administrator
        public bool IsSysAdmin => UseSession ? (Session.GetInt(Config.SessionSysAdmin) == 1) : _isSysAdmin;

        // Check if user is administrator
        public bool IsAdmin
        {
            get {
                bool res = IsSysAdmin;
                return res;
            }
        }

        // Save user level to session
        public void SaveUserLevel()
        {
            Session.SetValue(Config.SessionUserLevelArrays, UserLevel); // DN
            Session.SetValue(Config.SessionUserLevelPrivArrays, UserLevelPriv); // DN
        }

        // Load user level from session // DN
        public void LoadUserLevel()
        {
            if (Session[Config.SessionUserLevelArrays] == null ||
                Session[Config.SessionUserLevelPrivArrays] == null ||
                Empty(Session[Config.SessionUserLevelArrays]) ||
                Empty(Session[Config.SessionUserLevelPrivArrays]) ||
                IsEmptyList(Session[Config.SessionUserLevelArrays]) ||
                IsEmptyList(Session[Config.SessionUserLevelPrivArrays])) { // DN
                SetupUserLevel();
            } else {
                UserLevel = Session.GetValue<List<UserLevel>>(Config.SessionUserLevelArrays)!;
                UserLevelPriv = Session.GetValue<List<UserLevelPermission>>(Config.SessionUserLevelPrivArrays)!;
            }
        }

        public bool IsValidUserID(object? id) => true;

        // UserID Loading event
        public virtual void UserIdLoading() {
            //Log("UserID Loading: {UserID}", CurrentUserID);
        }

        // UserID Loaded event
        public virtual void UserIdLoaded() {
            //Log("UserID Loaded: {UserIDList}", UserIDList());
        }

        // User Level Loaded event
        public virtual void UserLevelLoaded() {
            //AddUserPermission(<UserLevelName>, <TableName>, <UserPermission>);
            //DeleteUserPermission(<UserLevelName>, <TableName>, <UserPermission>);
        }

        // Table Permission Loading event
        public virtual void TablePermissionLoading() {
            //Log("Table Permission Loading: {UserLevel}", CurrentUserLevelID);
        }

        // Table Permission Loaded event
        public virtual void TablePermissionLoaded() {
            //Log("Table Permission Loaded: {UserLevel}", CurrentUserLevel);
        }

        // User Custom Validate event
        public virtual bool UserCustomValidate(ref string usr, ref string pwd) {
            // Enter your custom code to validate user, return true if valid.
            return false;
        }

        // User Validated event
        public virtual bool UserValidated(DbDataReader? rs) {
            return true; // Return true/false to validate the user
        }

        // User PasswordExpired event
        public virtual void UserPasswordExpired(DbDataReader rs) {
            //Log("User_PasswordExpired");
        }
    }
} // End Partial class
