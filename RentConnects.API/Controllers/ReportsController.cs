using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Infrastructure.UnitOfwork;
using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        private const int MaxReportsPerWindow = 10;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan SameTargetCooldown = TimeSpan.FromMinutes(2);

        public ReportsController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportCreateDto dto)
        {
            if (dto == null) return BadRequest("Invalid payload.");
            if (string.IsNullOrWhiteSpace(dto.ReporterUserId)) return BadRequest("ReporterUserId is required.");
            if (dto.TargetId == Guid.Empty) return BadRequest("TargetId is required.");
            if (string.IsNullOrWhiteSpace(dto.Reason)) return BadRequest("Reason is required.");

            var rlKey = $"reports-rl:{dto.ReporterUserId}";
            var count = _cache.Get<int?>(rlKey) ?? 0;

            if (count >= MaxReportsPerWindow)
                return StatusCode(429, $"Too many reports. Try again later.");

            var targetKey = $"reports-target:{dto.ReporterUserId}:{(int)dto.TargetType}:{dto.TargetId}";
            if (_cache.TryGetValue(targetKey, out _))
                return StatusCode(429, "Please wait a bit before reporting the same thing again.");

            var exists = await _unitOfWork.ReportRepository.ExistsAsync(dto.ReporterUserId, dto.TargetType, dto.TargetId);
            if (exists)
                return BadRequest("You already reported this. (Pending)");

            if (dto.TargetType == ReportTargetType.Item)
            {
                var item = await _unitOfWork.ItemRepository.GetByIdAsync(dto.TargetId);
                if (item == null) return NotFound("Item not found.");
            }
            else if (dto.TargetType == ReportTargetType.Review)
            {
                var review = await _unitOfWork.ReviewRepository.GetByIdAsync(dto.TargetId);
                if (review == null) return NotFound("Review not found.");
            }
            else
            {
                return BadRequest("Invalid TargetType.");
            }

            var report = new Report
            {
                Id = Guid.NewGuid(),
                ReporterUserId = dto.ReporterUserId,
                TargetType = dto.TargetType,
                TargetId = dto.TargetId,
                Reason = dto.Reason.Trim(),
                Details = string.IsNullOrWhiteSpace(dto.Details) ? null : dto.Details.Trim(),
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            await _unitOfWork.CompleteAsync();

            _cache.Set(targetKey, true, SameTargetCooldown);
            _cache.Set(rlKey, count + 1, Window);

            return Ok(new { report.Id });
        }

        // =========================
        // ✅ ADMIN ENDPOINTS
        // =========================

        // GET: api/Reports/admin?status=Pending&targetType=Item&search=spam&page=1&pageSize=20
        [HttpGet("admin")]
        public async Task<IActionResult> AdminList(
            [FromQuery] ReportStatus? status,
            [FromQuery] ReportTargetType? targetType,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 100) pageSize = 100;

            var (items, total) = await _unitOfWork.ReportRepository.GetPagedAsync(status, targetType, search, page, pageSize);

            // enrich: item/review title snippet
            var itemIds = items.Where(r => r.TargetType == ReportTargetType.Item).Select(r => r.TargetId).Distinct().ToList();
            var reviewIds = items.Where(r => r.TargetType == ReportTargetType.Review).Select(r => r.TargetId).Distinct().ToList();

            var itemMap = new Dictionary<Guid, string>();
            foreach (var iid in itemIds)
            {
                var it = await _unitOfWork.ItemRepository.GetByIdAsync(iid);
                if (it != null) itemMap[iid] = it.Name;
            }

            var reviewMap = new Dictionary<Guid, string>();
            var reviewItemMap = new Dictionary<Guid, Guid>(); // ✅ ReviewId -> ItemId

            foreach (var rid in reviewIds)
            {
                var rv = await _unitOfWork.ReviewRepository.GetByIdAsync(rid);
                if (rv != null)
                {
                    reviewMap[rid] = (rv.Comment ?? "").Length > 60 ? (rv.Comment[..60] + "...") : (rv.Comment ?? "");
                    reviewItemMap[rid] = rv.ItemId; // ✅ مهم جداً
                }
            }


            var dtos = items.Select(r => new ReportAdminDto
            {
                Id = r.Id,
                ReporterUserId = r.ReporterUserId,
                ReporterName = r.ReporterUser?.FullName ?? r.ReporterUser?.UserName ?? r.ReporterUserId,
                ReporterEmail = r.ReporterUser?.Email,
                TargetType = r.TargetType,
                TargetId = r.TargetId,

                TargetItemId = r.TargetType == ReportTargetType.Review
    ? (reviewItemMap.TryGetValue(r.TargetId, out var iid) ? iid : (Guid?)null)
    : r.TargetId,

                TargetTitle = r.TargetType == ReportTargetType.Item
                    ? (itemMap.TryGetValue(r.TargetId, out var name) ? name : "(Item not found)")
                    : (reviewMap.TryGetValue(r.TargetId, out var snippet) ? snippet : "(Review not found)"),
                Reason = r.Reason,
                Details = r.Details,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(new PagedResult<ReportAdminDto>
            {
                Items = dtos,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/Reports/admin/{id}
        [HttpGet("admin/{id:guid}")]
        public async Task<IActionResult> AdminGet(Guid id)
        {
            var r = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (r == null) return NotFound();

            return Ok(new ReportAdminDto
            {
                Id = r.Id,
                ReporterUserId = r.ReporterUserId,
                ReporterName = r.ReporterUser?.FullName ?? r.ReporterUser?.UserName ?? r.ReporterUserId,
                ReporterEmail = r.ReporterUser?.Email,
                TargetType = r.TargetType,
                TargetId = r.TargetId,
                TargetTitle = null,
                Reason = r.Reason,
                Details = r.Details,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
        }

        // PATCH: api/Reports/admin/{id}/status?status=Resolved
        [HttpPatch("admin/{id:guid}/status")]
        public async Task<IActionResult> AdminUpdateStatus(Guid id, [FromQuery] ReportStatus status)
        {
            await _unitOfWork.ReportRepository.UpdateStatusAsync(id, status);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        // GET: api/Reports/admin/pending-count
        [HttpGet("admin/pending-count")]
        public async Task<IActionResult> AdminPendingCount()
        {
            var count = await _unitOfWork.ReportRepository.GetPendingCountAsync();
            return Ok(count);
        }

    }

    // DTOs for Admin response
    public class ReportAdminDto
    {
        public Guid Id { get; set; }
        public string ReporterUserId { get; set; } = "";
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }

        public ReportTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public string? TargetTitle { get; set; }
        public Guid? TargetItemId { get; set; }

        public string Reason { get; set; } = "";
        public string? Details { get; set; }

        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}