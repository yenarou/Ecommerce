using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Quantity
{
    public int Value { get; }

    private Quantity(int value) => Value = value;

    public static Quantity Create(int value)
    {
        if (value <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        return new Quantity(value);
    }

    public Quantity Increase(int amount) => Create(Value + amount);

    public Quantity Decrease(int amount)
    {
        if (amount > Value)
            throw new DomainException("Cannot decrease quantity below zero.");
        return Create(Value - amount);
    }

    public override string ToString() => Value.ToString();
}