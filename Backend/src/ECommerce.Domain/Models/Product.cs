using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class Product
{
    public Guid Id { get; private set;}
    public string Name { get; private set;}
    public string Description { get; private set;}
    public Money Price { get; private set;}
    public Quantity Stock{ get; private set;}
    public DateTime CreatedAt { get; private set;}
    public Category Category { get; private set;}
    
    private Product(Guid id, string name, string description, Money price, Quantity stock, Category category)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }

    public Product CreateProduct(string name, string description, Money price, Quantity stock, Category category)
    {
        return new Product(Guid.NewGuid(), name, description, price, stock, category);
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
    
    public void UpdateDescription(string description)
    {
        Description = description;
    }
    
    public void UpdateCategory(Category category)
    {
        Category = category;
    }

    public void UpdateStock(Quantity stock)
    {
        Stock = stock;
    }

    public void UpdatePrice(Money price)
    {
        Price = price;
    }

}