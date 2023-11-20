using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public enum Status { Pending = 1, Accepted, Rejected}
public class FriendList : IEntity
{
    public int Id { get; set; }
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public Status? ReqeustStatus { get; set; } = null!;
    public DateTime RequestedDate { get; set; }
    public DateTime? AcceptedDate { get; set; } = null!;

    //Nav props
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}