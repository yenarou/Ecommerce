using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class CartItem
{
    private CartItem()
    {
    }

    private CartItem(Guid id, Cart cart, Product product, Quantity quantity, Customization? customization = null)
    {
        Id = id;
        Cart = cart;
        Product = product;
        Quantity = quantity;
        Customization = customization;
    }

    public Guid Id { get; private set; }
    public Cart Cart { get; private set; }
    public Product Product { get; private set; }
    public Quantity Quantity { get; private set; }
    public Customization? Customization { get; private set; }

    internal static CartItem Create(Cart cart, Product product, Quantity quantity, Customization customization = null)
    {
        if (cart == null) throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");

        if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null.");


        return new CartItem(Guid.NewGuid(), cart, product, quantity);
    }

    public void UpdateQuantity(Quantity quantity)
    {
        Quantity = quantity;
    }
    
    public void UpdateCustomization(string description, bool wrap = false)
    {
        var customization = Models.Customization.Create(description, wrap: wrap);
        
        Customization = customization;
    }

    public void UpdateCustomization(Customization? customization)
    {
        Customization = customization;
    }

    public void RestoreProduct(Product product)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
    }
}