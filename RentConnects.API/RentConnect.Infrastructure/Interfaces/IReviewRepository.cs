using RentConnect.API.RentConnect.Application.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IReviewRepository : IRepository<Review, Guid>
{
    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId);
    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId);
}