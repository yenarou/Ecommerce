namespace ECommerce.Domain.Models;

public class Customization
{
    public Guid Id { get; private set;}
    public String Description { get; private set;}
    public DateTime CreatedAt { get; private set;}
    public Decimal AdditionalPrice { get; private set;}
    
    private Customization() { }

    private Customization(Guid id, string description, DateTime createdAt, decimal additionalPrice)
    {
        Id = id;
        Description = description;
        CreatedAt = createdAt;
        AdditionalPrice = additionalPrice;
    }

    public static Customization Create(string description, decimal additionalPrice)
    {
        return new Customization(Guid.NewGuid(), description, DateTime.UtcNow, additionalPrice);
    }

    public void Update(string description, decimal additionalPrice)
    {
        Description = description;
        AdditionalPrice = additionalPrice;
    }

    
}