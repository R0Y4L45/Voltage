using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Business.Services.Concrete;

public class FriendListService : IFriendListService
{
    private readonly VoltageDbContext _context;

    public FriendListService(VoltageDbContext context) =>
        _context = context;

    public async Task<int> AddAsync(FriendList entity)
    {
        await _context.AddAsync(entity);
        return await _context.SaveChangesAsync();
    }

    public void Delete(FriendList entity) => _context.Remove(entity);

    public async Task DeleteAsync(FriendList entity) => await Task.Run(() => { _context.Remove(entity); _context.SaveChanges(); });

    public async Task<FriendList> GetAsync(Expression<Func<FriendList, bool>> filter = null!) =>
        (await _context.FriendList?.FirstOrDefaultAsync(filter)!)!;

    public async Task<IEnumerable<FriendList>> GetListAsync(Expression<Func<FriendList, bool>> filter = null!) =>
        await Task.FromResult(filter == null ? _context.FriendList! : _context.FriendList?.Where(filter)!);

    public async Task<IEnumerable<UsersFriendListResult>> GetUsersSearchResultAsync(string Id, SearchDto obj)
    {
        FormattableString cmd =
            $@"SELECT users.UserName AS [UserName], 
                      users.Country AS [Country],
                      users.Photo AS [Photo], 
                      list.SenderName AS [SenderName],
               	      list.SenderId AS [SenderId],
               	      list.ReceiverName AS [ReceiverName],
               	      list.ReceiverId AS [ReceiverId],
               	      list.RequestStatus AS [RequestStatus],
               	      list.RequestedDate AS [RequestedDate],
                      list.AcceptedDate AS [AcceptedDate]
               FROM AspNetUsers AS [users]
               LEFT JOIN(SELECT senderUsers.UserName AS [SenderName],
               				    friendList.SenderId AS [SenderId],
               				    receiverUsers.UserName AS [ReceiverName],
               				    friendList.ReceiverId AS [ReceiverId],
               				    friendList.RequestStatus AS [RequestStatus],
               				    friendList.RequestedDate AS [RequestedDate],
               				    friendList.AcceptedDate as [AcceptedDate]
               		     FROM AspNetUsers AS senderUsers
               		     LEFT JOIN (SELECT * FROM FriendList
               		     	        WHERE SenderId = {Id} or ReceiverId = {Id}) AS [friendList]
               		     ON friendList.SenderId = senderUsers.Id
               		     RIGHT JOIN AspNetUsers AS [receiverUsers]
               		     ON friendList.ReceiverId = receiverUsers.Id) AS [list]
               ON list.ReceiverId = users.Id or list.SenderId = users.Id
               WHERE users.UserName LIKE '%' + {obj.Content} + '%' and users.Id != {Id}
               ORDER BY users.UserName";

        return await Task.Run(() => _context.Set<UsersFriendListResult>()?.FromSqlInterpolated(cmd)!);
    }

    public async Task<IEnumerable<UserDto>?> GetUsersByRequestAsync(string id) =>
        await Task.Run(() =>
        {
            return from users in _context.Users
                   join friendList in _context.FriendList!
                   on users.Id equals friendList.ReceiverId
                   join userRole in _context.UserRoles
                   on users.Id equals userRole.UserId
                   join role in _context.Roles
                   on userRole.RoleId equals role.Id
                   where friendList.ReceiverId == id
                   select new UserDto
                   {
                       Id = users.Id,
                       UserName = users.UserName,
                       Photo = users.Photo,
                       Country = users.Country,
                       Email = users.Email,
                       Role = role.Name
                   };
        });

    public async Task<UserDto> GetUserDtoByNameAsync(string name) =>
        await Task.Run(async () =>
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await (from users in _context.Users
                               join friendList in _context.FriendList!
                               on users.Id equals friendList.ReceiverId
                               join userRole in _context.UserRoles
                               on users.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where users.UserName == name
                               select new UserDto
                               {
                                   Id = users.Id,
                                   UserName = users.UserName,
                                   Photo = users.Photo,
                                   Country = users.Country,
                                   Email = users.Email,
                                   Role = role.Name
                               }).FirstOrDefaultAsync() ?? new UserDto();
            }
            
            return new UserDto();
        });

    public bool Update(FriendList entity)
    {
        if (_context.Update(entity) != null)
        {
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    public async Task<bool> CheckRequestAsync(string sender, string receiver) =>
        await Task.Run(() =>
        {
            if (_context.FriendList != null)
            {
                FriendList? temp = _context.FriendList.FirstOrDefault(_ => _.SenderId == sender &&
                                                                           _.ReceiverId == receiver ||
                                                                           _.SenderId == receiver &&
                                                                           _.ReceiverId == sender);
                if (temp != null)
                    return true;
            }

            return false;
        });
}
