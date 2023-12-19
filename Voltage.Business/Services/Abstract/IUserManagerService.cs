using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract;

public interface IUserManagerService
{
    Task<IdentityResult> CreateAsync(User user);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<IdentityResult> AddToRoleAsync(User user, string roleName);
    Task<User> FindByLoginAsync(string loginProvider, string providerKey);
    Task<User> FindByEmailAsync(string email);
    Task<User?> FindByNameAsync(string name);
    Task<User> FindByIdAsync(string Id);
    Task<IEnumerable<User>> GetAllUsers(Expression<Func<User, bool>> filter = null!);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<string> GenerateResetTokenAsync(User user);
    Task<string> GenerateEmailTokenAsync(User user);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info);
    Task<IdentityResult> ResetAccessFailedCountAsync(User user);
    Task<DateTimeOffset?> GetLockoutEndDateAsync(User user);
    Task<IdentityResult> DeleteAsync(User user);
    Task<IdentityResult> UpdateAsync(User user);
    Task<string?> GetProfilePhotoAsync(string userName);
    Task<bool> IsEmailConfirmedAsync(User user);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
