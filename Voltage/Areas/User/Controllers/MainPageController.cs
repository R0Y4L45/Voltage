using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.CustomHelpers;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Models.HelperModels;
using Voltage.Entities.Models.ViewModels;
using UserEntity = Voltage.Entities.Entity.User;

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
        ViewData["ShowFooter"] = false;
        string? profilePhoto = await _userManagerService.GetProfilePhotoAsync(User.Identity!.Name!);
        ViewBag.UserManagerService = _userManagerService;
        if (profilePhoto != null)
            HttpContext.Session.SetString("ProfilePhoto", profilePhoto);
        return View();
    }

    public IActionResult WebRTC() => View();

    public IActionResult FriendshipRequests()
    {
        ViewData["ShowFooter"] = false;
        return View();
    }
    public IActionResult SearchUsers()
    {
        ViewData["ShowFooter"] = false;
        return View();
    }

    public async Task<IActionResult> Message()
    {
        ViewData["ShowFooter"] = false;
        return View(await _userManagerService.GetAllUsers());
    }
    public async Task<IActionResult> Profile(string Id)
    {
        if (await _userManagerService.FindByIdAsync(Id) is UserEntity user)
            return View(new EditProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                PhotoUrl = user.Photo,
            });
            
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Profile(EditProfileViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
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

                        _emailService.SendEmail(new E_Message(new string[] { viewModel.Email }, "Confirmation Email Link", callbackUrl!));
                        user.Email = viewModel.Email;
                        await _userManagerService.UpdateAsync(user);
                        return Redirect("Index");
                    }
                    await _userManagerService.UpdateAsync(user);
                    return RedirectToAction("Profile", new { name = viewModel.UserName });
                }
                catch (Exception)
                {
                    throw;
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
}