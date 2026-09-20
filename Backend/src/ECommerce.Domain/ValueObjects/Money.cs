using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency must be a valid 3-letter ISO code.");

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency) => Create(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (Amount - other.Amount < 0)
            throw new DomainException("Resulting amount cannot be negative.");
        return new Money(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on different currencies: {Currency} vs {other.Currency}.");
    }

    public override string ToString() => $"{Amount} {Currency}";
}