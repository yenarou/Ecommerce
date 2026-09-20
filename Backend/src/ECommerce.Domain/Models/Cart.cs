using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class Cart
{
    public Guid Id { get; private set;}
    public User User { get; private set;}
    public DateTime CreatedAt { get; private set;}
    public DateTime UpdatedAt { get; private set;}
    public CartStatus Status { get; private set;}

    private Cart() { }

    private Cart(Guid id, User user, DateTime createdAt, DateTime updatedAt, CartStatus status)
    {
        Id = id;
        User = user;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Status = status;
    }

    public static Cart Create(User user)
    {
        var now = DateTime.UtcNow;
        return new Cart(Guid.NewGuid(), user, now, now, CartStatus.Active);
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void Convert()
    {
        Status = CartStatus.Converted;
        UpdatedAt = DateTime.UtcNow;
    }

}