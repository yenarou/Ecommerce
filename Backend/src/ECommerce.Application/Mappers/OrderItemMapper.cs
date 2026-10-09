using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderItemMapper
{
    public static OrderItemResponse ToResponse(this OrderItem item)
    {
        return new OrderItemResponse(
            item.Id,
            item.ProductId ?? throw new InvalidOperationException(
                $"Order item {item.Id} has no stored product ID."),
            item.Product.ToResponse(),
            item.Customization?.ToResponse(),
            item.Quantity.Value
        );
    }
}