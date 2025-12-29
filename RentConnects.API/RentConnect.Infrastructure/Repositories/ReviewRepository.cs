using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class ReviewRepository(ApiDbContext context) : IReviewRepository
{
    public async Task<Review> GetByIdAsync(Guid id)
    {
        return await context.Reviews
            .Include(r => r.Item)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await context.Reviews
            .Include(r => r.Item)
            .Include(r => r.User)
            .ToListAsync();

    }

    public async Task AddAsync(Review entity)
    {
        await context.Reviews.AddAsync(entity);

    }

    public async Task UpdateAsync(Review entity)
    {
        context.Reviews.Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review != null)
        {
            context.Reviews.Remove(review);
        }
    }

    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
    {
        return await context.Reviews
            .Where(r => r.ItemId == productId)
            .Include(r => r.User)
            .ToListAsync();

    }

    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId)
    {
        return await context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Item)
            .ToListAsync();
    }
}