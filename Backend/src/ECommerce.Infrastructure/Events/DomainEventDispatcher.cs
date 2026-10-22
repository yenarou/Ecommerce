using ECommerce.Application.Events;
using ECommerce.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Events;

public sealed class DomainEventDispatcher(
    IServiceProvider serviceProvider
) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchEventAsync(
                (dynamic)domainEvent,
                cancellationToken
            );
        }
    }

    private async Task DispatchEventAsync<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        var handlers = serviceProvider
            .GetServices<IDomainEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(
                domainEvent,
                cancellationToken
            );
        }
    }
}