using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace RentConnect.API.SignalR_Hub
{
    public class NotificationHub : Hub
    {
        // ينضم لغرفة باسم المستخدم
        public Task JoinUserGroup(string userId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
    }
}
