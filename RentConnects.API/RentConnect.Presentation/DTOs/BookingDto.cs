using RentConnect.API.Enums;
using System;

namespace RentConnect.Presentation.UI.RentConnect.Presentation.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public string RenterId { get; set; } = string.Empty;
    public string? RenterName { get; set; }
    public string? RenterEmail { get; set; }
    public string? RenterPhone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public BookingStatus Status { get; set; }

    public string? ItemImageUrl { get; set; }

}