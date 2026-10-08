using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Models;

namespace ECommerce.Application.Mappers;

public static class OrderItemMapper
{
    public static OrderItemResponse ToResponse(this OrderItem item)
    {
        return new OrderItemResponse(
            item.Id,
            item.Product.ToResponse(),
            item.Customization?.ToResponse(),
            item.Quantity.Value
        );
    }
}