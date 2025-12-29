using RentConnect.API.RentConnect.Domain.Interfaces;

namespace RentConnect.API.RentConnect.Domain.Models;

public class Category : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }

}