using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Controllers;

[Route("[controller]")]
[ApiController]
public class MessagesApiController : Controller
{
    private readonly IUserManagerService _userManagerService;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;

    public MessagesApiController(IUserManagerService userManagerService, 
                                 IMessageService messageService,
                                 IMapper mapper)
    {
        _userManagerService = userManagerService;
        _messageService = messageService;
        _mapper = mapper;
    }

    [HttpPost("MessageSaver")]
    public async Task<IActionResult> MessageSaver([FromBody] MessageDto message)
    {
        if (message.Sender != null && message.Receiver != null && message.Content != null)
            return Json(await _messageService.AddAsync(new Message
            {
                SenderId = message.Sender,
                ReceiverId = message.Receiver,
                Content = message.Content
            }));

        return Json(0);
    }

    [HttpPost("TakeMessages")]
    public async Task<IActionResult> TakeMessages([FromBody] string receiver)
    {
        string[] arr = receiver.Split(' ');
        string senderId = (await _userManagerService.FindByNameAsync(User.Identity?.Name!) ?? new User()).Id,
            recId = (await _userManagerService.FindByNameAsync(arr[0]) ?? new User()).Id;

        return Json(_mapper.Map<IEnumerable<MessageDto>>((await _messageService.GetListAsync(_ =>
            _.ReceiverId == recId && _.SenderId == senderId
                || _.ReceiverId == senderId && _.SenderId == recId)).OrderBy(_ => _.CreatedTime)).Take(int.Parse(arr[1])));
    }
}
