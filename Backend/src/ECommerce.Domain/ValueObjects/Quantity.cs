using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Quantity
{
    private Quantity(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Quantity Create(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Quantity must be greater than zero.");

        return new Quantity(value);
    }

    public Quantity Increase(int amount)
    {
        return Create(Value + amount);
    }

    public Quantity Decrease(int amount)
    {
        if (amount > Value)
            throw new InsufficientQuantityException(Value, amount);
        return Create(Value - amount);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}