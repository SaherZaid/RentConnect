using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Presentation.DTOs
{
    public class ReportCreateDto
    {
        public string ReporterUserId { get; set; } = default!;
        public ReportTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public string Reason { get; set; } = default!;
        public string? Details { get; set; }
    }
}
