namespace RentConnect.API.RentConnect.Domain.Models;


    public enum ReportTargetType
    {
        Item = 1,
        Review = 2
    }

    public enum ReportStatus
    {
        Pending = 1,
        Resolved = 2,
        Rejected = 3
    }

    public class Report
    {
        public Guid Id { get; set; }

        // من هو اللي بلغ
        public string ReporterUserId { get; set; } = default!;
        public ApplicationUser? ReporterUser { get; set; }

        // على ايش البلاغ
        public ReportTargetType TargetType { get; set; }
        public Guid TargetId { get; set; } // ItemId or ReviewId

        // سبب البلاغ
        public string Reason { get; set; } = default!;   // "Spam" , "Scam" , "Inappropriate" ...
        public string? Details { get; set; }             // نص إضافي اختياري

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
