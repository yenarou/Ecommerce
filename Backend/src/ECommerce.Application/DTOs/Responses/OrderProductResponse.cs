namespace ECommerce.Application.DTOs.Responses;

public record OrderProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    string Currency
);