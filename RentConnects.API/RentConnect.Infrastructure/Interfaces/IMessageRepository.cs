using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessagesByItemAsync(Guid itemId);
    Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkMessagesAsReadAsync(Guid conversationId, string userId);
    Task AddMessageAsync(Message message);
    Task<Message?> GetByIdAsync(Guid id);
    Task DeleteMessageAsync(Message message);

}