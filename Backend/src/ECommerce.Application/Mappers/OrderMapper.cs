using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderMapper
{
    public static OrderResponse ToResponse(
        this Order order,
        IReadOnlyDictionary<Guid, Image?>? productImages = null)
    {
        return new OrderResponse(
            order.Id,
            order.CreatedAt,
            order.UpdatedAt,
            order.Address.ToResponse(),
            order.Status,
            order.Items
                .Select(item =>
                {
                    var image = productImages is not null &&
                        productImages.TryGetValue(item.ProductId, out var productImage)
                            ? productImage
                            : null;

                    return item.ToResponse(image);
                })
                .ToList()
        );
    }
    
    public static
        ICollection<OrderResponse> ToResponse(
            ICollection<Order> orders,
            IReadOnlyDictionary<Guid, Image?>? productImages = null)
    {
        return orders
            .Select(order => order.ToResponse(productImages))
            .ToList();
    }
}