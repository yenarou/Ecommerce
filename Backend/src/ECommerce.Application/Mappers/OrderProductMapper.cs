using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderProductMapper
{
    public static OrderProductResponse ToResponse(this Product product)
    {
        return new OrderProductResponse(
            product.Id,
            product.Name,
            product.Price.Amount,
            product.Price.Currency,
            new ImageResponse(
                product.Images.FirstOrDefault()?.Id,
                product.Images.FirstOrDefault()?.Url,
                product.Images.FirstOrDefault()?.Alt
            )
        );
    }
}