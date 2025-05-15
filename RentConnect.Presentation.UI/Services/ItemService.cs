using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace RentConnect.Presentation.UI.Services;

public class ItemService : IItemService
{
    private readonly HttpClient _httpClient;

    public ItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ItemDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ItemDto>>("api/Item") ?? new List<ItemDto>();
    }

    public async Task<ItemDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ItemDto>($"api/Item/{id}");
    }

    public async Task AddAsync(ItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Item", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(ItemDto dto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/Item/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Item/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<HttpResponseMessage> AddItemWithImagesAsync(MultipartFormDataContent formData)
    {
        var response = await _httpClient.PostAsync("api/Item/create-with-images", formData);
        return response;
    }
}