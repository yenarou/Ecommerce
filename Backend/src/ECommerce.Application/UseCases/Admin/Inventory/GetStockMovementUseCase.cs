using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Inventory;

public class GetStockMovementsUseCase(IStockMovementRepository repository)
{
    public async Task<ICollection<StockMovementResponse>> Execute(Guid? productId, int take)
    {
        var movements = await repository.GetRecent(productId, Math.Clamp(take, 1, 500));
        return movements.Select(StockMovementResponse.From).ToList();
    }
}