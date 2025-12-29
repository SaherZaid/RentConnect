using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(Guid id);
    Task AddAsync(ItemDto dto);
    Task UpdateAsync(ItemDto dto);
    Task UpdateAsync(ItemUpdateDto dto);

    Task<HttpResponseMessage> UpdateItemWithImagesAsync(Guid id, MultipartFormDataContent formData);


    Task DeleteAsync(Guid id);
    Task<List<string>> GetCitiesAsync();


    Task<HttpResponseMessage> AddItemWithImagesAsync(MultipartFormDataContent formData); // ✨ لاحظ رجعنا HttpResponseMessage
}