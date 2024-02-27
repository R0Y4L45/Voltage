using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("[controller]")]
[ApiController]
public class UserInfoController : ControllerBase
{
    private readonly IUserManagerService _userManagerService;
    private readonly IFriendListService _friendListService;

    public UserInfoController(IUserManagerService userManagerService,
                              IFriendListService friendListService)
    {
        _userManagerService = userManagerService;
        _friendListService = friendListService;
    }

    [HttpGet("GetId")]
    public async Task<IActionResult> GetUserId(string? name = null)
    {
        if (User.Identity!.IsAuthenticated && name != null &&
            await _userManagerService.FindByNameAsync(name) is User user)
            return Ok(user.Id);

        return NotFound();
    }

    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser(string? name = null)
    {
        if (User.Identity!.IsAuthenticated && name != null && await _friendListService.GetUserDtoByNameAsync(name) is UserDto uDto)
            return Ok(uDto);
        return NotFound();
    }


    [HttpGet("GetMyFriend")]
    public async Task<IActionResult> GetMyFriend(string id, string? requestStatus = null)
    {
        if (User.Identity!.IsAuthenticated && await _friendListService.GetFriendListAsync(id, requestStatus!) is List<FriendListItemDto> friendList)
            return Ok(friendList);
        return NotFound();
    }




    [HttpGet("GetRequestList")]
    public async Task<IActionResult> GetRequestList()
    {
        if (User.Identity!.IsAuthenticated &&
            await _friendListService.GetUsersByRequestAsync(User.Claims.FirstOrDefault()!.Value) is IEnumerable<UserDto> userDto)
            return Ok(userDto);

        return NotFound();
    }
}
