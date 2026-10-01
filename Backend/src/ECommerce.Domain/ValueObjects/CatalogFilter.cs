namespace ECommerce.Domain.ValueObjects;

public record CatalogFilter(
    string? Search,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice
);