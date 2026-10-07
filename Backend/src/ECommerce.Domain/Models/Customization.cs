namespace ECommerce.Domain.Models;

public class Customization
{
    private Customization()
    {
    }

    private Customization(Guid id, string description, DateTime createdAt, decimal additionalPrice, bool wrap)
    {
        Id = id;
        Description = description;
        CreatedAt = createdAt;
        AdditionalPrice = additionalPrice;
    }

    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsWrap { get; private set; }
    public decimal AdditionalPrice { get; private set; }

    public static Customization Create(string description, bool wrap = false, decimal additionalPrice = 0)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Customization description cannot be null or empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));

        if (additionalPrice < 0)
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));

        return new Customization(Guid.NewGuid(), description, DateTime.UtcNow, additionalPrice, wrap);
    }

    public void Update(string description, decimal additionalPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Customization description cannot be null or empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));

        if (additionalPrice < 0)
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));

        Description = description;
        AdditionalPrice = additionalPrice;
    }
}