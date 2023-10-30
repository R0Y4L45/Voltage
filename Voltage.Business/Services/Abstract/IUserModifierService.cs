using Voltage.Core.DataAccess;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract;

public interface IUserModifierService : IEntityRepository<User, VoltageDbContext> { }
