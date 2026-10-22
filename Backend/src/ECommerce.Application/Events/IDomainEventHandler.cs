using ECommerce.Domain.Events;

namespace ECommerce.Application.Events;

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(
        TEvent domainEvent,
        CancellationToken cancellationToken = default
    );
}