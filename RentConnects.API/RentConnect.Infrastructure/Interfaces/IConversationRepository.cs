using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetOrCreateAsync(Guid itemId, string user1Id, string user2Id);
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
    Task DeleteAsync(Guid conversationId);
}