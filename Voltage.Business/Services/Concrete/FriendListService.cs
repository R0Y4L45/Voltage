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
    public void Delete(FriendList entity)
    {
        throw new NotImplementedException();
    }

    public async Task<FriendList> GetAsync(Expression<Func<FriendList, bool>> filter = null!) =>
        (await _context.FriendList?.FirstOrDefaultAsync(filter)!)!;

    public async Task<IEnumerable<FriendList>> GetListAsync(Expression<Func<FriendList, bool>> filter = null!) =>
        await Task.FromResult(filter == null ? _context.FriendList! : _context.FriendList?.Where(filter)!);

    public async Task<IEnumerable<UsersFriendListResult>> GetUsersSearchResult(string Id, SearchDto obj)
    {
        FormattableString cmd = 
            $@"SELECT users.UserName, 
                      users.Country,
                      users.Photo, 
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
               ORDER BY users.UserName
               OFFSET {obj.Skip} ROWS
               FETCH NEXT {4} ROWS ONLY";

        return await Task.Run(() => _context.UsersFriendListResults?.FromSqlInterpolated(cmd)!);
    }

    public bool Update(FriendList entity)
    {
        throw new NotImplementedException();
    }


}
