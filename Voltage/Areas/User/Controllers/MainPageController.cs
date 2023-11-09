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

    [HttpGet]
    public IActionResult Index()
    {
        TempData["ProfilePhoto"] = _signUpService.GetUserByName(User.Identity?.Name!).Result.Photo;

        return View();
    }

    [HttpGet]
    public IActionResult Message()
    {
        return View(_signUpService.GetAllUsers().Result);
    }

    [HttpGet]
    public IActionResult Profile()=>View();

    #region Mesages
    [HttpPost]
    public IActionResult GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return Json(_signUpService?.GetUserByName(name)?.Result.Id);

        return Json("null") ;
    }

    [HttpPost]
    public async Task<IActionResult> AcceptMessage([FromBody] MessageAcceptModel message)
    {
        var sender = (await _signUpService.GetUserByName(message.Sender!)).Id;
        var receiver = (await _signUpService.GetUserByName(message.Receiver!)).Id;
        var m = new Entities.Entity.Message
        {
            SenderId = sender,
            ReceiverId = receiver,
            Content = message.Message ?? string.Empty
        };

        _messageService.Add(m);

        return Json("null");
    }
    #endregion
}
