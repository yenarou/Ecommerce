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

    internal static Customization Create(string description, decimal additionalPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Customization description cannot be null or empty.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));
        }

        if (additionalPrice < 0)
        {
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));
        }

        return new Customization(Guid.NewGuid(), description, DateTime.UtcNow, additionalPrice);
    }

    public void Update(string description, decimal additionalPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Customization description cannot be null or empty.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));
        }

        if (additionalPrice < 0)
        {
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));
        }

        Description = description;
        AdditionalPrice = additionalPrice;
    }

    
}