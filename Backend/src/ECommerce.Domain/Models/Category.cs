namespace ECommerce.Domain.Models;

public class Category
{
    public Guid Id { get; private set;}
    public string Name { get; private set;}
    public string Description { get; private set;}
    public DateTime CreatedAt { get; private set;}
    
    private Category() { }

    private Category(Guid id, string name, string description, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    public static Category Create(string name, string description)
    {
        return new Category(Guid.NewGuid(), name, description, DateTime.UtcNow);
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

}