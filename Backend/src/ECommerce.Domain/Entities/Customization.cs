namespace ECommerce.Domain.Models;

public class Customization
{
    private Customization()
    {
    }

    private Customization(Guid id, string? description, DateTime createdAt, decimal additionalPrice, bool wrap)
    {
        Id = id;
        Description = description;
        CreatedAt = createdAt;
        IsWrap = wrap;
        AdditionalPrice = additionalPrice;
    }

    public Guid Id { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsWrap { get; private set; }
    public decimal AdditionalPrice { get; private set; }

    public static Customization Create(string? description, bool wrap = false, decimal additionalPrice = 0)
    {
        if (description is { Length: > 500 })
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));

        if (additionalPrice < 0)
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));

        return new Customization(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DateTime.UtcNow,
            additionalPrice,
            wrap);
    }

    public void Update(string? description, decimal additionalPrice)
    {
        if (description is { Length: > 500 })
            throw new ArgumentException("Customization description cannot exceed 500 characters.", nameof(description));

        if (additionalPrice < 0)
            throw new ArgumentException("Additional price cannot be negative.", nameof(additionalPrice));

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        AdditionalPrice = additionalPrice;
    }
}