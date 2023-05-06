namespace Zaharuddin.Models;

// Partial class
public partial class cityfmcodetests {
    /// <summary>
    /// Cookie class
    /// </summary>
    public class HttpCookies
    {
        public string this[string name]
        {
            get => Request?.Cookies[Config.ProjectName + "_" + name] ?? "";
            set => Response?.Cookies.Append(Config.ProjectName + "_" + name, value, new CookieOptions {
                    Path = AppPath(),
                    IsEssential = true,
                    Expires = Config.CookieExpiryTime,
                    HttpOnly = Config.CookieHttpOnly,
                    Secure = Config.CookieSecure,
                    SameSite = Enum.Parse<Microsoft.AspNetCore.Http.SameSiteMode>(Config.CookieSameSite)
                });
        }
    }
} // End Partial class
