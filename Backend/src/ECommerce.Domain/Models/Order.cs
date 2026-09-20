using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class Order
{
    public Guid Id { get; private set;}
    public User User { get; private set;}
    public DateTime CreatedAt { get; private set;}
    public DateTime UpdatedAt { get; private set;}
    public Address Address { get; private set;}
    public OrderStatus Status { get; private set;}

    private Order() { }

    private Order(Guid id, User user, DateTime createdAt, DateTime updatedAt, Address address, OrderStatus status)
    {
        Id = id;
        User = user;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Address = address;
        Status = status;
    }

    public static Order Create(User user, Address address)
    {
        var now = DateTime.UtcNow;
        return new Order(Guid.NewGuid(), user, now, now, address, OrderStatus.Pending);
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}