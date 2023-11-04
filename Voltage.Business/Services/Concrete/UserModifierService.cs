using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Concrete;

public class UserModifierService : IUserModifierService
{
    private readonly UserManager<User> _userManager;

    public UserModifierService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public void Add(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(User user)
    {
        return await _userManager.CreateAsync(user);
    }

    public void Delete(User entity)
    {
        _userManager.DeleteAsync(entity).Wait();
    }

    public User Get(Expression<Func<User, bool>> filter = null!)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetList(Expression<Func<User, bool>> filter = null!) => _userManager.Users.ToList();

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }

    public bool Update(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
    {
        return await _userManager.AddToRoleAsync(user, roleName);
    }
}
