using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class CategoryWithItemsDto : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ItemDto> Items { get; set; }
}