using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetById(Guid cartId);
    Task<ICollection<Cart>?> GetByUserId(Guid userId);
    Task<Cart?> GetActiveByUserId(Guid userId);
    Task Save(Cart cart);
    Task Delete(Guid cartId);
}