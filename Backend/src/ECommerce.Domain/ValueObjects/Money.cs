using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Money
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new InvalidCurrencyException(currency);

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency)
    {
        return Create(0, currency);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Add(Decimal value)
    {
        Money other = Money.Create(value, Currency);
        
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (Amount - other.Amount < 0)
            throw new ArgumentOutOfRangeException(nameof(Money), other, "Insufficient funds.");
        return new Money(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DifferentCurrenciesException(Currency, other.Currency);
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }
}