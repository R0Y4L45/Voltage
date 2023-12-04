using Voltage.Core.DataAccess;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Business.Services.Abstract;

public interface IFriendListService : IEntityRepository<FriendList, VoltageDbContext>
{
    Task<IEnumerable<UsersFriendListResult>> GetUsersSearchResult(string Id, SearchDto obj);
    Task DeleteAsync(FriendList entity);
}
