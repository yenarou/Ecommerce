namespace ECommerce.Domain.Exceptions;

public class InvalidCurrencyException(string currency)
    : DomainException($"Currency must be a valid 3-letter ISO code   : {currency}")
{
    public string Currency { get; } = currency;
}