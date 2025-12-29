using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class CategoryItemRepository(ApiDbContext context) : ICategoryItemRepository
{
    public async Task<CategoryItem> GetByIdAsync(Guid id)
    {
        return await context.CategoryItems
            .Include(cp => cp.Category)
            .Include(cp => cp.Item)
            .FirstOrDefaultAsync(cp => cp.Id == id);

    }

    public async Task<IEnumerable<CategoryItem>> GetAllAsync()
    {
        return await context.CategoryItems
            .Include(cp => cp.Category)
            .Include(cp => cp.Item)
            .ToListAsync();


    }

    public async Task AddAsync(CategoryItem entity)
    {
        await context.CategoryItems.AddAsync(entity);

    }

    public async Task UpdateAsync(CategoryItem entity)
    {
        context.CategoryItems.Update(entity);

    }

    public async Task DeleteAsync(Guid id)
    {
        var categoryProduct = await GetByIdAsync(id);
        if (categoryProduct != null)
        {
            context.CategoryItems.Remove(categoryProduct);

        }
    }

    public async Task<IEnumerable<CategoryItem>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await context.CategoryItems
            .Where(cp => cp.CategoryId == categoryId)
            .Include(cp => cp.Item)
            .ToListAsync();

    }


}