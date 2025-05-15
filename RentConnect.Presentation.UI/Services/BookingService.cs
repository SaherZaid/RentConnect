using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.IServices;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.Services;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;

    public BookingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>("api/bookings") ?? new List<BookingDto>();
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<BookingDto>($"api/bookings/{id}");
    }

    public async Task<IEnumerable<BookingDto>> GetByRenterIdAsync(string renterId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>($"api/bookings/renter/{renterId}") ?? new List<BookingDto>();
    }

    public async Task<IEnumerable<BookingDto>> GetByItemIdAsync(Guid itemId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>($"api/bookings/item/{itemId}") ?? new List<BookingDto>();
    }

    public async Task<IEnumerable<BookingDto>> GetByStatusAsync(BookingStatus status)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>($"api/bookings/status/{status}") ?? new List<BookingDto>();
    }

    public async Task<IEnumerable<BookingDto>> GetActiveBookingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>("api/bookings/active") ?? new List<BookingDto>();
    }

    public async Task<bool> IsItemAvailableAsync(Guid itemId, DateTime start, DateTime end, Guid? excludeId = null)
    {
        string url = $"api/bookings/check-availability?itemId={itemId}&startDate={start:s}&endDate={end:s}";
        if (excludeId.HasValue)
            url += $"&excludeBookingId={excludeId.Value}";

        return await _httpClient.GetFromJsonAsync<bool>(url);
    }

    public async Task<bool> HasCompletedBookingAsync(Guid itemId, string renterId)
    {
        string url = $"api/bookings/has-completed?itemId={itemId}&renterId={renterId}";
        return await _httpClient.GetFromJsonAsync<bool>(url);
    }

    public async Task AddAsync(BookingDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/bookings", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(BookingDto dto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/bookings/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/bookings/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ApproveAsync(Guid bookingId)
    {
        await _httpClient.PatchAsync($"api/bookings/approve/{bookingId}", null);
    }

    public async Task DeclineAsync(Guid bookingId)
    {
        await _httpClient.PatchAsync($"api/bookings/decline/{bookingId}", null);
    }

    public async Task<IEnumerable<BookingDto>> GetPendingForOwnerAsync(string ownerId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>($"api/bookings/pending-for-owner/{ownerId}")
               ?? new List<BookingDto>();
    }
}

