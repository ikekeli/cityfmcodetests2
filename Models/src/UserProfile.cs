namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {

    #pragma warning disable 169
    /// <summary>
    /// UserProfile class
    /// </summary>
    public class UserProfile
    {
        public string Username = "";

        private Dictionary<string, string> _profile = new ();

        public int TimeoutTime;

        public int MaxRetryCount;

        public int RetryLockoutTime;

        public int PasswordExpiryTime;

        private string BackupUsername = "";

        private Dictionary<string, string> _backupProfile = new ();

        private Dictionary<string, string> _allfilters = new ();

        // Constructor
        public UserProfile()
        {
            Load();
        }

        // Contains key // DN
        public bool ContainsKey(string name) => _profile.ContainsKey(name);

        // Get value
        public string GetValue(string name) => _profile.TryGetValue(name, out string? value) ? value : "";

        // Set value
        public void SetValue(string name, string value) => _profile[name] = value;

        // Get/Set as string
        public string this[string name]
        {
            get => GetValue(name);
            set => SetValue(name, value);
        }

        // Try get value
        public bool TryGetValue(string name, [NotNullWhen(true)] out string? value) => _profile.TryGetValue(name, out value);

        // Delete property
        public void Remove(string name) => _profile.Remove(name);

        // Backup profile
        public void Backup(string user)
        {
            if (!Empty(Username) && user != Username) {
                BackupUsername = Username;
                _backupProfile = new (_profile);
            }
        }

        // Restore profile
        public void Restore(string user)
        {
            if (!Empty(BackupUsername) && user != BackupUsername) {
                Username = BackupUsername;
                _profile = new (_backupProfile);
            }
        }

        // Assign properties
        public void Assign(Dictionary<string, object>? input)
        {
            if (input == null)
                return;
            foreach (var (key, value) in input) {
                if (IsNull(value))
                    SetValue(key, "");
                else if (value is bool || IsNumeric(value))
                    SetValue(key, ConvertToString(value));
                else if (value is string str && str.Length <= Config.DataStringMaxLength)
                    SetValue(key, str);
            }
        }

        // Assign properties
        public void Assign(Dictionary<string, string>? input)
        {
            if (input == null)
                return;
            foreach (var (key, value) in input)
                SetValue(key, value);
        }

        // Check if System Admin
        protected bool IsSysAdmin(string user) {
            string adminUserName = Config.EncryptionEnabled ? AesDecrypt(Config.AdminUserName) : Config.AdminUserName;
            return (user == "" || user == adminUserName);
        }

        // Load profile from session
        public void Load() => LoadProfile(Session.GetString(Config.SessionUserProfile));

        // Save profile to session
        public void Save() => Session[Config.SessionUserProfile] = ProfileToString();

        // Load profile
        public void LoadProfile(string str)
        {
            if (!Empty(str) && !SameString(str, "{}")) // DN
                _profile = StringToProfile(str);
        }

        // Clear profile
        public void ClearProfile() => _profile.Clear();

        // Clear profile (alias)
        public void Clear() => ClearProfile();

        // Serialize profile
        public string ProfileToString() => JsonConvert.SerializeObject(_profile);

        // Split profile
        private Dictionary<string, string> StringToProfile(string str)
        {
            try {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(str) ?? new ();
            } catch {
                return new ();
            }
        }

        #pragma warning disable 1998

        // User has 2FA secret
        public async Task<bool> HasUserSecret(string user, bool verified = false) => false;

        // Verify 2FA code
        public async Task<bool> Verify2FACode(string user, string code) => false;

        // Get User 2FA secret
        public async Task<string> GetUserSecret(string user) => "";

        // Set one time passwword (Email/SMS)
        public async Task<bool> SetOneTimePassword(string user, string account, string otp) => false;

        // Get backup codes
        public async Task<List<string>> GetBackupCodes(string user = "") => new ();

        // Get new set of backup codes
        public async Task<List<string>> GetNewBackupCodes(string user) => new ();

        // Reset user secret
        public async Task<bool> ResetUserSecret(string user) => false;
        #pragma warning restore 1998
    }
    #pragma warning restore 169
} // End Partial class
