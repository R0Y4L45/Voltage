using Microsoft.AspNetCore.Identity;

namespace Voltage.Entities.Entity;

public class User : IdentityUser
{
    public string? Photo { get; set; }
}
