using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IFavoriteRepository
{
    Task<IEnumerable<Guid>> GetItemIdsByUserIdAsync(string userId);
    Task AddAsync(Favorite favorite);
    Task RemoveAsync(string userId, Guid itemId);
    Task<bool> ExistsAsync(string userId, Guid itemId);
}