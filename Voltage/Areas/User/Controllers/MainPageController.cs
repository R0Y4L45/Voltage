﻿using AutoMapper;
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
    private readonly IMapper _mapper;

    public MainPageController(IMessageService messageService, IMapper mapper, 
        IUserManagerService userManagerService, IEmailService emailConfiguration)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userManagerService = userManagerService;
        _emailService = emailConfiguration;
    }

    public IActionResult Index() => View();
    public IActionResult FollowUser() => View();
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

    #region Messages Api

    [HttpPost]
    public async Task<IActionResult> GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return Json((await _userManagerService.FindByNameAsync(name)).Id);

        return Json("null");
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
    public async Task<IActionResult> FindUsers([FromBody] string letter)
    {
        string[] arr = letter.Split(' ');

        if (arr[0] != string.Empty)
            return Json((await _userManagerService.GetAllUsers(_ => _.UserName.Contains(arr[0]))).OrderBy(_ => _.UserName).Take(int.Parse(arr[1])));

        return Json(false);
    }

    #endregion
}
