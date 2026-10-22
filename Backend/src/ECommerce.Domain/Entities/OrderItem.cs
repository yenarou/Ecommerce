using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class OrderItem
{   
    private OrderItem()
    {
    }

    private OrderItem(
        Guid id,
        Order order,
        Product product,
        Quantity quantity,
        Customization? customization = null
        )
    {
        Id = id;
        Order = order;
        OrderId = order.Id;

        ProductId = product.Id;
        ProductName = product.Name;
        UnitPrice = product.Price;

        Quantity = quantity;

        Customization = customization;
        CustomizationId = customization?.Id;
        
        ProductImage = product.Images.FirstOrDefault();
        ProductImageId = ProductImage?.Id ?? Guid.Empty;
    }


    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;
    
    public Guid? ProductImageId { get; private set; }
    public Image? ProductImage { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    

    public Quantity Quantity { get; private set; } = null!;

    public Customization? Customization { get; private set; }
    public Guid? CustomizationId { get; private set; }

    internal static OrderItem Create(
        Order order,
        Product product,
        Quantity quantity,
        Customization? customization = null)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(quantity);

        return new OrderItem(
            Guid.NewGuid(),
            order,
            product,
            quantity,
            customization);
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
    
}