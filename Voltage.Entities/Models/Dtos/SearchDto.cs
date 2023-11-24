namespace Voltage.Entities.Models.Dtos;

public class SearchDto
{
    public string? Content { get; set; }
    public int Take { get; set; } = 8;
    public int Skip { get; set; } = 0;
}
