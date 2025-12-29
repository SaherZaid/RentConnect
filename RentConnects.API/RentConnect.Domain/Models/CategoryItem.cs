using RentConnect.API.RentConnect.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RentConnect.API.RentConnect.Domain.Models;

public class CategoryItem : IEntity<Guid>
{
    [Key]
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; }
}