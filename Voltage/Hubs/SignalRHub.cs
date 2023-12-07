using Microsoft.AspNetCore.SignalR;

namespace Voltage.Hubs;

public class SignalRHub : Hub
{
    public async Task SendToUser(string userId, string message) =>
        await Clients.User(userId).SendAsync("ReceiveMessage", Context.User?.Identity?.Name, message, DateTime.Now);
    public async Task SendNotification(string userId, string message) =>
        await Clients.User(userId).SendAsync("ReceiveNotifications", Context.User?.Identity?.Name, message, DateTime.Now);
    public async Task SendRequest(string userId, string message) =>
        await Clients.User(userId).SendAsync("ReceiveRequests", Context.User?.Identity?.Name, message, DateTime.Now);
    public string GetConnectionId() => Context.UserIdentifier!;
    public string GetUserName() => Context.User?.Identity?.Name!;
}
