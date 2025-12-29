using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class CategoryDto : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<ItemDto> Items { get; set; } = new List<ItemDto>();
}