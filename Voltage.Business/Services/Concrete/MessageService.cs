using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Concrete;

public class MessageService : IMessageService
{
    private readonly VoltageDbContext _context;

    public MessageService(VoltageDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Message entity)
    {
        _context?.AddAsync(entity);
        return await _context!.SaveChangesAsync();
    }


    public void Delete(Message entity)
    {
        throw new NotImplementedException();
    }

    public Message Get(Expression<Func<Message, bool>> filter = null!)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Message>> GetListAsync(Expression<Func<Message, bool>> filter = null!)
        =>await Task.FromResult(filter == null ? _context.Message! : _context.Message!.Where(filter));

    public bool Update(Message entity)
    {
        throw new NotImplementedException();
    }
}
