using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class ConversationRepository(ApiDbContext context) : IConversationRepository
{
    public async Task<Conversation?> GetOrCreateAsync(Guid itemId, string user1Id, string user2Id)
    {
        var convo = await context.Conversations
            .Include(c => c.Messages) // مهم عشان نجيب آخر رسالة بعد الإنشاء
            .FirstOrDefaultAsync(c =>
                c.ItemId == itemId &&
                ((c.User1Id == user1Id && c.User2Id == user2Id) ||
                 (c.User1Id == user2Id && c.User2Id == user1Id)));

        if (convo == null)
        {
            convo = new Conversation
            {
                ItemId = itemId,
                User1Id = user1Id,
                User2Id = user2Id
            };

            context.Conversations.Add(convo);
            await context.SaveChangesAsync();

            // نحتاج نرجعه من جديد مع Include للرسائل
            convo = await context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == convo.Id);
        }

        return convo;
    }


    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
    {
        return await context.Conversations
            .Include(c => c.Messages)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .ToListAsync();
    }
}