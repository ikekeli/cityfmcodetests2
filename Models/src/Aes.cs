namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// AES encryption class
    /// </summary>
    public class Aes
    {
        public static byte[] AesEncrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] saltBytes = passwordBytes;
            using MemoryStream ms = new ();
            var aes = System.Security.Cryptography.Aes.Create();
            aes.FeedbackSize = 128;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA256); // DO NOT CHANGE, must match dll
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            using (CryptoStream cs = new (ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
            }
            return ms.ToArray();
        }

        public static byte[] AesDecrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] saltBytes = passwordBytes;
            using MemoryStream ms = new ();
            var aes = System.Security.Cryptography.Aes.Create();
            aes.FeedbackSize = 128;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000, HashAlgorithmName.SHA256); // DO NOT CHANGE, must match dll
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            using (CryptoStream cs = new (ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            }
            return ms.ToArray();
        }

        public static string Encrypt(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = AesEncrypt(bytesToBeEncrypted, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted).Replace("+", "-").Replace("/", "_").Replace("=", ""); // Remove padding
        }

        public static string Decrypt(string input, string password)
        {
            try {
                string inputBase64 = ConvertToString(input).Replace("-", "+").Replace("_", "/");
                int len = inputBase64.Length;
                if (len % 4 != 0)
                    inputBase64 = inputBase64.PadRight(len + 4 - len % 4, '='); // Add padding
                // Get the bytes of the string
                byte[] bytesToBeDecrypted = Convert.FromBase64String(inputBase64);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                byte[] bytesDecrypted = AesDecrypt(bytesToBeDecrypted, passwordBytes);
                string result = Encoding.UTF8.GetString(bytesDecrypted);
                return result;
            } catch {
                return input;
            }
        }
    }
} // End Partial class
