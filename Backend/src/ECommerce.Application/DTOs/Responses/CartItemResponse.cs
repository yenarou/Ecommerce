namespace ECommerce.Application.DTOs.Responses;

public record CartItemResponse(
    Guid Id,
    int Quantity,
    ProductResponse Product,
    CustomizationResponse? Customization
);