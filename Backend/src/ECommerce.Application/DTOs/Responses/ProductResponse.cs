namespace ECommerce.Application.DTOs.Responses;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    DateTime CreatedAt,
    Guid CategoryId,
    string CategoryName,
    List<ImageResponse> Images
);