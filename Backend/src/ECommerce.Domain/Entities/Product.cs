using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Product
{
    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        Price = null!;
        Stock = null!;
        Category = null!;
        Images = new List<Image>();
    }

    private Product(Guid id, string name, string description, Money price, Quantity stock, Category category, ICollection<Image> images)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Category = category;
        CategoryId = category.Id;
        CreatedAt = DateTime.UtcNow;
        IsPublished = true;
        Images = images;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public Quantity Stock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Category Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool IsPublished { get; private set; }
    public ICollection<Image> Images { get; private set; }
    
    public static Product Create(string name, string description, Money price, Quantity stock, Category category, ICollection<Image> images)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(name));

        if (name.Length > 200) throw new ArgumentException("Product name cannot exceed 200 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Product description cannot exceed 1000 characters.", nameof(description));

        if (price == null) throw new ArgumentNullException(nameof(price), "Price cannot be null.");

        if (stock == null) throw new ArgumentNullException(nameof(stock), "Stock cannot be null.");

        if (category == null) throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        return new Product(Guid.NewGuid(), name, description, price, stock, category, images);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(name));

        if (name.Length > 200) throw new ArgumentException("Product name cannot exceed 200 characters.", nameof(name));

        Name = name;
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty.", nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException("Product description cannot exceed 1000 characters.", nameof(description));

        Description = description;
    }

    public void UpdateCategory(Category category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        Category = category;
        CategoryId = category.Id;
    }

    public void RestoreCategory(Category category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category), "Category cannot be null.");
        if (category.Id != CategoryId)
            throw new ArgumentException("Category ID does not match the product category ID.", nameof(category));

        Category = category;
    }

    public void UpdateStock(Quantity stock)
    {
        if (stock == null) throw new ArgumentNullException(nameof(stock), "Stock cannot be null.");

        Stock = stock;
    }

    public void UpdatePrice(Money price)
    {
        if (price == null) throw new ArgumentNullException(nameof(price), "Price cannot be null.");

        Price = price;
    }

    public void Publish()
    {
        IsPublished = true;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }

    public void AddImage(string url, string alt)
    {
        var image = Image.Create(url, alt);
        Images.Add(image);
    }

    public void RemoveImage(Image image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        Images.Remove(image);
    }
}