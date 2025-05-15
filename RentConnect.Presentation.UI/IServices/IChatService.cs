using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface IChatService
{
    Task<List<MessageDto>> GetMessagesByItemAsync(Guid itemId);
    Task<List<MessageDto>> GetMessagesByConversationIdAsync(Guid conversationId, string userId);
    Task SendMessageAsync(MessageDto message);
    Task<int> GetUnreadConversationsCountAsync(string userId);
    Task<List<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<ConversationDto> GetOrCreateConversationAsync(Guid itemId, string senderId, string receiverId);
    //Task<List<MessageDto>> GetMessagesByConversationIdAsync(Guid conversationId);
}