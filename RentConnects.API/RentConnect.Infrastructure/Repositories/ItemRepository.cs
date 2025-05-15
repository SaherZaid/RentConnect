using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class ItemRepository(ApiDbContext context) : IItemRepository
{
    public async Task<Item> GetByIdAsync(Guid id)
    {
        return await context.Items
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await context.Items
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .ToListAsync();
    }

    public async Task AddAsync(Item entity)
    {
        await context.Items.AddAsync(entity);
    }

    public async Task UpdateAsync(Item updatedProduct)
    {
        var existingItem = await context.Items.FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);
        if (existingItem is null)
        {
            return;
        }

        existingItem.Name = updatedProduct.Name;
        existingItem.Description = updatedProduct.Description;


        context.Items.Update(existingItem);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item is not null)
        {
            context.Items.Remove(item);
        }
    }

}