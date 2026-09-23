using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetById(Guid orderId);
    Task<List<Order>?> GetByUserId(Guid userId);
    Task Save(Order order);
    Task Delete(Guid orderId);
}