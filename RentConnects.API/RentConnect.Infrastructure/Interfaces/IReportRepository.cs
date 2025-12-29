using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.Interfaces
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<bool> ExistsAsync(string reporterUserId, ReportTargetType type, Guid targetId);

        // ✅ Admin
        Task<(List<Report> Items, int Total)> GetPagedAsync(
            ReportStatus? status,
            ReportTargetType? targetType,
            string? search,
            int page,
            int pageSize);

        Task<Report?> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid id, ReportStatus status);

        Task<int> GetPendingCountAsync();

    }
}
