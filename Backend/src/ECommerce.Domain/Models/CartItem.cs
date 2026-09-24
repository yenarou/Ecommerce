namespace ECommerce.Domain.Models;

public class CartItem
{
    public Guid Id { get; private set;}
    public Cart Cart { get; private set;}
    public Product Product { get; private set;}
    public int Quantity { get; private set;}
    public Customization? Customization { get; private set;}
    
    private CartItem() { }

    private CartItem(Guid id, Cart cart, Product product, int quantity, Customization? customization = null)
    {
        Id = id;
        Cart = cart;
        Product = product;
        Quantity = quantity;
        Customization = customization;       
    }

    internal static CartItem Create(Cart cart, Product product, int quantity, Customization customization = null)
    {
        if (cart == null)
        {
            throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");
        }

        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        return new CartItem(Guid.NewGuid(), cart, product, quantity);
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        Quantity = quantity;
    }

    
}