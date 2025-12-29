using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;
using RentConnect.API.RentConnect.Infrastructure.Repositories;

namespace RentConnect.API.RentConnect.Infrastructure.UnitOfwork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApiDbContext _apiContext;


    public IItemRepository ItemRepository { get; }
    public IReviewRepository ReviewRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public ICategoryItemRepository CategoryItemRepository { get; }
    public IBookingRepository BookingRepository { get; }
    public IItemImageRepository ItemImageRepository { get; }
    public IFavoriteRepository FavoriteRepository { get; }
    public IMessageRepository MessageRepository { get; }
    public IConversationRepository ConversationRepository { get; }
    public INotificationRepository NotificationRepository { get; }
    public IReportRepository ReportRepository { get; }





    public UnitOfWork(ApiDbContext apiContext)
    {
        _apiContext = apiContext;

        ItemRepository = new ItemRepository(apiContext);
        ReviewRepository = new ReviewRepository(apiContext);
        CategoryRepository = new CategoryRepository(apiContext);
        CategoryItemRepository = new CategoryItemRepository(apiContext);
        BookingRepository = new BookingRepository(apiContext);
        ItemImageRepository = new ItemImageRepository(apiContext);
        FavoriteRepository = new FavoriteRepository(apiContext);
        MessageRepository = new MessageRepository(apiContext);
        ConversationRepository = new ConversationRepository(apiContext);
        NotificationRepository = new NotificationRepository(apiContext);
        ReportRepository = new ReportRepository(apiContext);
    }

    public async Task CompleteAsync()
    {
        await _apiContext.SaveChangesAsync();
    }

    public async void Dispose()
    {
        await _apiContext.DisposeAsync();
    }


}