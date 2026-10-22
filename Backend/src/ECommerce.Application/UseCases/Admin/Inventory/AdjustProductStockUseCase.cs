using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Application.Services;
using ECommerce.Domain.Models;

namespace ECommerce.Application.UseCases.Admin.Inventory;

public class AdjustProductStockUseCase(InventoryService inventory)
{
    public async Task<ProductResponse> Execute(Guid productId, int quantity, string createdBy)
    {
        var product = await inventory.Apply(
            productId, quantity, StockMovementReason.Adjustment, null, createdBy);

        return ProductMapper.ToResponse(product);
    }
}