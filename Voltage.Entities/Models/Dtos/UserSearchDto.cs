using Voltage.Entities.Entity;

namespace Voltage.Entities.Models.Dtos;

public class UserSearchDto
{
    public string? UserName { get; set; }
    public string? Sender { get; set; }
    public string? Receiver { get; set; }
    public Status Stat { get; set; }
    public string? Photo { get; set; }
    public string? Country { get; set; }
}