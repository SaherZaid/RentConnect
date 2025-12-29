using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.UnitOfwork;

public interface IUnitOfWork : IDisposable
{
    IItemRepository ItemRepository { get; }
    IReviewRepository ReviewRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ICategoryItemRepository CategoryItemRepository { get; }
    IBookingRepository BookingRepository { get; }
    IItemImageRepository ItemImageRepository { get; }
    IFavoriteRepository FavoriteRepository { get; }
    IMessageRepository MessageRepository { get; }
    IConversationRepository ConversationRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IReportRepository ReportRepository { get; }



    Task CompleteAsync();
}