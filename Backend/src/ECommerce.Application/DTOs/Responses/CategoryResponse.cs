namespace ECommerce.Application.DTOs.Responses;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description
);