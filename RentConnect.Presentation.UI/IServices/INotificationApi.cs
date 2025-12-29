using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices
{
    public interface INotificationApi
    {
        Task<IEnumerable<NotificationDto>> GetMyAsync();
        Task<int> GetUnreadCountAsync();
        Task MarkReadAsync(Guid id);
    }
}
