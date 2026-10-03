namespace ECommerce.Domain.ValueObjects;

public record CatalogFilter(
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
);