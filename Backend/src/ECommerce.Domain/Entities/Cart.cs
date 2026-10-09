using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Cart
{
    private Cart()
    {
    }

    private Cart(Guid id, User user, DateTime createdAt, DateTime updatedAt, CartStatus status)
    {
        Id = id;
        User = user;
        UserId = user.Id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Status = status;
        Items = new List<CartItem>();
    }

    public Guid Id { get; private set; }
    public User User { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public CartStatus Status { get; private set; }
    public ICollection<CartItem> Items { get; private set; }

    public static Cart Create(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        var now = DateTime.UtcNow;
        return new Cart(Guid.NewGuid(), user, now, now, CartStatus.Active);
    }

    public void AddCartItem(Product product, Quantity quantity, Customization? customization = null)
    {
        if (Status == CartStatus.Converted)
            throw new InvalidOperationException("Cannot add items to a converted cart.");

        var item = CartItem.Create(this, product, quantity);
        
        item.UpdateCustomization(customization);

        Items.Add(item);
    }

    public void UpdateTimestamp()
    {
        if (Status == CartStatus.Converted)
            throw new InvalidOperationException("Cannot update timestamp for a converted cart.");

        UpdatedAt = DateTime.UtcNow;
    }

    public void Convert()
    {
        if (Status == CartStatus.Converted) throw new InvalidOperationException("Cart is already converted.");

        Status = CartStatus.Converted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItems(ICollection<CartItem> items)
    {
        UpdateTimestamp();
        Items = items;
    }
}