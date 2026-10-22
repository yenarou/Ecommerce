using ECommerce.Application.Events;
using ECommerce.Domain.Events;
using HotChocolate.Subscriptions;

namespace ECommerce.Infrastructure.Events.Handlers;

public sealed class StockChangedDomainEventHandler(
    ITopicEventSender eventSender)
    : IDomainEventHandler<StockChangedDomainEvent>
{
    public async Task HandleAsync(
        StockChangedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var message = new StockUpdatedEvent(
            domainEvent.ProductId,
            domainEvent.NewStock.Value);

        await eventSender.SendAsync(
            domainEvent.ProductId.ToString(),
            message,
            cancellationToken);
    }
}