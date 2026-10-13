using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Catalog;

public class GetPublishedProductsCase(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<PaginatedCollectionResponse<ProductResponse>> Execute(
        int page,
        int size,
        string? categorySlug = null)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(
                nameof(page),
                page,
                "Page must be greater than 0.");

        if (size < 1)
            throw new ArgumentOutOfRangeException(
                nameof(size),
                size,
                "Size must be greater than 0.");

        var category = categorySlug is null
            ? null
            : await categoryRepository.GetBySlug(categorySlug);
        var catalogFilter = new CatalogFilter(CategoryId: category?.Id);

        var paginatedResult = await productRepository.FilterPublished(
            catalogFilter,
            page,
            size);

        var totalPages = (int)Math.Ceiling(
            (double)paginatedResult.Total / size);

        return new PaginatedCollectionResponse<ProductResponse>(
            ProductMapper.ToResponse(paginatedResult.Items),
            paginatedResult.Total,
            page,
            size,
            totalPages);
    }
}