using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;

namespace Voltage.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("[controller]")]
[ApiController]
public class RequestApiController : ControllerBase
{
    private readonly IUserManagerService _userManagerService;
    private readonly IFriendListService _friendListService;

    public RequestApiController(IUserManagerService userManagerService,
                                IFriendListService friendListService)
    {
        _userManagerService = userManagerService;
        _friendListService = friendListService;
    }

    [HttpPost("FriendshipRequest")]
    public async Task<IActionResult> FriendshipRequest([FromBody] string name)
    {
        if (User.Identity!.IsAuthenticated)
        {
            string sender = User.Claims.FirstOrDefault()?.Value!;

            if (await _userManagerService.FindByNameAsync(name) is User user)
            {
                if (await _friendListService.CheckFriendListAsync(sender, user.Id) is not FriendList friendList)
                {
                    await _friendListService.AddAsync(new FriendList
                    {
                        SenderId = sender,
                        ReceiverId = user.Id,
                        RequestStatus = Status.Pending
                    });

                    return Ok(Status.None);
                }

                return Ok(friendList.RequestStatus);
            }
        }

        return Ok(null);
    }

    [HttpPost("CancelRequest")]
    public async Task<IActionResult> CancelRequest([FromBody] string name)
    {
        if (User.Identity!.IsAuthenticated)
        {
            string sender = User.Claims.FirstOrDefault()?.Value!;

            if (await _userManagerService.FindByNameAsync(name) is User user &&
                await _friendListService.CheckFriendListAsync(sender, user.Id) is FriendList entity)
            {
                if (entity.SenderId == sender)
                {
                    if (entity.RequestStatus == Status.Pending)
                    {
                        await _friendListService.DeleteAsync(entity);
                        return Ok(0); //Cancel request
                    }
                    else if (entity.RequestStatus == Status.Accepted)
                        return Ok(1); //Request accepted

                    if (entity.RequestStatus == Status.Pending)
                        return Ok(2); //Pending request
                }
                else if (entity.SenderId == user.Id && entity.RequestStatus == Status.Pending)
                    return Ok(2);
            }
        }
        //Decline or not found such User
        return Ok(3);
    }

    [HttpPost("RemoveFriend")]
    public async Task<IActionResult> RemoveFriend([FromBody] string name)
    {
        if (User.Identity!.IsAuthenticated)
        {
            string sender = User.Claims.FirstOrDefault()!.Value;

            if (await _userManagerService.FindByNameAsync(name) is User user &&
                await _friendListService.CheckFriendListAsync(user.Id, sender) is FriendList entity)
            {
                if (entity.RequestStatus == Status.Accepted)
                {
                    await _friendListService.DeleteAsync(entity);
                    return Ok(true);
                }
                else if (entity.RequestStatus == Status.Pending)
                    return Ok(false);
            }
        }

        return Ok(true);
    }

    [HttpPost("AcceptRequest")]
    public async Task<IActionResult> AcceptRequest([FromBody] string name)
    {
        if (User.Identity!.IsAuthenticated)
        {
            string sender = User.Claims.FirstOrDefault()!.Value;

            if (await _userManagerService.FindByNameAsync(name) is User user)
                if (await _friendListService.CheckFriendListAsync(sender, user.Id) is FriendList entity)
                    if (entity.SenderId == user.Id && entity.RequestStatus == Status.Pending)
                        return await Task.Run(() =>
                        {
                            entity.AcceptedDate = DateTime.Now;
                            entity.RequestStatus = Status.Accepted;
                            _friendListService.Update(entity);
                            return Ok(true);
                        });
        }

        return Ok(false);
    }

    [HttpPost("DeclineRequest")]
    public async Task<IActionResult> DeclineRequest([FromBody] string name)
    {
        if (User.Identity!.IsAuthenticated)
        {
            string sender = User.Claims.FirstOrDefault()!.Value;

            if (await _userManagerService.FindByNameAsync(name) is User user)
                if (await _friendListService.CheckFriendListAsync(sender, user.Id) is FriendList entity)
                    if (entity.SenderId == user.Id && entity.RequestStatus == Status.Pending)
                    {
                        await _friendListService.DeleteAsync(entity);
                        return Ok(true);
                    }
        }

        return Ok(false);
    }
}
