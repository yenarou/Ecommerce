namespace ECommerce.Application.DTOs.Responses;

public record PaginatedCollectionResponse<T>(
    ICollection<T> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages);