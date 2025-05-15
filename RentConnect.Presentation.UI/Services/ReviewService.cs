using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.Services;

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;

    public ReviewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ReviewDto>>("api/review")
               ?? new List<ReviewDto>();
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ReviewDto>($"api/review/{id}");
    }

    public async Task<IEnumerable<ReviewDto>> GetByItemIdAsync(Guid itemId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ReviewDto>>($"api/review/item/{itemId}")
               ?? new List<ReviewDto>();
    }

    public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ReviewDto>>($"api/review/user/{userId}")
               ?? new List<ReviewDto>();
    }



    public async Task AddAsync(ReviewDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/review", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(ReviewDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/review/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/review/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> UserHasCompletedBookingAsync(Guid itemId, string userId)
    {
        return await _httpClient.GetFromJsonAsync<bool>($"api/booking/has-completed-booking?itemId={itemId}&userId={userId}");
    }
    public async Task<IEnumerable<ReviewDto>> GetReviewsByItemIdAsync(Guid itemId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ReviewDto>>($"api/review/item/{itemId}") ?? new List<ReviewDto>();
    }

    public async Task AddReviewAsync(ReviewDto review)
    {
        var response = await _httpClient.PostAsJsonAsync("api/review", review);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ReviewDetailsDto>> GetAllWithDetailsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<ReviewDetailsDto>>("api/review/details");
        return response ?? new List<ReviewDetailsDto>();
    }

}




