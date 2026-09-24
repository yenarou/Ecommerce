namespace ECommerce.Domain.Exceptions;

public class DifferentCurrenciesException(string currency1, string currency2)
    : DomainException($"Cannot operate on different currencies: {currency1} vs {currency2}")
{
    public string Currency1 { get; } = currency1;
    public string Currency2 { get; } = currency2;
}