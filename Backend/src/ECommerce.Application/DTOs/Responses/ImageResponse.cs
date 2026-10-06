namespace ECommerce.Application.DTOs.Responses;

public record ImageResponse(
    Guid Id,
    string Url,
    string Alt
);