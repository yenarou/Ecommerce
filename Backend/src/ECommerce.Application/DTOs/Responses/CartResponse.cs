namespace ECommerce.Application.DTOs.Responses;

public record CartResponse(
    Guid Id,
    string Status,
    DateTime UpdatedAt,
    IReadOnlyCollection<CartItemResponse> Items);