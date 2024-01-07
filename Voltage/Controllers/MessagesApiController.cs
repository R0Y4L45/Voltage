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
    public async Task<IActionResult> TakeMessages([FromBody] TakeMessagesDto takeMsgDto)
    {
        if (takeMsgDto.UserName != null &&
            await _userManagerService.FindByNameAsync(User.Identity?.Name!) is User sender &&
            await _userManagerService.FindByNameAsync(takeMsgDto.UserName) is User rec)
            return Json(_mapper.Map<IEnumerable<MessageDto>>((await _messageService.GetListAsync(_ =>
                    _.ReceiverId == rec.Id && _.SenderId == sender.Id ||
                    _.ReceiverId == sender.Id && _.SenderId == rec.Id)).OrderBy(_ => _.CreatedTime)).SkipLast(takeMsgDto.Skip).TakeLast(9));

        return Json(new List<MessageDto>());
    }
}
