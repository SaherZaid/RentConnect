using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.API.SignalR_Hub;
using RentConnect.API.IService;
using Microsoft.AspNetCore.SignalR;

namespace RentConnect.API.RentConnect.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task CreateAsync(
            string userId,
            string title,
            string message,
            string? link = null)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            var dto = new NotificationDto
            {
                Id = notification.Id,
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = notification.CreatedAt
            };

            // push realtime
            await _hubContext
                .Clients
                .Group($"user-{userId}")
                .SendAsync("ReceiveNotification", dto);
        }
    }
}
