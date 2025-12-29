using System.Net.Http.Json;
using RentConnect.API.Controllers;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.Presentation.UI.IServices;

namespace RentConnect.Presentation.UI.Services
{
    public class ReportAdminService : IReportAdminService
    {
        private readonly HttpClient _http;

        public ReportAdminService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PagedResult<ReportAdminDto>> GetPagedAsync(
            ReportStatus? status,
            ReportTargetType? targetType,
            string? search,
            int page,
            int pageSize)
        {
            var url = $"api/Reports/admin?page={page}&pageSize={pageSize}";

            if (status.HasValue) url += $"&status={status.Value}";
            if (targetType.HasValue) url += $"&targetType={targetType.Value}";
            if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";

            var data = await _http.GetFromJsonAsync<PagedResult<ReportAdminDto>>(url);
            return data ?? new PagedResult<ReportAdminDto> { Items = new(), Total = 0, Page = page, PageSize = pageSize };
        }

        public async Task<bool> UpdateStatusAsync(Guid id, ReportStatus status)
        {
            var url = $"api/Reports/admin/{id}/status?status={status}";
            var resp = await _http.PatchAsync(url, null);
            return resp.IsSuccessStatusCode;
        }

        public async Task<int> GetPendingCountAsync()
        {
            var count = await _http.GetFromJsonAsync<int>("api/Reports/admin/pending-count");
            return count;
        }

    }
}