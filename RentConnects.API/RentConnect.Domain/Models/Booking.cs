using RentConnect.API.Enums;
using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.API.RentConnect.Domain.Models;

public class Booking : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public string RenterId { get; set; }
    public ApplicationUser Renter { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BookingStatus Status { get; set; }

}