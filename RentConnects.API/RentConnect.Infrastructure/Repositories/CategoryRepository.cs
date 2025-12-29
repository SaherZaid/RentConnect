using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories;

public class CategoryRepository(ApiDbContext context) : ICategoryRepository
{
    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await context.Categories.ToListAsync();
    }

    public async Task AddAsync(Category entity)
    {
        await context.Categories.AddAsync(entity);
    }

    public async Task UpdateAsync(Category entity)
    {
        context.Categories.Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
        }

    }

    public async Task<IEnumerable<object>> GetCategoriesWithItemsAsync()
    {
        return await context.Categories
            .Select(category => new CategoryWithItemsDto
            {
                Id = category.Id,
                Name = category.Name,
                Items = context.CategoryItems
                    .Where(cp => cp.CategoryId == category.Id)
                    .Select(cp => new ItemDto
                    {
                        Id = cp.Item.Id,
                        Name = cp.Item.Name,
                        PricePerDay = cp.Item.PricePerDay,
                        Description = cp.Item.Description,
                        OwnerId = cp.Item.OwnerId,

                    })
                    .ToList()
            })
            .ToListAsync();
    }

}