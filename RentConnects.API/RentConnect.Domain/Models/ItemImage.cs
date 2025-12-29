using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.API.RentConnect.Domain.Models;

public class ItemImage : IEntity<Guid>

{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }      // Foreign Key
    public Item Item { get; set; }         // Navigation property
    public string ImageUrl { get; set; }
}