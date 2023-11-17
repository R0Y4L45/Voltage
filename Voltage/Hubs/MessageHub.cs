using Microsoft.AspNetCore.SignalR;

namespace Voltage.Hubs;

public class MessageHub : Hub
{
    //public async Task SendMessage(string message)
    //{
    //    await Clients.All.SendAsync("ReceiveMessage", Context.User!.Identity!.Name, message);
    //}
    public async Task SendToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", Context.User?.Identity?.Name, message, DateTime.Now);
    }
    public string GetConnectionId() => Context.UserIdentifier!;
    public string GetUserName() => Context.User?.Identity?.Name!;
}
