using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Presentation.DTOs
{
    public class ItemUpdateWithImagesDto
    {
        [Required] public Guid Id { get; set; }

        [Required] public string Name { get; set; } = "";
        [Required] public string Description { get; set; } = "";

        [Range(0, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required] public Guid CategoryId { get; set; }

        [Required] public string City { get; set; } = "";

        public bool IsShippable { get; set; }
        public string? Notes { get; set; }

        // صور جديدة يضيفها
        public List<IFormFile>? NewImages { get; set; }

        // صور قديمة يبغى يحذفها (بالـ Id)
        public List<Guid>? DeletedImageIds { get; set; }
    }
}
