namespace Voltage.Entities.Models;

public class MessageDto
{
    public string? Content { get; set; }
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    public DateTime CreatedTime { get; set; }
}
