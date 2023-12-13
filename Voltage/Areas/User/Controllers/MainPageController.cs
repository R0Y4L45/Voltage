using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.CustomHelpers;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;
using Voltage.Entities.Models.HelperModels;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    private readonly IUserManagerService _userManagerService;
    private readonly IMessageService _messageService;
    private readonly IEmailService _emailService;
    private readonly IFriendListService _friendListService;
    private readonly IMapper _mapper;
    private IEnumerable<UsersFriendListResult>? _list;
    private int _count;

    public MainPageController(IMessageService messageService, IMapper mapper,
        IUserManagerService userManagerService,
        IEmailService emailConfiguration,
        IFriendListService friendListService)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userManagerService = userManagerService;
        _emailService = emailConfiguration;
        _friendListService = friendListService;
    }

    #region ActionMethods
    public async Task<IActionResult> Index()
    {
        string? profilePhoto = await _userManagerService.GetProfilePhotoAsync(User.Identity!.Name!);
        ViewBag.UserManagerService = _userManagerService;
        if (profilePhoto != null)
            HttpContext.Session.SetString("ProfilePhoto", profilePhoto);
        return View();
    }

    public IActionResult MessageMobile() => View();
    public IActionResult FriendshipRequests() => View();
    public IActionResult SearchUsers() => View();

    public async Task<IActionResult> Message() => View(await _userManagerService.GetAllUsers());
    public async Task<IActionResult> Profile(string Id)
    {
        var user = await _userManagerService.FindByIdAsync(Id);
        if (user == null)
            return NotFound();
        var viewmodel = new EditProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            PhotoUrl = user.Photo,
        };

        return View(viewmodel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(EditProfileViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManagerService.FindByIdAsync(viewModel.Id!);
                    if (user == null)
                        return NotFound();

                    var oldEmail = user.Email;

                    user.UserName = viewModel.UserName;
                    user.DateOfBirth = viewModel.DateOfBirth;
                    user.Photo = (viewModel.Photo != null) ? await UploadFileHelper.UploadFile(viewModel.Photo) : "https://t4.ftcdn.net/jpg/00/64/67/63/360_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";

                    if (user.Email != viewModel.Email)
                    {
                        user.EmailConfirmed = false;
                        string? token = await _userManagerService.GenerateEmailTokenAsync(await _userManagerService.FindByEmailAsync(user.Email)),
                        callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = viewModel.Email }, Request.Scheme);

                        E_Message message = new E_Message(new string[] { viewModel.Email }, "Confirmation Email Link", callbackUrl!);
                        _emailService.SendEmail(message);
                        user.Email = viewModel.Email;
                        await _userManagerService.UpdateAsync(user);
                        return Redirect("Index");
                        //string? token = await _userManagerService.GenerateEmailTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "User", token, email = viewModel.Email }, Request.Scheme);
                        //E_Message message = new E_Message(new string[] { viewModel.Email }, "Confirmation Email Link", callbackUrl!);
                        //_emailService.SendEmail(message);
                        //if (await _userManagerService.IsEmailConfirmedAsync(user))
                        //    user.Email = viewModel.Email;
                        //
                        //else
                        //    user.Email = oldEmail;
                    }
                    await _userManagerService.UpdateAsync(user);
                    return RedirectToAction("Profile", new { name = viewModel.UserName });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public IActionResult Settings() => View();

    #endregion

    #region Messages Api

    [HttpPost]
    public async Task<IActionResult> GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return Json((await _userManagerService.FindByNameAsync(name)).Id);

        return Json("null");
    }

    [HttpPost]
    public async Task<IActionResult> GetUser([FromBody] string? name = null)
    {
        if (name == "+")
        {
            var d = await _friendListService.GetUserDtoByNameAsync(User.Identity?.Name!);
            return Json(d);
        }
        else if (name != null)
            return Json(await _friendListService.GetUserDtoByNameAsync(name));

        return Json(string.Empty);
    }

    [HttpPost]
    public async Task<IActionResult> MessageSaver([FromBody] MessageDto message)
    {
        if (message.Sender != null && message.Receiver != null && message.Content != null)
            return Json(await _messageService.AddAsync(new Message
            {
                SenderId = message.Sender,
                ReceiverId = message.Receiver,
                Content = message.Content
            }));

        return await Task.FromResult(Json(0));
    }

    [HttpPost]
    public async Task<IActionResult> TakeMessages([FromBody] string receiver)
    {
        string[] arr = receiver.Split(' ');
        string senderId = (await _userManagerService.FindByNameAsync(User.Identity?.Name!)).Id,
            recId = (await _userManagerService.FindByNameAsync(arr[0])).Id;

        var d = _mapper.Map<IEnumerable<MessageDto>>((await _messageService.GetListAsync(_
            => _.ReceiverId == recId && _.SenderId == senderId
                || _.ReceiverId == senderId && _.SenderId == recId)).OrderBy(_ => _.CreatedTime)).Take(int.Parse(arr[1]));
        return Json(d);
    }

    [HttpPost]
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

    [HttpPost]
    public async Task<IActionResult> FriendshipRequest([FromBody] string name)
    {
        string sender = User.Claims.FirstOrDefault()?.Value!,
            receiver = (await _userManagerService.FindByNameAsync(name)).Id;

        if (receiver != null && !await _friendListService.CheckRequestAsync(sender, receiver))
        {
            await _friendListService.AddAsync(new FriendList
            {
                SenderId = sender,
                ReceiverId = receiver,
                RequestStatus = Status.Pending
            });

            return Json(true);
        }

        return Json(false);
    }

    [HttpPost]
    public async Task<IActionResult> CancelRequest([FromBody] string name)
    {
        string sender = User.Claims.FirstOrDefault()?.Value!,
            receiver = (await _userManagerService.FindByNameAsync(name)).Id;

        FriendList entity = await _friendListService.GetAsync(_ => _.SenderId == sender &&
                                                                   _.ReceiverId == receiver &&
                                                                   _.RequestStatus == Status.Pending);

        if (entity != null)
        {
            await _friendListService.DeleteAsync(entity);
            return Json(true);
        }

        return Json(false);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFriend([FromBody] string name)
    {
        string sender = User.Claims.FirstOrDefault()?.Value!,
            receiver = (await _userManagerService.FindByNameAsync(name)).Id;

        FriendList entity = await _friendListService.GetAsync(_ => _.SenderId == sender &&
        _.ReceiverId == receiver &&
        _.RequestStatus == Status.Accepted || _.SenderId == receiver &&
        _.ReceiverId == sender &&
        _.RequestStatus == Status.Accepted);

        if (entity != null)
        {
            await _friendListService.DeleteAsync(entity);
            return Json(true);
        }

        return Json(false);
    }

    [HttpPost]
    public async Task<IActionResult> AcceptRequest([FromBody] string name)
    {
        string sender = User.Claims.FirstOrDefault()?.Value!,
            receiver = (await _userManagerService.FindByNameAsync(name)).Id;

        FriendList entity = await _friendListService.GetAsync(_ => _.SenderId == receiver &&
        _.ReceiverId == sender &&
        _.RequestStatus == Status.Pending);

        if (entity != null)
        {
            entity.RequestStatus = Status.Accepted;
            entity.AcceptedDate = DateTime.Now;

            return await Task.Run(() => Json(_friendListService.Update(entity)));
        }

        return Json(false);
    }

    [HttpPost]
    public async Task<IActionResult> DeclineRequest([FromBody] string name)
    {
        string sender = User.Claims.FirstOrDefault()?.Value!,
            receiver = (await _userManagerService.FindByNameAsync(name)).Id;

        FriendList entity = await _friendListService.GetAsync(_ => _.SenderId == receiver &&
        _.ReceiverId == sender &&
        _.RequestStatus == Status.Pending);

        if (entity != null)
        {
            await _friendListService.DeleteAsync(entity);
            return Json(true);
        }

        return Json(false);
    }

    [HttpPost]
    public async Task<IActionResult> GetRequestList() => 
        Json(await _friendListService.GetUsersByRequestAsync(User.Claims.FirstOrDefault()?.Value!));

    #endregion
}