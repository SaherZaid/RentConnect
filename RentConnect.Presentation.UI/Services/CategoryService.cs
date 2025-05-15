using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.Services;
public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/category");
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CategoryDto>($"api/category/{id}");
    }

    public async Task<IEnumerable<CategoryWithItemsDto>> GetCategoriesWithItemsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryWithItemsDto>>("api/category/with-products");
    }



    public async Task AddCategoryAsync(CategoryDto categorydto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/category", categorydto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCategoryAsync(CategoryDto categorydto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/category/{categorydto.Id}", categorydto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/category/{id}");
        response.EnsureSuccessStatusCode();
    }




}