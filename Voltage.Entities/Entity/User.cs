using Microsoft.AspNetCore.Identity;
using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class User : IdentityUser, IEntity
{
    public string? Photo { get; set; }
    public DateOnly? DateOfBirth { get; set; } = null!;
}
