namespace RentConnect.API.RentConnect.Domain.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        // صاحب الإشعار
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; }

        // محتوى الإشعار
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;

        // رابط لما يضغط على الإشعار (مثلاً /bookings أو /messages?id=...)
        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
