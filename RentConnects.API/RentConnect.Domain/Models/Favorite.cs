using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Domain.Models;

public class Favorite
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public Guid ItemId { get; set; }

    [ForeignKey("ItemId")]
    public Item Item { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}