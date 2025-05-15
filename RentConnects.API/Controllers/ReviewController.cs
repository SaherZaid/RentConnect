using Microsoft.AspNetCore.Mvc;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/review/details
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

    // GET: api/review/item/{itemId}
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

    // GET: api/review/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUserId(string userId)
    {
        var reviews = await _unitOfWork.ReviewRepository.GetReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    // POST: api/review
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] ReviewDto createReviewDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if the item exists
        var item = await _unitOfWork.ItemRepository.GetByIdAsync(createReviewDto.ItemId);
        if (item == null)
        {
            return NotFound($"Item with ID {createReviewDto.ItemId} not found.");
        }

        // Check if the user has a completed booking for the item
        var hasCompletedBooking = await _unitOfWork.BookingRepository.HasCompletedBookingAsync(
            createReviewDto.ItemId,
            createReviewDto.UserId
        );

        if (!hasCompletedBooking)
        {
            return BadRequest("You can only review items you have booked and completed.");
        }

        // Create a new review
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

        // Fetch and return the created review with related entities
        var createdReview = await _unitOfWork.ReviewRepository.GetByIdAsync(review.Id);
        return Ok(createdReview);
    }



    // PUT: api/review/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewDto updateReviewDto)
    {
        if (id != updateReviewDto.Id)
        {
            return BadRequest("Review ID mismatch.");
        }

        var existingReview = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
        if (existingReview == null)
        {
            return NotFound($"Review with ID {id} not found.");
        }

        existingReview.Rating = updateReviewDto.Rating;
        existingReview.Comment = updateReviewDto.Comment;
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    // DELETE: api/review/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound($"Review with ID {id} not found.");
        }

        await _unitOfWork.ReviewRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}



