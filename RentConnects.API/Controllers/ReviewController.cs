using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;
using RentConnect.API.IService;

namespace RentConnect.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetAllReviewsWithDetails()
    {
        var reviews = await _unitOfWork.ReviewRepository.GetAllAsync();
        var reviewDetails = reviews.Select(r => new
        {
            ReviewId = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            User = new
            {
                r.UserId,
                r.User.UserName,
                r.User.Email,
                r.User.FullName
            },
            Item = new
            {
                r.ItemId,
                r.Item.Name,
                r.Item.Description,
                r.Item.PricePerDay
            }
        });

        return Ok(reviewDetails);
    }

    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetReviewsByProductId(Guid itemId)
    {
        var reviews = await _unitOfWork.ReviewRepository.GetReviewsByProductIdAsync(itemId);

        var dtoList = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UserFullName = r.User.FullName,
            UserId = r.UserId,
            ItemId = r.ItemId
        });

        return Ok(dtoList);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUserId(string userId)
    {
        var reviews = await _unitOfWork.ReviewRepository.GetReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] ReviewDto createReviewDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var item = await _unitOfWork.ItemRepository.GetByIdAsync(createReviewDto.ItemId);
        if (item == null)
            return NotFound($"Item with ID {createReviewDto.ItemId} not found.");

        var hasCompletedBooking = await _unitOfWork.BookingRepository.HasCompletedBookingAsync(
            createReviewDto.ItemId,
            createReviewDto.UserId
        );

        if (!hasCompletedBooking)
            return BadRequest("You can only review items you have booked and completed.");

        var review = new Review
        {
            UserId = createReviewDto.UserId,
            ItemId = createReviewDto.ItemId,
            Rating = createReviewDto.Rating,
            Comment = createReviewDto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ReviewRepository.AddAsync(review);
        await _unitOfWork.CompleteAsync();

        // ✅ NOTIFICATION: لصاحب المنتج عند وصول Review جديد
        var reviewer = await _userManager.FindByIdAsync(createReviewDto.UserId);
        var reviewerName = reviewer?.FullName ?? "Someone";

        await _notificationService.CreateAsync(
            userId: item.OwnerId,
            title: "New review ⭐",
            message: $"{reviewerName} left a {createReviewDto.Rating}/5 review on “{item.Name}”.",
            link: $"/items/{item.Id}"
        );

        var createdReview = await _unitOfWork.ReviewRepository.GetByIdAsync(review.Id);
        return Ok(createdReview);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewDto updateReviewDto)
    {
        if (id != updateReviewDto.Id)
            return BadRequest("Review ID mismatch.");

        var existingReview = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
        if (existingReview == null)
            return NotFound($"Review with ID {id} not found.");

        existingReview.Rating = updateReviewDto.Rating;
        existingReview.Comment = updateReviewDto.Comment;

        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
        if (review == null)
            return NotFound($"Review with ID {id} not found.");

        await _unitOfWork.ReviewRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}