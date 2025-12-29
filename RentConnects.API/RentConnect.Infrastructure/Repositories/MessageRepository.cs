using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class MessageRepository(ApiDbContext context) : IMessageRepository
{
    public async Task<IEnumerable<Message>> GetMessagesByItemAsync(Guid itemId)
    {
        return await context.Messages
            .Where(m => m.ItemId == itemId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId)
    {
        return await context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await context.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .CountAsync();
    }

    public async Task MarkMessagesAsReadAsync(Guid conversationId, string userId)
    {
        var messages = await context.Messages
            .Where(m => m.ConversationId == conversationId && m.ReceiverId == userId && !m.IsRead)
            .ToListAsync();

        foreach (var msg in messages)
        {
            msg.IsRead = true;
        }

        await context.SaveChangesAsync();
    }

    public async Task AddMessageAsync(Message message)
    {
        await context.Messages.AddAsync(message);
    }

    public async Task<Message?> GetByIdAsync(Guid id)
    {
        return await context.Messages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public Task DeleteMessageAsync(Message message)
    {
        context.Messages.Remove(message);
        return Task.CompletedTask;
    }


}