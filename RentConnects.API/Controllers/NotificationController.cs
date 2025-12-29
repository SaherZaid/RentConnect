using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.API.SignalR_Hub;
using System.Security.Claims;


namespace RentConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        // ✅ GET: api/Notification?userId=xxx
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");

            var notifications = await _unitOfWork.NotificationRepository.GetByUserAsync(userId);

            var dtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });

            return Ok(dtos);
        }

        // ✅ GET: api/Notification/unread-count?userId=xxx
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");

            var count = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        // ✅ PATCH: api/Notification/{id}/read?userId=xxx
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");

            await _unitOfWork.NotificationRepository.MarkAsReadAsync(id, userId);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // ✅ (مهم) Endpoint اختبار للتأكد إن الـ Hub شغال + البوش شغال
        // POST: api/Notification/test?userId=xxx
        [HttpPost("test")]
        public async Task<IActionResult> TestPush([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");

            await CreateAndPushNotificationAsync(
                userId,
                "Test notification",
                "If you see this, SignalR is working ✅",
                "/items"
            );

            return Ok("Pushed");
        }

        // ✅ helper داخلي
        [NonAction]
        public async Task CreateAndPushNotificationAsync(
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
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            var dto = new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Link = notification.Link,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            await _hubContext.Clients.Group($"user-{userId}")
                .SendAsync("ReceiveNotification", dto);
        }
    }
}

