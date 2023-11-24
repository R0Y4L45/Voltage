using Voltage.Entities.Entity;

namespace Voltage.Entities.Models.Dtos;

public class UsersResultDto
{
    public IEnumerable<User>? Users { get; set; }
    public int Count { get; set; }
    public bool Next { get; set; }
}
