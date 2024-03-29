using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Concrete;

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly VoltageDbContext _dbContext;

    public UserManagerService(UserManager<User> userManager, VoltageDbContext dbContext, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    #region Methods

    public async Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info) =>
        await _userManager.AddLoginAsync(user, info);

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            IdentityResult roleResult = await AddRole("User");
            if (!roleResult.Succeeded)
                return roleResult;
        }
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password) =>
        await _userManager.CheckPasswordAsync(user, password);

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) =>
        await _userManager.ConfirmEmailAsync(user, token);

    public async Task<IdentityResult> CreateAsync(User user) =>
        await _userManager.CreateAsync(user);

    public async Task<IdentityResult> CreateAsync(User user, string password) =>
        await _userManager.CreateAsync(user, password);

    public async Task<User> FindByLoginAsync(string loginProvider, string providerKey) =>
        await _userManager.FindByLoginAsync(loginProvider, providerKey);

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user) =>
        await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<string> GenerateEmailTokenAsync(User user) =>
        await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<string> GenerateResetTokenAsync(User user) =>
        await _userManager.GeneratePasswordResetTokenAsync(user);

    public async Task<IEnumerable<User>> GetAllUsers(Expression<Func<User, bool>> filter = null!) =>
        filter == null ? await Task.FromResult(_userManager.Users) : await Task.FromResult(_userManager.Users.Where(filter));

    public async Task<DateTimeOffset?> GetLockoutEndDateAsync(User user) =>
        await _userManager.GetLockoutEndDateAsync(user);

    public async Task<User> FindByEmailAsync(string email) =>
        await _userManager.FindByEmailAsync(email);

    public async Task<User?> FindByNameAsync(string name) =>
        await _userManager.FindByNameAsync(name) ?? null;

    public async Task<IdentityResult> ResetAccessFailedCountAsync(User user) =>
        await _userManager.ResetAccessFailedCountAsync(user);

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password) =>
        await _userManager.ResetPasswordAsync(user, token, password);

    public async Task<IdentityResult> DeleteAsync(User user) =>
        await _userManager.DeleteAsync(user);

    public async Task<IdentityResult> UpdateAsync(User user) =>
        await _userManager.UpdateAsync(user);

    public async Task<User> FindByIdAsync(string Id) =>
        await _userManager.FindByIdAsync(Id);

    public async Task BeginTransactionAsync() =>
        await _dbContext.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync() =>
        await _dbContext.Database.CommitTransactionAsync();

    public async Task RollbackTransactionAsync() =>
        await _dbContext.Database.RollbackTransactionAsync();

    public async Task<bool> IsEmailConfirmedAsync(User user) =>
        await _userManager.IsEmailConfirmedAsync(user);

    public async Task<string?> GetProfilePhotoAsync(string userName) => (await _userManager.FindByNameAsync(userName)).Photo;

    private async Task<IdentityResult> AddRole(string roleName) =>
    await _roleManager.CreateAsync(new IdentityRole
    {
        Name = roleName
    });
    #endregion
}
