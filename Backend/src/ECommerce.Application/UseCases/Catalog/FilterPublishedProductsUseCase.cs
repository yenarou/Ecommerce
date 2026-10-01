using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Catalog;

public class FilterPublishedProductsUseCase(
    IProductRepository productRepository)
{

    public async Task<PaginatedCollectionResponse<ProductResponse>> Execute(CatalogFilter catalogFilter, int page, int size)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0.");

        if (size < 1)
            throw new ArgumentException("Size must be greater than 0.");

        var products = await productRepository.FilterPublished(
            catalogFilter,
            page, size);
        
        return  new PaginatedCollectionResponse<ProductResponse>(
            Page: 2,
            Size: 20,
            Total: 157,
            Items: ProductMapper.ToResponse(products));
    }
}