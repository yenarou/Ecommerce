using ECommerce.Domain.Models;

namespace ECommerce.Application.DTOs.Responses;

public record StockMovementResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    string Reason,
    string CreatedBy,
    DateTime CreatedAt)
{
    public static StockMovementResponse From(StockMovement m) => new(
        m.Id, m.ProductId, m.ProductName, m.Quantity, m.Reason, m.CreatedBy, m.CreatedAt);
}