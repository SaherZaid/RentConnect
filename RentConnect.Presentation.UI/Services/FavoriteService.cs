using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.Services;

public class FavoriteService : IFavoriteService
{
    private readonly HttpClient _http;

    public FavoriteService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Guid>> GetFavoritesForUserAsync(string userId)
    {
        var result = await _http.GetFromJsonAsync<List<Guid>>($"api/favorites/{userId}");
        return result ?? new();
    }

    public async Task<bool> AddToFavoritesAsync(string userId, Guid itemId)
    {
        var dto = new FavoriteDto { UserId = userId, ItemId = itemId };
        var response = await _http.PostAsJsonAsync("api/favorites", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFromFavoritesAsync(string userId, Guid itemId)
    {
        var dto = new FavoriteDto { UserId = userId, ItemId = itemId };
        var response = await _http.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/favorites")
        {
            Content = JsonContent.Create(dto)
        });
        return response.IsSuccessStatusCode;
    }

}