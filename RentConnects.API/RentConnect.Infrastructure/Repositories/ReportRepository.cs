using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.DataAccess;
using RentConnect.API.RentConnect.Infrastructure.Interfaces;

namespace RentConnect.API.RentConnect.Infrastructure.Repositories
{
    public class ReportRepository(ApiDbContext context) : IReportRepository
    {
        public async Task AddAsync(Report report)
        {
            await context.Reports.AddAsync(report);
        }

        public async Task<bool> ExistsAsync(string reporterUserId, ReportTargetType type, Guid targetId)
        {
            return await context.Reports.AnyAsync(r =>
                r.ReporterUserId == reporterUserId &&
                r.TargetType == type &&
                r.TargetId == targetId &&
                r.Status == ReportStatus.Pending);
        }

        // ✅ Admin - paging + filters
        public async Task<(List<Report> Items, int Total)> GetPagedAsync(
            ReportStatus? status,
            ReportTargetType? targetType,
            string? search,
            int page,
            int pageSize)
        {
            var q = context.Reports
                .AsNoTracking()
                .Include(r => r.ReporterUser)
                .AsQueryable();

            if (status.HasValue)
                q = q.Where(r => r.Status == status.Value);

            if (targetType.HasValue)
                q = q.Where(r => r.TargetType == targetType.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                q = q.Where(r =>
                    r.Reason.Contains(search) ||
                    (r.Details != null && r.Details.Contains(search)) ||
                    r.ReporterUserId.Contains(search) ||
                    (r.ReporterUser != null && (
                        (r.ReporterUser.FullName != null && r.ReporterUser.FullName.Contains(search)) ||
                        (r.ReporterUser.Email != null && r.ReporterUser.Email.Contains(search)) ||
                        (r.ReporterUser.UserName != null && r.ReporterUser.UserName.Contains(search))
                    ))
                );
            }

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(r => r.Status)                // Pending أول
                .ThenByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await context.Reports
                .AsNoTracking()
                .Include(r => r.ReporterUser)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateStatusAsync(Guid id, ReportStatus status)
        {
            var report = await context.Reports.FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return;

            report.Status = status;
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await context.Reports.CountAsync(r => r.Status == ReportStatus.Pending);
        }

    }
}