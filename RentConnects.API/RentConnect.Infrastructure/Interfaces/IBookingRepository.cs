using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Application.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces;

public interface IBookingRepository : IRepository<Booking, Guid>
{
    Task<bool> IsItemAvailableAsync(Guid itemId, DateTime startDate, DateTime endDate, Guid? excludeBookingId = null);
    Task<bool> HasCompletedBookingAsync(Guid itemId, string renterId);
    Task<IEnumerable<BookingDto>> GetByRenterIdAsync(string renterId);
    Task<IEnumerable<BookingDto>> GetByItemIdAsync(Guid itemId);
    Task<IEnumerable<BookingDto>> GetActiveBookingsAsync();
    Task<IEnumerable<BookingDto>> GetByStatusAsync(BookingStatus status);
    Task ApproveBookingAsync(Guid bookingId);
    Task DeclineBookingAsync(Guid bookingId);
    Task<IEnumerable<Booking>> GetPendingBookingsForOwnerAsync(string ownerId);
    Task<Booking?> GetFullBookingByIdAsync(Guid id);

}