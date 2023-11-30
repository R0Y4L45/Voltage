using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class UsersFriendListResult : IEntity
{
    public string? UserName { get; set; }
    public string? Country { get; set; }
    public string? Photo { get; set; }
    public string? SenderName { get; set; }
    public string? SenderId { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverId { get; set; }
    public Status? RequestStatus { get; set; }
    public DateTime? RequestedDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
}
