using Voltage.Entities.Entity;

namespace Voltage.Entities.Models.Dtos;

public class UsersResultDto
{
    //public IEnumerable<UserSearchDto>? Users { get; set; }
    public IEnumerable<User>? Users { get; set; }
    public int Count { get; set; }
    public bool Next { get; set; }
}
