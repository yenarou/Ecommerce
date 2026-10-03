namespace ECommerce.Domain.Exceptions;

public class InvalidOrderStatusTransition(string statusFrom, string statusTo)
    : DomainException($"Cannot transition from '{statusFrom}' to '{statusTo}'.")
{
    public string StatusFrom { get; } = statusFrom;
    public string StatusTo { get; } = statusTo;
}