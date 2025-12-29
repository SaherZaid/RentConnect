using Microsoft.AspNetCore.Mvc;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoritesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Guid>>> GetUserFavorites(string userId)
    {
        var itemIds = await _unitOfWork.FavoriteRepository.GetItemIdsByUserIdAsync(userId);
        return Ok(itemIds);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FavoriteDto dto)
    {
        var exists = await _unitOfWork.FavoriteRepository.ExistsAsync(dto.UserId!, dto.ItemId);
        if (!exists)
        {
            var fav = new Favorite
            {
                UserId = dto.UserId,
                ItemId = dto.ItemId
            };
            await _unitOfWork.FavoriteRepository.AddAsync(fav);
            await _unitOfWork.CompleteAsync();
        }
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromBody] FavoriteDto dto)
    {
        await _unitOfWork.FavoriteRepository.RemoveAsync(dto.UserId!, dto.ItemId);
        await _unitOfWork.CompleteAsync();
        return Ok();
    }
}