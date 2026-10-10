using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.DTOs.Requests.Admin;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    Guid CategoryId
);