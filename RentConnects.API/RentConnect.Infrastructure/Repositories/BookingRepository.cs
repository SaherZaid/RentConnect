using Microsoft.EntityFrameworkCore;
using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;


public class BookingRepository(ApiDbContext context) : IBookingRepository
{
    private BookingDto ToDto(Booking b)
    {
        return new BookingDto
        {
            Id = b.Id,
            ItemId = b.ItemId,
            ItemName = b.Item?.Name,
            RenterId = b.RenterId,
            RenterName = b.Renter?.FullName,
            RenterEmail = b.Renter?.Email,
            RenterPhone = b.Renter?.PhoneNumber,
            OwnerName = b.Item?.Owner?.FullName,
            OwnerEmail = b.Item?.Owner?.Email,
            OwnerPhone = b.Item?.Owner?.PhoneNumber,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.Status
        };
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await context.Bookings
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // يرجع Booking كيان
    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await context.Bookings
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
    }

    public async Task UpdateAsync(Booking booking)
    {
        var existingBooking = await context.Bookings.FindAsync(booking.Id);
        if (existingBooking == null) return;

        if (booking.StartDate != default && booking.StartDate != existingBooking.StartDate)
            existingBooking.StartDate = booking.StartDate;

        if (booking.EndDate != default && booking.EndDate != existingBooking.EndDate)
            existingBooking.EndDate = booking.EndDate;

        if (booking.Status != existingBooking.Status)
            existingBooking.Status = booking.Status;

        if (booking.ItemId != Guid.Empty && booking.ItemId != existingBooking.ItemId)
            existingBooking.ItemId = booking.ItemId;

        if (!string.IsNullOrEmpty(booking.RenterId) && booking.RenterId != existingBooking.RenterId)
            existingBooking.RenterId = booking.RenterId;
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await context.Bookings.FindAsync(id);
        if (booking is not null)
        {
            context.Bookings.Remove(booking);
        }
    }

    public async Task<bool> HasCompletedBookingAsync(Guid itemId, string renterId)
    {
        return await context.Bookings.AnyAsync(b =>
            b.ItemId == itemId &&
            b.RenterId == renterId &&
            b.Status == BookingStatus.Confirmed);
    }

    public async Task<bool> IsItemAvailableAsync(Guid itemId, DateTime startDate, DateTime endDate, Guid? excludeBookingId = null)
    {
        var overlapping = await context.Bookings
            .Where(b => b.ItemId == itemId && b.Status == BookingStatus.Confirmed && b.Id != excludeBookingId)
            .Where(b => b.StartDate < endDate && b.EndDate > startDate)
            .AnyAsync();

        return !overlapping;
    }

    public async Task<IEnumerable<BookingDto>> GetByRenterIdAsync(string renterId)
    {
        var bookings = await context.Bookings
            .Where(b => b.RenterId == renterId)
            .Include(b => b.Item).ThenInclude(i => i.Owner)
            .Include(b => b.Renter)
            .ToListAsync();

        return bookings.Select(ToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByItemIdAsync(Guid itemId)
    {
        var bookings = await context.Bookings
            .Where(b => b.ItemId == itemId)
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .ToListAsync();

        return bookings.Select(ToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetActiveBookingsAsync()
    {
        var now = DateTime.UtcNow;
        var bookings = await context.Bookings
            .Where(b => b.StartDate <= now && b.EndDate >= now)
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .ToListAsync();

        return bookings.Select(ToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByStatusAsync(BookingStatus status)
    {
        var bookings = await context.Bookings
            .Where(b => b.Status == status)
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .ToListAsync();

        return bookings.Select(ToDto);
    }

    public async Task ApproveBookingAsync(Guid bookingId)
    {
        var booking = await context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = BookingStatus.Confirmed;
            context.Bookings.Update(booking);
        }
    }

    public async Task DeclineBookingAsync(Guid bookingId)
    {
        var booking = await context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = BookingStatus.Declined;
            context.Bookings.Update(booking);
        }
    }

    public async Task<IEnumerable<Booking>> GetPendingBookingsForOwnerAsync(string ownerId)
    {
        return await context.Bookings
            .Include(b => b.Item)
            .Include(b => b.Renter)
            .Where(b => b.Status == BookingStatus.Pending && b.Item.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Booking?> GetFullBookingByIdAsync(Guid id)
    {
        return await context.Bookings
            .Include(b => b.Item).ThenInclude(i => i.Owner)
            .Include(b => b.Renter)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

}






