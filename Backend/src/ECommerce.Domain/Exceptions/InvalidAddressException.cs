namespace ECommerce.Domain.Exceptions;

public class InvalidAddressException(string invalidField) : DomainException($"Invalid address field: {invalidField}")
{
    public string InvalidField { get; } = invalidField;
}