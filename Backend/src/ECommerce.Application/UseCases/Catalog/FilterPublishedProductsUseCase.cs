using ECommerce.Application.DTOs.Requests;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Catalog;

public class FilterPublishedProductsUseCase(
    IProductRepository productRepository)
{

    public async Task<ICollection<Product>> Execute(CatalogFilter catalogFilter, int page, int size)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0.");

        if (size < 1)
            throw new ArgumentException("Size must be greater than 0.");

        return await productRepository.FilterPublished(
            catalogFilter,
            page, size);
    }
}