using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace RentConnect.API.RentConnect.Infrastructure.Repositories
{
    public class NotificationRepository(ApiDbContext context) : INotificationRepository
    {
      
        public async Task AddAsync(Notification notification)
        {
            await context.Notifications.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetByUserAsync(string userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(Guid id, string userId)
        {
            var n = await context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (n != null && !n.IsRead)
            {
                n.IsRead = true;
            }
        }
    }
}
