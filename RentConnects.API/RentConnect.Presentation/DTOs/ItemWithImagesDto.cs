using Microsoft.Build.Framework;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class ItemWithImagesDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public double PricePerDay { get; set; }

    [Required]
    public string OwnerId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public string City { get; set; }
    public bool IsShippable { get; set; }
    public string? Notes { get; set; }
    public List<IFormFile> Images { get; set; } = new();
}