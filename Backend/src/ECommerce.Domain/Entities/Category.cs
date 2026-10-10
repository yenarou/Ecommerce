namespace ECommerce.Domain.Models;

public class Category
{
    private Category()
    {
    }

    private Category(string slug, string name, string description, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        Slug = slug; 
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static Category Create(string slug, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

        if (name.Length > 100) throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(slug));

        if (slug.Length > 100) throw new ArgumentException("Slug cannot exceed 100 characters.", nameof(name));

        if (description is { Length: > 500 })
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));

        return new Category(slug, name, description, DateTime.UtcNow);
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(name));

        if (name.Length > 100) throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));

        if (description is { Length: > 500 })
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));

        Name = name;
        Description = description;
    }
}