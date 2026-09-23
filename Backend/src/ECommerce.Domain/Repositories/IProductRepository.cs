using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetById(Guid productId);
#warning Usar paginado
    Task<List<Product>?> GetAll(); 
    Task Save(Product product);
    Task Delete(Guid productId);
}