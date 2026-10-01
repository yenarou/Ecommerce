using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetById(Guid productId);
    Task<ICollection<Product>> GetAll();
    Task Save(Product product);
    Task Delete(Guid productId);
}