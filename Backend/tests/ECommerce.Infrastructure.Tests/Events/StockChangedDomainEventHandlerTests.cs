using ECommerce.Domain.Events;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Events.Handlers;
using HotChocolate.Subscriptions;
using Moq;

namespace ECommerce.Infrastructure.Tests.Events;

public class StockChangedDomainEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_SendsUpdatedStockToProductTopic()
    {
        var eventSender = new Mock<ITopicEventSender>();
        var handler = new StockChangedDomainEventHandler(eventSender.Object);
        var productId = Guid.NewGuid();
        using var cancellationSource = new CancellationTokenSource();
        var domainEvent = new StockChangedDomainEvent(
            productId,
            Quantity.Create(5),
            Quantity.Create(8));

        await handler.HandleAsync(domainEvent, cancellationSource.Token);

        eventSender.Verify(sender => sender.SendAsync(
                productId.ToString(),
                It.Is<ECommerce.Application.Events.StockUpdatedEvent>(message =>
                    message.ProductId == productId &&
                    message.Stock == 8),
                cancellationSource.Token),
            Times.Once);
    }
}
