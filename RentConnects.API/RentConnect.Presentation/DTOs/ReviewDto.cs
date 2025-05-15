using RentConnect.API.RentConnect.Domain.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class ReviewDto : IEntity<Guid>
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    public string Comment { get; set; }
    public string? UserFullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}