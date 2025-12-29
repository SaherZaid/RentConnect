using RentConnect.API.RentConnect.Domain.Interfaces;
using RentConnect.API.RentConnect.Domain.Models;
using RentConnect.API.RentConnect.Presentation.DTOs;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double PricePerDay { get; set; }
    public string OwnerId { get; set; }
    public string? OwnerUserName { get; set; }

    public string? OwnerFullName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string City { get; set; }
    public bool IsShippable { get; set; }
    public string? Notes { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<ItemImageDto> Images { get; set; } = new();

    public int BookingCount { get; set; }
    public double AverageRating { get; set; }
}