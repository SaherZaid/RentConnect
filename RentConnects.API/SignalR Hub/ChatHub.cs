using Microsoft.AspNetCore.SignalR;

namespace RentConnect.API.SignalR_Hub;

public class ChatHub : Hub
{
    public async Task SendMessage(string itemId, string senderId, string receiverId, string message)
    {
        string groupName = $"item-{itemId}";
        await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
    }

    public async Task JoinItemGroup(string itemId)
    {
        string groupName = $"item-{itemId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveItemGroup(string itemId)
    {
        string groupName = $"item-{itemId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}