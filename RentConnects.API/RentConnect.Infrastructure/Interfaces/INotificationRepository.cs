using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(Guid id, string userId);
    }
}
