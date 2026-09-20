namespace ECommerce.Domain.Models;

public class CartItem
{
    public Guid Id { get; private set;}
    public Cart Cart { get; private set;}
    public Product Product { get; private set;}
    public int Quantity { get; private set;}
    
    private CartItem() { }

    private CartItem(Guid id, Cart cart, Product product, int quantity)
    {
        Id = id;
        Cart = cart;
        Product = product;
        Quantity = quantity;
    }

    public static CartItem Create(Cart cart, Product product, int quantity)
    {
        return new CartItem(Guid.NewGuid(), cart, product, quantity);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    
}