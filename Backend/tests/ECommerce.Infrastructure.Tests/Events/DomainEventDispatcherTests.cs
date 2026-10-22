using ECommerce.Application.Events;
using ECommerce.Domain.Events;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Tests.Events;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_DispatchesEventsToMatchingHandlersInOrder()
    {
        var receivedEvents = new List<StockChangedDomainEvent>();
        var handler = new RecordingHandler(receivedEvents);
        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<StockChangedDomainEvent>>(handler);

        using var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider);
        var firstEvent = CreateEvent(1);
        var secondEvent = CreateEvent(2);

        await dispatcher.DispatchAsync([firstEvent, secondEvent]);

        Assert.Equal([firstEvent, secondEvent], receivedEvents);
    }

    [Fact]
    public async Task DispatchAsync_PassesCancellationTokenToHandler()
    {
        var handler = new RecordingHandler([]);
        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<StockChangedDomainEvent>>(handler);
        using var serviceProvider = services.BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(serviceProvider);
        using var cancellationSource = new CancellationTokenSource();

        await dispatcher.DispatchAsync(
            [CreateEvent(1)],
            cancellationSource.Token);

        Assert.Equal(cancellationSource.Token, handler.ReceivedCancellationToken);
    }

    private static StockChangedDomainEvent CreateEvent(int newStock)
    {
        return new StockChangedDomainEvent(
            Guid.NewGuid(),
            Quantity.Create(newStock - 1),
            Quantity.Create(newStock));
    }

    private sealed class RecordingHandler(
        ICollection<StockChangedDomainEvent> receivedEvents)
        : IDomainEventHandler<StockChangedDomainEvent>
    {
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task HandleAsync(
            StockChangedDomainEvent domainEvent,
            CancellationToken cancellationToken = default)
        {
            receivedEvents.Add(domainEvent);
            ReceivedCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }
}
