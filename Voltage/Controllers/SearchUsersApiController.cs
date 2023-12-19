using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("[controller]")]
[ApiController]
public class SearchUsersApiController : Controller
{
    private readonly IFriendListService _friendListService;
    private IEnumerable<UsersFriendListResult>? _list;
    private int _count;

    public SearchUsersApiController(IFriendListService friendListService)
    {
        _friendListService = friendListService;
    }

    [HttpPost("SearchUsers")]
    public async Task<IActionResult> SearchUsers([FromBody] SearchDto searchObj)
    {
        if (!string.IsNullOrEmpty(searchObj.Content))
        {
            if (_list == null)
            {
                _list = await _friendListService.GetUsersSearchResultAsync(User.Claims.First().Value, searchObj);
                _count = _list.Count();
            }

            bool next = searchObj.Skip + searchObj.Take < _count;

            return Json(new UsersResultDto
            {
                Users = _list.Skip(searchObj.Skip).Take(searchObj.Take),
                Count = _count,
                Next = next
            });
        }

        return Json(string.Empty);
    }
}
