namespace ECommerce.Application.Events;

public sealed record StockUpdatedEvent(
    Guid ProductId,
    int Stock
);