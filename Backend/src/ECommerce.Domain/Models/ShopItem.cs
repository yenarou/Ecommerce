namespace ECommerce.Domain.Models;

public class ShopItem
{
    private ShopItem()
    {
    }

    private ShopItem(Product product, int price)
    {
        Id = Guid.NewGuid();
        Product = product;
        Price = price;
    }

    public ShopItem(Guid? id, Product product, int price)
    {
        Id = id ?? Guid.NewGuid();
        Product = product;
        Price = price;
    }

    public Guid Id { get; private set; }
    public Product Product { get; private set; }
    public int Price { get; private set; }

    public static ShopItem Create(Product product, int price)
    {
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        return new ShopItem(product, price);
    }

    public bool Available()
    {
        return Product.Stock.Value > 0;
    }
}