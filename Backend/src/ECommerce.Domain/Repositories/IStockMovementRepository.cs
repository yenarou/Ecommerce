using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IStockMovementRepository
{
    Task Add(StockMovement movement);
    Task<ICollection<StockMovement>> GetRecent(Guid? productId, int take);
}