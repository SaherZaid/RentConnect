using RentConnect.API.RentConnect.Application.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IItemImageRepository : IRepository<ItemImage, Guid>
{
    Task<IEnumerable<ItemImage>> GetByItemIdAsync(Guid itemId);
}