using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetByRenterIdAsync(string renterId);
    Task<IEnumerable<BookingDto>> GetByItemIdAsync(Guid itemId);
    Task<IEnumerable<BookingDto>> GetByStatusAsync(BookingStatus status);
    Task<IEnumerable<BookingDto>> GetActiveBookingsAsync();
    Task<bool> IsItemAvailableAsync(Guid itemId, DateTime startDate, DateTime endDate, Guid? excludeId = null);
    Task<bool> HasCompletedBookingAsync(Guid itemId, string renterId);

    Task AddAsync(BookingDto dto);
    Task UpdateAsync(BookingDto dto);
    Task DeleteAsync(Guid id);
    Task ApproveAsync(Guid bookingId);
    Task DeclineAsync(Guid bookingId);
    Task<IEnumerable<BookingDto>> GetPendingForOwnerAsync(string ownerId);


}