using Voltage.Core.Abstract;

namespace Voltage.Entities.Entity;

public class Notification : IEntity
{
    public int Id { get; set; }
    //public string SenderId { get; set; } = null!;
    public string ReceipentId { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime CreatedTime { get; set; }
    public bool IsRead { get; set; }
    
    public User? Receipent { get; set; }
}
