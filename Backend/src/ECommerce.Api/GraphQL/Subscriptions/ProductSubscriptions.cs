using ECommerce.Application.Events;
using HotChocolate;
using HotChocolate.Types;

namespace ECommerce.Api.GraphQL.Subscriptions;

public sealed class ProductSubscriptions
{
    [Subscribe]
    [Topic("{productId}")]
    public StockUpdatedEvent StockUpdated(
        Guid productId,
        [EventMessage] StockUpdatedEvent message)
    {
        return message;
    }
}