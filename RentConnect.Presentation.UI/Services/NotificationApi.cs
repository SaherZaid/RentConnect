using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;

namespace RentConnect.Presentation.UI.Services
{
    public class NotificationApi : INotificationApi
    {
        private readonly HttpClient _http;

        public NotificationApi(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<NotificationDto>> GetMyAsync()
            => await _http.GetFromJsonAsync<List<NotificationDto>>("api/Notification")
               ?? new List<NotificationDto>();

        public async Task<int> GetUnreadCountAsync()
            => await _http.GetFromJsonAsync<int>("api/Notification/unread-count");

        public async Task MarkReadAsync(Guid id)
        {
            var res = await _http.PatchAsync($"api/Notification/{id}/read", null);
            res.EnsureSuccessStatusCode();
        }
    }
}
