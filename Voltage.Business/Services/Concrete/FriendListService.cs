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
                   on users.Id equals friendList.SenderId
                   join userRole in _context.UserRoles
                   on users.Id equals userRole.UserId
                   join role in _context.Roles
                   on userRole.RoleId equals role.Id
                   where friendList.ReceiverId == id && friendList.RequestStatus == Status.Pending
                   select new UserDto
                   {
                       UserName = users.UserName,
                       Photo = users.Photo,
                       Country = users.Country,
                       Email = users.Email,
                       Role = role.Name
                   };
        });

    public async Task<UserDto?> GetUserDtoByNameAsync(string name) =>
        await Task.Run(async () =>
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await (from users in _context.Users
                              join friendList in _context.FriendList!
                              on users.Id equals friendList.SenderId
                              join userRole in _context.UserRoles
                              on users.Id equals userRole.UserId
                              join role in _context.Roles
                              on userRole.RoleId equals role.Id
                              where users.UserName == name
                              select new UserDto
                              {
                                  UserName = users.UserName,
                                  Photo = users.Photo,
                                  Country = users.Country,
                                  Email = users.Email,
                                  Role = role.Name
                              }).FirstOrDefaultAsync();
            }
            
            return null;
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

    public async Task<FriendList?> CheckFriendListAsync(string sender, string receiver) =>
        await Task.Run(() =>
        {
            if (_context.FriendList != null)
            {
                FriendList? temp = _context.FriendList.FirstOrDefault(_ => _.SenderId == sender &&
                                                                           _.ReceiverId == receiver ||
                                                                           _.SenderId == receiver &&
                                                                           _.ReceiverId == sender);
                if (temp != null)
                    return temp;
            }

            return null;
        });


    /*public async Task<IEnumerable<FriendListItemDto>> GetFriendListAsync(string userId)
    {
        var cmd = $@"
    SELECT 
        users.UserName AS [UserName], 
        users.Country AS [Country],
        users.Photo AS [Photo], 
        friendList.SenderId AS [FriendId],
        friendList.RequestStatus AS [RequestStatus],
        friendList.RequestedDate AS [RequestedDate],
        friendList.AcceptedDate AS [AcceptedDate]
    FROM 
        FriendList AS [friendList]
    INNER JOIN 
        AspNetUsers AS [users]
    ON 
        (friendList.SenderId = users.Id OR friendList.ReceiverId = users.Id)
        AND (friendList.RequestStatus = '2' OR friendList.RequestStatus = '1')
    WHERE 
        (friendList.SenderId = {userId} OR friendList.ReceiverId = {userId})
        AND users.Id != {userId}
    ORDER BY 
        users.UserName;";

        return await _context.Set<FriendListItemDto>().FromSqlRaw(cmd).ToListAsync();
    }*/



    public async Task<List<FriendListItemDto>> GetFriendListAsync(string userId, string? requestStatus = null)
    {
        var friendListQuery = _context.FriendList!
            .Where(fl => (fl.SenderId == userId || fl.ReceiverId == userId));

        if (!string.IsNullOrEmpty(requestStatus))
        {
            if (requestStatus == "Pending")
                friendListQuery = friendListQuery.Where(fl => fl.RequestStatus == Status.Pending);
            else if (requestStatus == "Accepted")
                friendListQuery = friendListQuery.Where(fl => fl.RequestStatus == Status.Accepted);
        }

        var friendList = await friendListQuery
            .Join(_context.Users,
                  fl => fl.SenderId == userId ? fl.ReceiverId : fl.SenderId,
                  user => user.Id,
                  (fl, user) => new FriendListItemDto
                  {
                      UserName = user.UserName,
                      Country = user.Country,
                      Email = user.Email,
                      Photo = user.Photo,
                      FriendId = fl.SenderId == userId ? fl.ReceiverId : fl.SenderId,
                      RequestStatus = fl.RequestStatus.ToString(),
                      RequestedDate = fl.RequestedDate,
                      AcceptedDate = fl.AcceptedDate
                  })
            .Where(dto => dto.FriendId != userId)
            .OrderBy(dto => dto.UserName)
            .ToListAsync();

        return friendList;
    }
}

