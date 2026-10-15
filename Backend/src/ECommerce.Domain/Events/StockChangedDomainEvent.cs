using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Events;

public sealed record StockChangedDomainEvent(
    Guid ProductId,
    Quantity PreviousStock,
    Quantity NewStock
) : IDomainEvent;