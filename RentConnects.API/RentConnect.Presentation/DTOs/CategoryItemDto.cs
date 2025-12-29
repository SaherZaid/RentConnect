using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class CategoryItemDto
{
    public Guid CategoryId { get; set; }
    public List<Guid> ItemsIds { get; set; }
}