using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllAsync();
    Task<ReviewDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ReviewDto>> GetByItemIdAsync(Guid itemId);
    Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId);
    Task AddAsync(ReviewDto dto);
    Task UpdateAsync(ReviewDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> UserHasCompletedBookingAsync(Guid itemId, string userId);
    Task<IEnumerable<ReviewDto>> GetReviewsByItemIdAsync(Guid itemId);
    Task AddReviewAsync(ReviewDto review);

    Task<List<ReviewDetailsDto>> GetAllWithDetailsAsync();

}