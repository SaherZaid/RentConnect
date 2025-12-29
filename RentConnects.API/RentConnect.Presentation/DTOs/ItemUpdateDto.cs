using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Presentation.DTOs
{
    public class ItemUpdateDto
    {
        [Required] public Guid Id { get; set; }

        [Required, MinLength(2)] public string Name { get; set; } = "";
        [Required, MinLength(5)] public string Description { get; set; } = "";

        [Range(0, 1000000)] public double PricePerDay { get; set; }

        [Required] public Guid CategoryId { get; set; }
        [Required] public string City { get; set; } = "";

        public bool IsShippable { get; set; }
        public string? Notes { get; set; }
    }
}
