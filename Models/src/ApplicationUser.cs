namespace Zaharuddin.Models;

/// <summary>
/// Application user class
/// </summary>
public class ApplicationUser : IIdentity
{
    public virtual string Id { get; set; } = ""; // User ID

    public virtual string UserName { get; set; } = ""; // User name

    public virtual string Email { get; set; } = ""; // Email

    public virtual string ParentUserId { get; set; } = ""; // Parent user ID(s)

    public virtual string UserLevelId { get; set; } = ""; // User level ID(s)

    public string NormalizedUserName { get; internal set; } = ""; // IUserStore

    public string AuthenticationType { get; set; } = ""; // IIdentity

    public bool IsAuthenticated { get; set; } // IIdentity

    public string Name { get; set; } = ""; // IIdentity

    public List<int> UserLevelIds => UserLevelId.Split(Config.MultipleOptionSeparator).Where(id => Int32.TryParse(id, out _)).Select(Int32.Parse).ToList();

    public List<string> ParentUserIds => ParentUserId.Split(Config.MultipleOptionSeparator).Select(s => s.Trim()).ToList();
}
