using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetById(Guid productId);
    Task<PaginatedResult<Product>> FilterPublished(CatalogFilter catalogFilter, int page, int size);
    Task<ICollection<Product>> FilterPublished(CatalogFilter catalogFilter);
    Task Save(Product product);
    Task Delete(Guid productId);
}