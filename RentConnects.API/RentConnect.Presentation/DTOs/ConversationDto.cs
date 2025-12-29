namespace RentConnect.API.RentConnect.Presentation.DTOs;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public Guid ItemId { get; set; }
    public string ParticipantId { get; set; } = null!;
    public string Participant { get; set; } = null!;
    public string? LastMessage { get; set; }
    public DateTime? LastTimestamp { get; set; }
    public bool HasUnreadMessages { get; set; } // NEW
    public int UnreadCount { get; set; }
}