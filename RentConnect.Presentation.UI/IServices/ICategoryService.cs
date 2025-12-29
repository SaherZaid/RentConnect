using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<IEnumerable<CategoryWithItemsDto>> GetCategoriesWithItemsAsync();
    Task AddCategoryAsync(CategoryDto categorydto);
    Task UpdateCategoryAsync(CategoryDto categorydto);
    Task DeleteCategoryAsync(Guid id);



}