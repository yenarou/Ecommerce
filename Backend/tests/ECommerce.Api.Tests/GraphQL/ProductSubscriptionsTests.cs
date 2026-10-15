using ECommerce.Api.GraphQL.Subscriptions;
using ECommerce.Application.Events;

namespace ECommerce.Api.Tests.GraphQL;

public class ProductSubscriptionsTests
{
    [Fact]
    public void StockUpdated_ReturnsPublishedEventMessage()
    {
        var subscriptions = new ProductSubscriptions();
        var message = new StockUpdatedEvent(Guid.NewGuid(), 12);

        var result = subscriptions.StockUpdated(
            message.ProductId,
            message);

        Assert.Same(message, result);
    }
}
