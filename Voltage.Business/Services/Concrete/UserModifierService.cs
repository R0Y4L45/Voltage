using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<User>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return users;
    }

    public Task<int> AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetListAsync(Expression<Func<User, bool>> filter = null!)
    {
        return await Task.FromResult(filter == null ? _userManager.Users : _userManager.Users.Where(filter)); 
    }
}
