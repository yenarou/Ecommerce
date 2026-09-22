namespace ECommerce.Domain.Exceptions;


public class InsufficientQuantityException(int availableQuantity, int requestedQuantity) : DomainException($"Cant decrease that amount")
{
    public int AvailableQuantity { get; } = availableQuantity;
    public int RequestedQuantity { get; } = requestedQuantity;
}