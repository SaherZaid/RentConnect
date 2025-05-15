using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(Guid id);
    Task AddAsync(ItemDto dto);
    Task UpdateAsync(ItemDto dto);
    Task DeleteAsync(Guid id);

    Task<HttpResponseMessage> AddItemWithImagesAsync(MultipartFormDataContent formData); // ✨ لاحظ رجعنا HttpResponseMessage
}