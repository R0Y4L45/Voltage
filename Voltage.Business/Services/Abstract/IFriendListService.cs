using Voltage.Core.DataAccess;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Business.Services.Abstract;

public interface IFriendListService : IEntityRepository<FriendList, VoltageDbContext>
{
    Task<IEnumerable<UsersFriendListResult>> GetUsersSearchResultAsync(string Id, SearchDto obj);
    Task<List<FriendListItemDto>> GetFriendListAsync(string userId, string requestStatus);
    Task<IEnumerable<UserDto>?> GetUsersByRequestAsync(string id);
    Task<UserDto?> GetUserDtoByNameAsync(string name);
    Task DeleteAsync(FriendList entity);
    Task<FriendList?> CheckFriendListAsync(string sender, string receiver);
}
