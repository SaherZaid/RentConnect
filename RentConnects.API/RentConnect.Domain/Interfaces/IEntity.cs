namespace RentConnect.API.RentConnect.Domain.Interfaces;

public interface IEntity<T>
{
    public T Id { get; set; }
}