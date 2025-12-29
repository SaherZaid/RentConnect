using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;
using RentConnect.API.IService;


namespace RentConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    // ✅ ADD:
    private readonly INotificationService _notificationService;

    public BookingsController(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService) // ✅ ADD
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userManager = userManager;
        _notificationService = notificationService; // ✅ ADD
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(Guid id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking == null) return NotFound();

        var dto = new BookingDto
        {
            Id = booking.Id,
            ItemId = booking.ItemId,
            RenterId = booking.RenterId,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Status = booking.Status
        };

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAll()
    {
        var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet("renter/{renterId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRenter(string renterId)
    {
        var bookings = await _unitOfWork.BookingRepository.GetByRenterIdAsync(renterId);
        return Ok(bookings);
    }

    [HttpGet("item/{itemId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByItem(Guid itemId)
    {
        var bookings = await _unitOfWork.BookingRepository.GetByItemIdAsync(itemId);
        return Ok(bookings);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetActive()
    {
        var bookings = await _unitOfWork.BookingRepository.GetActiveBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByStatus(BookingStatus status)
    {
        var bookings = await _unitOfWork.BookingRepository.GetByStatusAsync(status);
        return Ok(bookings);
    }

    [HttpGet("check-availability")]
    public async Task<ActionResult<bool>> IsItemAvailable(Guid itemId, DateTime startDate, DateTime endDate, Guid? excludeBookingId = null)
    {
        var available = await _unitOfWork.BookingRepository.IsItemAvailableAsync(itemId, startDate, endDate, excludeBookingId);
        return Ok(available);
    }

    [HttpGet("has-completed")]
    public async Task<ActionResult<bool>> HasCompleted(Guid itemId, string renterId)
    {
        var hasCompleted = await _unitOfWork.BookingRepository.HasCompletedBookingAsync(itemId, renterId);
        return Ok(hasCompleted);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isAvailable = await _unitOfWork.BookingRepository.IsItemAvailableAsync(dto.ItemId, dto.StartDate, dto.EndDate);
        if (!isAvailable)
            return BadRequest("This item is already booked during the selected period.");

        var item = await _unitOfWork.ItemRepository.GetByIdAsync(dto.ItemId);
        if (item == null)
            return NotFound("Item not found");

        var renter = await _userManager.FindByIdAsync(dto.RenterId);
        if (renter == null)
            return NotFound("Renter not found");

        var owner = await _userManager.FindByIdAsync(item.OwnerId);
        if (owner == null)
            return NotFound("Owner not found");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ItemId = dto.ItemId,
            RenterId = dto.RenterId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = BookingStatus.Pending
        };

        await _unitOfWork.BookingRepository.AddAsync(booking);
        await _unitOfWork.CompleteAsync();

        // ✅ NOTIFICATION: للمالك (Owner) عند وصول طلب حجز جديد
        await _notificationService.CreateAsync(
            userId: item.OwnerId,
            title: "New booking request",
            message: $"{renter.FullName} requested to book “{item.Name}” ({dto.StartDate:yyyy-MM-dd} → {dto.EndDate:yyyy-MM-dd})",
            link: "/my-bookings/owner"
        );

        // ✅ NOTIFICATION: للمستأجر (Renter) تأكيد استلام الطلب
        await _notificationService.CreateAsync(
            userId: renter.Id,
            title: "Booking request sent",
            message: $"Your request for “{item.Name}” was sent to the owner.",
            link: "/bookings"
        );

        // Send email to renter
        var renterEmailBody = $"""
            Hello {renter.FullName},<br/><br/>
            Your booking request for <strong>{item.Name}</strong> has been received.<br/>
            <strong>Dates:</strong> {dto.StartDate:yyyy-MM-dd} to {dto.EndDate:yyyy-MM-dd}<br/>
            We will notify you once the owner accepts or declines your booking.<br/><br/>
            Thanks,<br/>RentConnect Team
        """;

        await _emailService.SendEmailAsync(renter.Email, "Booking Request Received", renterEmailBody);

        // Send email to item owner
        var ownerEmailBody = $"""
            Hello {owner.FullName},<br/><br/>
            You have received a new booking request for your item: <strong>{item.Name}</strong>.<br/>
            <strong>Renter:</strong> {renter.FullName} ({renter.Email})<br/>
            <strong>Dates:</strong> {dto.StartDate:yyyy-MM-dd} to {dto.EndDate:yyyy-MM-dd}<br/><br/>
            Please log in to confirm or decline the booking:<br/>
            <a href="https://localhost:7166/my-bookings/owner">Manage Bookings</a><br/><br/>
            RentConnect Team
        """;

        await _emailService.SendEmailAsync(owner.Email, "New Booking Request", ownerEmailBody);

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BookingDto dto)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking == null) return NotFound();

        booking.StartDate = dto.StartDate;
        booking.EndDate = dto.EndDate;
        booking.Status = dto.Status;

        await _unitOfWork.BookingRepository.UpdateAsync(booking);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _unitOfWork.BookingRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpPatch("approve/{id}")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking == null) return NotFound();

        var item = await _unitOfWork.ItemRepository.GetByIdAsync(booking.ItemId);
        var renter = await _userManager.FindByIdAsync(booking.RenterId);

        if (item == null || renter == null)
            return BadRequest("Invalid booking data.");

        booking.Status = BookingStatus.Confirmed;
        await _unitOfWork.CompleteAsync();

        // ✅ NOTIFICATION: للمستأجر (تم القبول)
        await _notificationService.CreateAsync(
            userId: renter.Id,
            title: "Booking approved ✅",
            message: $"Your booking for “{item.Name}” was approved ({booking.StartDate:yyyy-MM-dd} → {booking.EndDate:yyyy-MM-dd}).",
            link: "/bookings"
        );

        // Send confirmation email to renter
        var body = $"""
            Hello {renter.FullName},<br/><br/>
            Good news! Your booking for <strong>{item.Name}</strong> from {booking.StartDate:yyyy-MM-dd} to {booking.EndDate:yyyy-MM-dd} has been <strong>approved</strong> by the owner.<br/><br/>
            Please get in touch with the owner if needed.<br/><br/>
            Thanks,<br/>RentConnect Team
        """;

        await _emailService.SendEmailAsync(renter.Email, "Your Booking is Approved", body);

        return NoContent();
    }

    [HttpPatch("decline/{id}")]
    public async Task<IActionResult> Decline(Guid id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking == null) return NotFound();

        var item = await _unitOfWork.ItemRepository.GetByIdAsync(booking.ItemId);
        var renter = await _userManager.FindByIdAsync(booking.RenterId);

        if (item == null || renter == null)
            return BadRequest("Invalid booking data.");

        booking.Status = BookingStatus.Declined;
        await _unitOfWork.CompleteAsync();

        // ✅ NOTIFICATION: للمستأجر (تم الرفض)
        await _notificationService.CreateAsync(
            userId: renter.Id,
            title: "Booking declined ❌",
            message: $"Your booking for “{item.Name}” was declined.",
            link: "/bookings"
        );

        // Send rejection email to renter
        var body = $"""
            Hello {renter.FullName},<br/><br/>
            Unfortunately, your booking for <strong>{item.Name}</strong> from {booking.StartDate:yyyy-MM-dd} to {booking.EndDate:yyyy-MM-dd} has been <strong>declined</strong> by the owner.<br/><br/>
            You can try booking another item or contact the owner for more information.<br/><br/>
            Thanks,<br/>RentConnect Team
        """;

        await _emailService.SendEmailAsync(renter.Email, "Your Booking was Declined", body);

        return NoContent();
    }

    [HttpGet("pending-for-owner/{ownerId}")]
    public async Task<IActionResult> GetPendingBookingsForOwner(string ownerId)
    {
        var bookings = await _unitOfWork.BookingRepository.GetPendingBookingsForOwnerAsync(ownerId);
        return Ok(bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            ItemId = b.ItemId,
            ItemName = b.Item?.Name,
            OwnerId = b.Item?.OwnerId ?? "",
            RenterId = b.RenterId,
            RenterName = b.Renter?.FullName,
            RenterEmail = b.Renter?.Email,
            RenterPhone = b.Renter?.PhoneNumber,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.Status
        }));
    }
}