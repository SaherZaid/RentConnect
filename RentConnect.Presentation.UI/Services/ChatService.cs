using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.Services;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MessageDto>> GetMessagesByItemAsync(Guid itemId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<MessageDto>>($"api/chat/{itemId}");
        return result ?? new List<MessageDto>();
    }

    public async Task<List<MessageDto>> GetMessagesByConversationIdAsync(Guid conversationId, string userId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<MessageDto>>(
            $"api/chat/conversation/{conversationId}?userId={userId}");
        return result ?? new List<MessageDto>();
    }

    public async Task SendMessageAsync(MessageDto message)
    {
        var response = await _httpClient.PostAsJsonAsync("api/chat", message);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> GetUnreadConversationsCountAsync(string userId)
    {
        var result = await _httpClient.GetFromJsonAsync<int>($"api/chat/unreadcount/{userId}");
        return result;
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ConversationDto>>($"api/chat/user/{userId}");
        return result ?? new List<ConversationDto>();
    }

    public async Task<ConversationDto> GetOrCreateConversationAsync(Guid itemId, string senderId, string receiverId)
    {
        var response = await _httpClient.GetFromJsonAsync<ConversationDto>(
            $"api/chat/getorcreate?itemId={itemId}&user1Id={senderId}&user2Id={receiverId}");

        return response!;
    }

    public async Task UpdateMessageAsync(Guid messageId, string newContent)
    {
        var payload = new { content = newContent };
        var response = await _httpClient.PutAsJsonAsync($"api/chat/message/{messageId}", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var response = await _httpClient.DeleteAsync($"api/chat/message/{messageId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteConversationAsync(Guid conversationId)
    {
        var response = await _httpClient.DeleteAsync($"api/chat/conversation/{conversationId}");
        response.EnsureSuccessStatusCode();
    }

}