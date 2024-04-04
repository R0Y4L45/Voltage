using Microsoft.AspNetCore.SignalR;

namespace Voltage.Hubs;

public class SignalRHub : Hub
{

    private static Dictionary<string, string> activeUsers = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        string userId = Context.UserIdentifier!;
        string userName = Context.User!.Identity!.Name!;

        if (!activeUsers.ContainsKey(userId))
        {
            activeUsers.Add(userId, userName);
            await Clients.All.SendAsync("UserConnected", userName);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier!;
        string? userName = activeUsers.GetValueOrDefault(userId);

        if (userName != null)
        {
            activeUsers.Remove(userId);
            await Clients.All.SendAsync("UserDisconnected", userName);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendTypingNotification(string userName, string messageText) =>
        await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveTypingNotification", userName, messageText);


    public async Task SendToUser(string userId, string message) =>
        await Clients.User(userId).SendAsync("ReceiveMessage", Context.User?.Identity?.Name, message, DateTime.Now);
    public async Task SendNotification(string userId, string message) =>
        await Clients.User(userId).SendAsync("ReceiveNotifications", Context.User?.Identity?.Name, message, DateTime.Now);
    public async Task SendRequest(string id, object user, string status) =>
        await Clients.User(id).SendAsync("ReceiveRequests", user, status);
    public string GetConnectionId() => Context.UserIdentifier!;
    public string GetUserName() => Context.User?.Identity?.Name!;
}