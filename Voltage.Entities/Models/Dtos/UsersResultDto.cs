using Voltage.Entities.Entity;

namespace Voltage.Entities.Models.Dtos;

public class UsersResultDto
{
    public IEnumerable<UsersFriendListResult>? Users { get; set; }
    public int Count { get; set; }
    public bool Next { get; set; }
}
