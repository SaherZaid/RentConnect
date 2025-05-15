using Microsoft.AspNetCore.Mvc;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{itemId}")]
    public async Task<IActionResult> GetMessages(Guid itemId)
    {
        var messages = await _unitOfWork.MessageRepository.GetMessagesByItemAsync(itemId);
        var dtos = messages.Select(m => new MessageDto
        {
            ItemId = m.ItemId,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Content = m.Content,
            Timestamp = m.Timestamp
        });

        return Ok(dtos);
    }

    [HttpGet("conversation/{conversationId}")]
    public async Task<IActionResult> GetMessagesByConversation(Guid conversationId, [FromQuery] string userId)
    {
        await _unitOfWork.MessageRepository.MarkMessagesAsReadAsync(conversationId, userId);
        var messages = await _unitOfWork.MessageRepository.GetMessagesByConversationIdAsync(conversationId);

        var dtos = messages.Select(m => new MessageDto
        {
            ItemId = m.ItemId,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Content = m.Content,
            Timestamp = m.Timestamp
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(MessageDto dto)
    {
        var conversation = await _unitOfWork.ConversationRepository
            .GetOrCreateAsync(dto.ItemId, dto.SenderId, dto.ReceiverId);

        var msg = new Message
        {
            ConversationId = conversation!.Id,
            ItemId = dto.ItemId,
            SenderId = dto.SenderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content,
            Timestamp = DateTime.UtcNow
        };

        await _unitOfWork.MessageRepository.AddMessageAsync(msg);
        await _unitOfWork.CompleteAsync();

        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserConversations(string userId)
    {
        var conversations = await _unitOfWork.ConversationRepository.GetUserConversationsAsync(userId);

        var result = new List<ConversationDto>();

        foreach (var c in conversations)
        {
            var lastMsg = c.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault();

            int unreadCount = c.Messages
                .Count(m => m.ReceiverId == userId && !m.IsRead);

            result.Add(new ConversationDto
            {
                ConversationId = c.Id,
                ItemId = c.ItemId,
                ParticipantId = c.User1Id == userId ? c.User2Id : c.User1Id,
                LastMessage = lastMsg?.Content,
                LastTimestamp = lastMsg?.Timestamp,
                UnreadCount = unreadCount
            });
        }

        return Ok(result);
    }


    [HttpGet("unreadcount/{userId}")]
    public async Task<IActionResult> GetUnreadCount(string userId)
    {
        var count = await _unitOfWork.MessageRepository.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [HttpGet("getorcreate")]
    public async Task<IActionResult> GetOrCreateConversation([FromQuery] Guid itemId, [FromQuery] string user1Id, [FromQuery] string user2Id)
    {
        var convo = await _unitOfWork.ConversationRepository.GetOrCreateAsync(itemId, user1Id, user2Id);

        var lastMsg = convo.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault();

        return Ok(new ConversationDto
        {
            ConversationId = convo.Id,
            ItemId = convo.ItemId,
            ParticipantId = user1Id == convo.User1Id ? convo.User2Id : convo.User1Id,
            LastMessage = lastMsg?.Content,
            LastTimestamp = lastMsg?.Timestamp
        });
    }


}
