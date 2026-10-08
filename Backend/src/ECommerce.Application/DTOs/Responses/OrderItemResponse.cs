namespace ECommerce.Application.DTOs.Responses;

public record OrderItemResponse(
    Guid Id,
    OrderProductResponse Product,
    CustomizationResponse? Customization,
    int Quantity
);