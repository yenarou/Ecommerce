using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Models;

namespace ECommerce.Application.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.Stock.Value,
            product.CreatedAt,
            product.Category.Id,
            product.Category.Name
        );
    }
    
    public static 
        ICollection<ProductResponse> ToResponse(
        ICollection<Product> products)
    {
        return products
            .Select(ToResponse)
            .ToList();
    }
}