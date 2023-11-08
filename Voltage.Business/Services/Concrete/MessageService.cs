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

    public void Add(Message entity)
    {
        _context?.Add(entity);
        _context?.SaveChanges();
    }

    public void Delete(Message entity)
    {
        throw new NotImplementedException();
    }

    public Message Get(Expression<Func<Message, bool>> filter = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Message> GetList(Expression<Func<Message, bool>> filter = null)
    {
        throw new NotImplementedException();
    }

    public bool Update(Message entity)
    {
        throw new NotImplementedException();
    }
}
