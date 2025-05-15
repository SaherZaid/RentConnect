using RentConnect.API.RentConnect.Application.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface ICategoryItemRepository : IRepository<CategoryItem, Guid>
{
    Task<IEnumerable<CategoryItem>> GetByCategoryIdAsync(Guid categoryId);



}