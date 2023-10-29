using Microsoft.AspNetCore.SignalR;

namespace Voltage.Hubs;

public class NotificationHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", Context.User!.Identity!.Name, message);
        await Console.Out.WriteLineAsync();
    }
    public async Task SendToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", Context.User!.Identity!.Name, message);
    }
    public string GetConnectionId() => Context.UserIdentifier!;
}
