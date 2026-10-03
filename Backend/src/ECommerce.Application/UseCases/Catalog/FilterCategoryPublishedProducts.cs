using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Catalog;

public class FilterCategoryPublishedProducts(
    IProductRepository productRepository)
{
    public async Task<ICollection<ProductResponse>> Execute(
        Guid? categoryId)
    {
        
        var catalogFilter = new CatalogFilter(
            CategoryId: categoryId);

        var products = await productRepository.FilterPublished(
            catalogFilter);

        return ProductMapper.ToResponse(products);

    }
}