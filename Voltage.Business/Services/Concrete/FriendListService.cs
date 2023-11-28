using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

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
    
    public bool Update(FriendList entity)
    {
        throw new NotImplementedException();
    }

}
