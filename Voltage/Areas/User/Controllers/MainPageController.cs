using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Models.Dtos;
using Voltage.Entities.Models.ViewModels;

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
    public async Task<IActionResult> Profile(string Id)
    {
        var user = await _userManagerService.FindByIdAsync(Id);
        if (user == null)
            return NotFound();
        var viewmodel = new EditProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email
        };
        return View(viewmodel);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(EditProfileViewModel viewModel) 
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManagerService.FindByIdAsync(viewModel.Id);
                if (user == null)
                    return NotFound();

                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;

                var result = await _userManagerService.UpdateAsync(user);
                if(!result.Succeeded)
                {
                    return BadRequest();
                }
                return RedirectToAction("Profile", new { name = viewModel.UserName });
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

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

        List<MessageDto> messages = _mapper.Map<List<MessageDto>>((await _messageService.GetListAsync(_
            => _.ReceiverId == recId && _.SenderId == senderId
                || _.ReceiverId == senderId && _.SenderId == recId)).OrderBy(_ => _.CreatedTime));

        return Json(messages);
    }

    #endregion
}
