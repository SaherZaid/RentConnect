using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class ItemImageRepository(ApiDbContext context) : IItemImageRepository
{
    public async Task<ItemImage> GetByIdAsync(Guid id)
    {
        return await context.ItemImages.FindAsync(id);
    }

    public async Task<IEnumerable<ItemImage>> GetAllAsync()
    {
        return await context.ItemImages.ToListAsync();
    }

    public async Task<IEnumerable<ItemImage>> GetByItemIdAsync(Guid itemId)
    {
        return await context.ItemImages.Where(x => x.ItemId == itemId).ToListAsync();
    }

    public async Task AddAsync(ItemImage entity)
    {
        await context.ItemImages.AddAsync(entity);
    }

    public async Task UpdateAsync(ItemImage entity)
    {
        context.ItemImages.Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var image = await GetByIdAsync(id);
        if (image != null)
        {
            context.ItemImages.Remove(image);
        }
    }
}