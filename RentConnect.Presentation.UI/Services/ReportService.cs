using RentConnect.API.RentConnect.Presentation.DTOs;
using RentConnect.Presentation.UI.IServices;

namespace RentConnect.Presentation.UI.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _http;

        public ReportService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CreateAsync(ReportCreateDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/Reports", dto);
            return resp.IsSuccessStatusCode;
        }
    }
}
