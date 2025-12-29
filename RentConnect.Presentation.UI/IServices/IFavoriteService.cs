namespace RentConnect.Presentation.UI.IServices;

public interface IFavoriteService
{
    Task<List<Guid>> GetFavoritesForUserAsync(string userId);
    Task<bool> AddToFavoritesAsync(string userId, Guid itemId);
    Task<bool> RemoveFromFavoritesAsync(string userId, Guid itemId);
}