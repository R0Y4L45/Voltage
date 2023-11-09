using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Models;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    private readonly ISignUpService _signUpService;
    private readonly IMessageService _messageService;

    public MainPageController(ISignUpService signUpService, IMessageService messageService)
    {
        _signUpService = signUpService;
        _messageService = messageService;
    }

    public IActionResult Index()
    {
        TempData["ProfilePhoto"] = _signUpService.GetUserByName(User.Identity?.Name!).Result.Photo;

        return View();
    }

    public IActionResult Message()
    {
        return View(_signUpService.GetAllUsers().Result);
    }

    [HttpPost]
    public IActionResult GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return Json(_signUpService?.GetUserByName(name)?.Result.Id);

        return Json("null");
    }

    [HttpPost]
    public async Task<IActionResult> AcceptMessage([FromBody] MessageAcceptModel message)
    {
        if (message.Sender != null && message.Receiver != null)
            return Json(await _messageService.AddAsync(new Entities.Entity.Message
            {
                SenderId = message.Sender,
                ReceiverId = message.Receiver,
                Content = message.Message ?? string.Empty
            }));

        return await Task.FromResult(Json(string.Empty));
    }

    [HttpPost]
    public async Task<IActionResult> TakeMessages([FromBody] string receiver)
    {
        string senderId = (await _signUpService.GetUserByName(User.Identity?.Name!)).Id,
            recId = (await _signUpService.GetUserByName(receiver)).Id;

        return Json(await _messageService.GetListAsync(_ => _.ReceiverId == recId && _.SenderId == senderId));
    }
}
