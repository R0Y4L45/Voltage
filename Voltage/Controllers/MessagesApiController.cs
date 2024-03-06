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

    /// <summary>
    /// This Api return ordered MessageDto's Dictionary which keys are created time
    /// </summary>
    /// <param name="takeMsgDto"></param>
    /// <returns></returns>
    [HttpPost("TakeMessages")]
    public async Task<IActionResult> TakeMessages([FromBody] TakeMessagesDto takeMsgDto)
    {
        if (takeMsgDto.UserName != null &&
            await _userManagerService.FindByNameAsync(User.Identity?.Name!) is User sender &&
            await _userManagerService.FindByNameAsync(takeMsgDto.UserName) is User rec)
        {
            Dictionary<string, List<MessageDto>> groupedDic = _mapper.Map<IEnumerable<MessageDto>>(await _messageService.GetListAsync(message => //return IEnumerable<Message>
                    message.ReceiverId == rec.Id && message.SenderId == sender.Id ||
                    message.ReceiverId == sender.Id && message.SenderId == rec.Id))
                .OrderByDescending(mesDto => mesDto.CreatedTime) //Ordered by DESC by createdTime
                .Skip(takeMsgDto.Skip) //Skip ordered messages of list according to parameter(takeMsgDto.Skip)
                .Take(9) //Take 9 message to send client 
                .GroupBy(_ => _.CreatedTime.ToShortDateString()) //Grouped IEnumerable<MessageDto> according to createdTime. Return IGrouping<string, MessageDto> 
                .ToDictionary(_ => _.Key, _ => _.ToList()); //IGrouping<string, MessageDto> was converted to dictionary

            return Json(groupedDic); //return dictionary which Converted to JSON
        }

        return Json(new Dictionary<string, List<MessageDto>>()); //return empty dictionary which Converted to JSON
    }
}
