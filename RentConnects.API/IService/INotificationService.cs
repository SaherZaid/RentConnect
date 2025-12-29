namespace RentConnect.API.IService
{
    public interface INotificationService
    {
        Task CreateAsync(
            string userId,
            string title,
            string message,
            string? link = null
        );
    }
}
