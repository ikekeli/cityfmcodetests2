namespace Zaharuddin.Models;

/// <summary>
/// Custom user store class
/// </summary>
public class CustomUserStore : IUserStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>
{
    // Constructor
    public CustomUserStore()
    {
        throw new NotImplementedException();
    }

    // CreateAsync
    public Task<IdentityResult> CreateAsync(ApplicationUser user,
        CancellationToken cancellationToken = default(CancellationToken)) =>
        throw new NotImplementedException();

    // DeleteAsync
    public Task<IdentityResult> DeleteAsync(ApplicationUser user,
        CancellationToken cancellationToken = default(CancellationToken)) =>
        throw new NotImplementedException();

    // Dispose
    public void Dispose() {}

    // FindByIdAsync
    public Task<ApplicationUser> FindByIdAsync(string userId,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    // FindByNameAsync
    public Task<ApplicationUser> FindByNameAsync(string userName,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    // GetNormalizedUserNameAsync
    public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // GetPasswordHashAsync
    public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // GetUserIdAsync
    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // GetUserNameAsync
    public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // HasPasswordAsync
    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // SetNormalizedUserNameAsync
    public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // SetPasswordHashAsync
    public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // SetUserNameAsync
    public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // UpdateAsync
    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
