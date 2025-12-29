using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class ConversationRepository(ApiDbContext context) : IConversationRepository
{
    // ✅ هنا سوينا المحادثة على مستوى المستخدمين فقط (مو مرتبطه بالـ ItemId في البحث)
    public async Task<Conversation?> GetOrCreateAsync(Guid itemId, string user1Id, string user2Id)
    {
        // ندور على أي محادثة بين نفس الشخصين، بغض النظر عن ترتيبهم، ومن غير ما نقيّد بالـ ItemId
        var convo = await context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                (c.User1Id == user1Id && c.User2Id == user2Id) ||
                (c.User1Id == user2Id && c.User2Id == user1Id));

        // لو ما فيه، ننشئ محادثة جديدة
        if (convo == null)
        {
            convo = new Conversation
            {
                ItemId = itemId,   // ممكن تخلي أول Item بدأ فيه الشات
                User1Id = user1Id,
                User2Id = user2Id
            };

            context.Conversations.Add(convo);
            await context.SaveChangesAsync();

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

    public async Task DeleteAsync(Guid conversationId)
    {
        var convo = await context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (convo == null) return;

        context.Messages.RemoveRange(convo.Messages);
        context.Conversations.Remove(convo);
    }
}