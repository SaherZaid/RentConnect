using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.IServices
{
    public interface IReportService
    {
        Task<bool> CreateAsync(ReportCreateDto dto);
    }
}
