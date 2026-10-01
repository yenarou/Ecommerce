using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Catalog;

public class GetProductDetails(IProductRepository productRepository)
{
    public async Task<ProductResponse> Execute(Guid productId)
    {
        var product = await productRepository.GetById(productId);

        return product is not null
            ? ProductMapper.ToResponse(product)
            : throw new ProductNotFoundException(productId.ToString());
    }
}