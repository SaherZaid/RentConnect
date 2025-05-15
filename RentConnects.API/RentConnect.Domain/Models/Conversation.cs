namespace RentConnect.API.RentConnect.Domain.Models;

public class Conversation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public string User1Id { get; set; } = null!;
    public string User2Id { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}