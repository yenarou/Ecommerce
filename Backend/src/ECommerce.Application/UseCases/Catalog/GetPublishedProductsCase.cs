using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Catalog;

public class GetPublishedProductsCase(
    IProductRepository productRepository)
{
    public async Task<PaginatedCollectionResponse<ProductResponse>> Execute(    int page,
        int size)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page must be greater than 0.");

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be greater than 0.");
        
        var products = await productRepository.GetPublishedPage(page, size);
        
        return  new PaginatedCollectionResponse<ProductResponse>(
            Page: 2,
            Size: 20,
            Total: 157,
            Items: ProductMapper.ToResponse(products));
        
    }
}
