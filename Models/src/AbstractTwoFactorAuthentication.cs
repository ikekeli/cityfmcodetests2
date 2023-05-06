namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Abstract Two Factor Authentication class
    /// </summary>
    public abstract class AbstractTwoFactorAuthentication : ITwoFactorAuthentication
    {
        // Check code
        public abstract bool CheckCode(string secret, string code);

        // Generate secret
        public abstract string GenerateSecret();

        public virtual string GetQrCodeUrl(string user, string secret, string? issuer = null, int size = 0)
        {
            return ""; // To be implemented by subclasses
        }

        // Show (API action)
        public abstract Task<IActionResult> Show();

        // Generate backup codes
        public List<string> GenerateBackupCodes()
        {
            int length = Config.TwoFactorAuthenticationBackupCodeLength;
            int count = Config.TwoFactorAuthenticationBackupCodeCount;
            List<string> list = new ();
            Random random = new ();
            for (int i = 0; i < count; i++) {
                string code = "";
                for (int j = 0; j < length; j++)
                    code += random.Next(0, 9).ToString();
                list.Add(code);
            }
            return list;
        }

        // Get backup codes
        public async Task<List<string>> GetBackupCodes()
        {
            string user = CurrentUserName(); // Must be current user
            var profile = ResolveProfile();
            return await profile.GetBackupCodes(user);
        }

        // Get new backup codes
        public async Task<List<string>> GetNewBackupCodes()
        {
            string user = CurrentUserName(); // Must be current user
            var profile = ResolveProfile();
            return await profile.GetNewBackupCodes(user);
        }

        // Verify
        public async Task<bool> Verify(string code)
        {
            string user = CurrentUserName(); // Must be current user
            var profile = ResolveProfile();
            if (Empty(code)) // Verify if user has secret only
                return await profile.HasUserSecret(user, true);
            else // Verified, just check code
                if (await profile.HasUserSecret(user))
                    return await profile.Verify2FACode(user, code);
            return false;
        }

        // Reset
        public async Task<bool> Reset(string? user)
        {
            user = IsSysAdmin() && !Empty(user) ? user : Config.ForceTwoFactorAuthentication ? "" : CurrentUserName();
            if (!Empty(user)) {
                var profile = ResolveProfile();
                if (await profile.HasUserSecret(user))
                    return await profile.ResetUserSecret(user);
            }
            return false;
        }

        public virtual Task<JsonBoolResult> SendOneTimePassword(string user, string? account = null)
        {
            return Task.FromResult(new JsonBoolResult(new {
                    success = false,
                    version = Config.ProductVersion
                }, false)); // To be implemented by subclasses
        }

        public virtual string GetAccount(string user)
        {
            return ""; // To be implemented by subclasses
        }

        // One Time Password Sending event
        public static bool OtpSending(string user, dynamic client) {
            // Example:
            // Log(user, client); // View user and client (Email or Sms object)
            // if (SameText(Config.TwoFactorAuthenticationType, "email")) { // Possible values: "email" or "sms"
            //     client.Content = ...; // Change content
            //     client.Recipient = ...; // Change recipient
            //     // return false; // Return false to cancel
            // }
            return true;
        }
    }
} // End Partial class
