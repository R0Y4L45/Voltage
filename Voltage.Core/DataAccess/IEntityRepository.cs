using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Voltage.Core.Abstract;

namespace Voltage.Core.DataAccess;

public interface IEntityRepository<TEntity, TContext> 
    where TEntity : class, IEntity, new()
    where TContext : DbContext
{
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null!);
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null!);
    Task<int> AddAsync(TEntity entity);
    void Delete(TEntity entity);
    bool Update(TEntity entity);
}
