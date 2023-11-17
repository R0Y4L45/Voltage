namespace Voltage.Entities.Models.Dtos;

public class MessageDto
{
    public string? Sender { get; set; }
    public string? Receiver { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedTime { get; set; }
}
