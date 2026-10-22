using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderItemMapper
{
    public static OrderItemResponse ToResponse(this OrderItem item)
    {
        return new OrderItemResponse(
            item.Id,
            item.ProductId,
            new OrderProductResponse(
                item.ProductId,
                item.ProductName,
                item.UnitPrice.Amount,
                item.UnitPrice.Currency
            ),
            item.Customization?.ToResponse(),
            item.Quantity.Value
        );
    }
}