using Microsoft.AspNetCore.Identity;
using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class User : IdentityUser, IEntity
{
    public string? Photo { get; set; }
    public string? Country { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    //Nav prop
    public ICollection<Message>? SentMessages { get; set; }
    public ICollection<Message>? ReceivedMessages { get; set; }

    public ICollection<FriendList>? SenderRequest { get; set; }
    public ICollection<FriendList>? ReceiverRequest { get; set; }

    public ICollection<Notification>? Receipent { get; set; }
}
