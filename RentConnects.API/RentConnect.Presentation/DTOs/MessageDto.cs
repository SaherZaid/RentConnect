namespace RentConnect.API.RentConnect.Presentation.DTOs;

public class MessageDto
{
    public Guid ItemId { get; set; }
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}