using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using RentConnect.API.RentConnect.Presentation.DTOs;

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
        var resp = await _httpClient.GetAsync($"api/Item/{id}");

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ItemDto>();
    }

    public async Task<List<string>> GetCitiesAsync()
    {
        var data = await _httpClient.GetFromJsonAsync<List<string>>("api/Item/cities");
        return data ?? new List<string>();
    }



    public async Task AddAsync(ItemDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Item", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(ItemDto dto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/Item/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Update failed: {(int)response.StatusCode} - {body}");
        }
    }

    public async Task UpdateAsync(ItemUpdateDto dto)
    {
        var resp = await _httpClient.PatchAsJsonAsync($"api/Item/{dto.Id}", dto);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Update failed: {(int)resp.StatusCode} - {body}");
        }
    }

    public async Task<HttpResponseMessage> UpdateItemWithImagesAsync(Guid id, MultipartFormDataContent formData)
    {
        return await _httpClient.PutAsync($"api/Item/{id}/update-with-images", formData);
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