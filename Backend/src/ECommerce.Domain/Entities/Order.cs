using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class Order
{
    private Order()
    {
    }

    private Order(Guid id, User user, DateTime createdAt, DateTime updatedAt, Address address, OrderStatus status)
    {
        Id = id;
        User = user;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Address = address;
        Status = status;
        Items = new List<OrderItem>();
    }

    public Guid Id { get; private set; }
    public User User { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Address Address { get; private set; }
    public OrderStatus Status { get; private set; }
    public ICollection<OrderItem> Items { get; private set; }
    #warning 

    public static Order Create(User user, Address address)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        if (address == null) throw new ArgumentNullException(nameof(address), "Address cannot be null.");

        var now = DateTime.UtcNow;
        return new Order(Guid.NewGuid(), user, now, now, address, OrderStatus.Pending);
    }

    public void AddOrderItem(CartItem cartItem)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot add items to an order with status '{Status}'.");

        var orderItem = OrderItem.Create(this, cartItem.Product, cartItem.Quantity);

        Items.Add(orderItem);
    }

    public void AddOrderItem(OrderItem orderItem)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot add items to an order with status '{Status}'.");

        Items.Add(orderItem);
    }

    public void AddOrderItems(IEnumerable<OrderItem> orderItems)
    {
        if (orderItems == null) throw new ArgumentNullException(nameof(orderItems), "Order items cannot be null.");

        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot add items to an order with status '{Status}'.");

        foreach (var orderItem in orderItems) Items.Add(orderItem);
    }

    public void AddOrderItems(Cart cart)
    {
        if (cart == null) throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");

        if (cart.Items == null || !cart.Items.Any())
            throw new ArgumentException("Cart has no items to add.", nameof(cart));

        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot add items to an order with status '{Status}'.");

        foreach (var cartItem in cart.Items) AddOrderItem(cartItem);
    }


    public void UpdateAddress(Address address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address), "Address cannot be null.");

        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot update address for an order with status '{Status}'.");

        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus status)
    {
        if (status == null) throw new ArgumentNullException(nameof(status), "Order status cannot be null.");

        // Use the TransitionTo method to validate state transitions
        Status = Status.TransitionTo(status);
        UpdatedAt = DateTime.UtcNow;
    }

    public Money GetTotal()
    {
        string currency = Items.FirstOrDefault().Product.Price.Currency;

        Money total = Money.Create(0, currency); 
        
        foreach (var item in Items)
        {
            total.Add(item.Product.Price);
        }
        
        return total;
    }
}