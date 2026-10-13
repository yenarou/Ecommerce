using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.DTOs.Responses;

public record OrderResponse(
    Guid Id,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    AddressResponse Address,
    OrderStatus Status,
    IReadOnlyCollection<OrderItemResponse> Items
);