using RentConnect.API.RentConnect.Domain.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace RentConnect.API.RentConnect.Domain.Models;

public class Item : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double PricePerDay { get; set; }
    public string OwnerId { get; set; } // Foreign key to ApplicationUser
    public ApplicationUser Owner { get; set; } // Navigation property
    public Guid CategoryId { get; set; }   //Fk
    public Category Category { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<ItemImage> Images { get; set; }
    public string City { get; set; }
    public bool IsShippable { get; set; }
    public string? Notes { get; set; }

}