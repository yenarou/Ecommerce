namespace ECommerce.Application.DTOs.Responses;

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    OrderProductResponse Product,
    CustomizationResponse? Customization,
    int Quantity
);