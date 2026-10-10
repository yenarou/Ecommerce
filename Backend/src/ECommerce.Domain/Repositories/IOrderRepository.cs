using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetById(Guid orderId);
    Task<ICollection<Order>> GetByUserId(Guid userId);
    Task<ICollection<Order>> GetAll();
    Task Save(Order order);
    Task Delete(Guid orderId);
}