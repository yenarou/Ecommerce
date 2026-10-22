using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderItemMapper
{
    public static OrderItemResponse ToResponse(this OrderItem item, Image? productImage = null)
    {
        var image = productImage ?? item.ProductImage;

        return new OrderItemResponse(
            item.Id,
            item.ProductId,
            new OrderProductResponse(
                item.ProductId,
                item.ProductName,
                item.UnitPrice.Amount,
                item.UnitPrice.Currency,
                image is not null
                    ? new ImageResponse(image.Id, image.Url, image.Alt)
                    : null
            ),
            item.Customization?.ToResponse(),
            item.Quantity.Value
        );
    }
}