using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.DTOs.Responses;

public record CreateOrderResponse(
    Guid Id,
    decimal Total
);