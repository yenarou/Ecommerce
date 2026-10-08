using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Mappers;

public static class CartMapper
{
    public static CartResponse CartToResponse(Cart cart)
    {
        return new CartResponse(
            cart.Id,
            cart.Status.ToString(),
            cart.UpdatedAt,
            cart.Items
                .Select(item => new CartItemResponse(
                    item.Id,
                    item.Quantity.Value,
                    new ProductResponse(
                        item.Product.Id,
                        item.Product.Name,
                        item.Product.Description,
                        item.Product.Price.Amount,
                        item.Product.Price.Currency,
                        item.Product.Stock.Value,
                        item.Product.CreatedAt,
                        item.Product.Category.Id,
                        item.Product.Category.Name,
                        item.Product.Images
                            .Select(image => new ImageResponse(
                                image.Id,
                                image.Url,
                                image.Alt
                            ))
                            .ToList()
                    ),
                    item.Customization is null
                        ? null
                        : new CustomizationResponse(
                            item.Customization.Description,
                            item.Customization.IsWrap
                        )
                ))
                .ToList()
        );
    }
    
    
    
}