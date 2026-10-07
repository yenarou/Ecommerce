using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class OrderItem
{
    private OrderItem()
    {
    }

    private OrderItem(Guid id, Order order, Product product, Quantity quantity, Customization? customization = null)
    {
        Id = id;
        Order = order;
        Product = product;
        Quantity = quantity;
        Customization = customization;
    }

    public Guid Id { get; private set; }
    public Order Order { get; private set; }
    public Product Product { get; private set; }
    public Customization? Customization { get; private set; }
    public Quantity Quantity { get; private set; }

    internal static OrderItem Create(Order order, Product product, Quantity quantity, Customization? customization = null)
    {
        if (order == null) throw new ArgumentNullException(nameof(order), "Cart cannot be null.");

        if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        
        return new OrderItem(Guid.NewGuid(), order, product, quantity);
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Quantity = Quantity.Create(quantity);
    }
    
    public void AddCustomization(string description, bool wrap = false)
    {
        var customization = Models.Customization.Create(description, wrap: wrap);
        
        Customization = customization;
    }
}