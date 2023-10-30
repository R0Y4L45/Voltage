using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Voltage.Core.Abstract;

namespace Voltage.Core.DataAccess;

public interface IEntityRepository<TEntity, TContext> 
    where TEntity : class, IEntity, new()
    where TContext : DbContext
{
    TEntity Get(Expression<Func<TEntity, bool>> filter = null!);
    IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null!);
    void Add(TEntity entity);
    void Delete(TEntity entity);
    bool Update(TEntity entity);
}
