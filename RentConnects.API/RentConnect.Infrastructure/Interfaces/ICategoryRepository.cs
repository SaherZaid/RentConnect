using RentConnect.API.RentConnect.Application.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<IEnumerable<object>> GetCategoriesWithItemsAsync();
}