using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Presentation.DTOs
{
    public class ContactRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Topic { get; set; } = "general";

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Message { get; set; } = string.Empty;
    }
}
