using RentConnect.API.Controllers;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.Presentation.UI.IServices;

public interface IReportAdminService
{
    Task<PagedResult<ReportAdminDto>> GetPagedAsync(
        ReportStatus? status,
        ReportTargetType? targetType,
        string? search,
        int page,
        int pageSize);

    Task<bool> UpdateStatusAsync(Guid id, ReportStatus status);

    Task<int> GetPendingCountAsync();

}