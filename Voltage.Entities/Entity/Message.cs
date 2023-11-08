using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class Message : IEntity
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; } = DateTime.Now;

    //Nav props
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}
