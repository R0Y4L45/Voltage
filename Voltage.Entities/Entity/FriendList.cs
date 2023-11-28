using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public enum Status { None, Pending, Accepted }
public class FriendList : IEntity
{
    public int Id { get; set; }
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public Status RequestStatus { get; set; } = Status.None;
    public DateTime RequestedDate { get; set; } = DateTime.Now;
    public DateTime? AcceptedDate { get; set; } = null!;

    //Nav props
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}