using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class OrderItem
{   
    private OrderItem()
    {
    }

    private OrderItem(Guid id, Order order, Product product, Quantity quantity, Customization? customization = null)
    {
        Id = id;
        Order = order;
        OrderId = order.Id;
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        ProductId = product.Id;
        ProductName = product.Name;
        UnitPrice = product.Price;
        Customization = customization;
        CustomizationId = customization?.Id;
    }

    public Guid Id { get; private set; }
    public Order Order { get; private set; }
    public Guid OrderId { get; private set; }
    public Product Product { get; private set; }
    public Guid? ProductId { get; private set; }
    public Customization? Customization { get; private set; }
    public Guid? CustomizationId { get; private set; }
    public Quantity Quantity { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; }
    //public Product Product { get; private set; } solo en memoria, no se guarda en Postgres

    internal static OrderItem Create(Order order, Product product, Quantity quantity, Customization? customization = null)
    {
        if (order == null) throw new ArgumentNullException(nameof(order), "Cart cannot be null.");

        if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        
        return new OrderItem(Guid.NewGuid(), order, product, quantity, customization);
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Quantity = Quantity.Create(quantity);
    }
    
    public void AddCustomization(string description, bool wrap = false)
    {
        var customization = Customization.Create(description, wrap: wrap);
        
        Customization = customization;
        CustomizationId = customization.Id;
    }

    public void UpdateCustomization(Customization? customization)
    {
        Customization = customization;
        CustomizationId = customization?.Id;
    }

    public void RestoreProduct(Product product)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        ProductId = product.Id;
        ProductName = product.Name;
        UnitPrice = product.Price;
    }
}