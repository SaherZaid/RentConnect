using RentConnect.API.RentConnect.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Domain.Models;

public class Review : IEntity<Guid>
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; } // Reference to the user who submitted the review
    public ApplicationUser User { get; set; }

    [Required]
    public Guid ItemId { get; set; } // Reference to the reviewed product
    public Item Item { get; set; }

    [Required]
    [Range(1, 5)] // Rating range is 1 to 5 stars
    public int Rating { get; set; }

    public string Comment { get; set; } // comment for the review

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set creation time
}