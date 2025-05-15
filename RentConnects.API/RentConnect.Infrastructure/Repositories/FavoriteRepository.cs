using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class FavoriteRepository(ApiDbContext context) : IFavoriteRepository
{

    public async Task<IEnumerable<Guid>> GetItemIdsByUserIdAsync(string userId)
    {
        return await context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ItemId)
            .ToListAsync();
    }

    public async Task AddAsync(Favorite favorite)
    {
        await context.Favorites.AddAsync(favorite);
    }

    public async Task RemoveAsync(string userId, Guid itemId)
    {
        var fav = await context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId);
        if (fav != null)
        {
            context.Favorites.Remove(fav);
        }
    }

    public async Task<bool> ExistsAsync(string userId, Guid itemId)
    {
        return await context.Favorites.AnyAsync(f => f.UserId == userId && f.ItemId == itemId);
    }
}