using Microsoft.AspNetCore.Identity;
using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class User : IdentityUser, IEntity
{
    public string? Photo { get; set; }
    public DateTime DateOfBirth { get; set; }
}
