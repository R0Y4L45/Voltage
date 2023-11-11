using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    private readonly IUserManagerService _userManagerService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;

    public MainPageController(IMessageService messageService, IMapper mapper, IUserManagerService userManagerService)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userManagerService = userManagerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        TempData["ProfilePhoto"] = (await _userManagerService.FindByNameAsync(User.Identity?.Name!)).Photo;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Message()
    {
        return View(await _userManagerService.GetAllUsers());
    }

    [HttpGet]
    public IActionResult Profile() => View();

    [HttpGet]
    public IActionResult Settings() => View();

    #region Messages Api

    [HttpPost]
    public async Task<IActionResult> GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return Json((await _userManagerService.FindByNameAsync(name)).Id);

        return Json("null");
    }

    [HttpPost]
    public async Task<IActionResult> AcceptMessage([FromBody] MessageDto message)
    {
        if (message.Sender != null && message.Receiver != null && message.Content != null)
            return Json(await _messageService.AddAsync(new Entities.Entity.Message
            {
                SenderId = message.Sender,
                ReceiverId = message.Receiver,
                Content = message.Content
            }));

        return await Task.FromResult(Json(string.Empty));
    }

    [HttpPost]
    public async Task<IActionResult> TakeMessages([FromBody] string receiver)
    {
        string senderId = (await _userManagerService.FindByNameAsync(User.Identity?.Name!)).Id,
            recId = (await _userManagerService.FindByNameAsync(receiver)).Id;

        var d = (await _messageService.GetListAsync(_ => _.ReceiverId == recId && _.SenderId == senderId)).ToList();
        List<MessageDto> messages = new List<MessageDto>();
        foreach (var item in d)
            messages.Add(_mapper.Map<MessageDto>(item));

        return Json(messages);
    }

    #endregion
}
