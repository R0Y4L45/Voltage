using Microsoft.AspNetCore.Identity;
using Voltage.Core.DataAccess;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract;

public interface IUserModifierService : IEntityRepository<User, VoltageDbContext> 
{
    Task<IdentityResult> CreateAsync(User user);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<IdentityResult> AddToRoleAsync(User user, string roleName);
}
