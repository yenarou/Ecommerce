namespace ECommerce.Application.DTOs.Responses;

public record PaginatedCollectionResponse<T>(
    int Page,
    int Size,
    int Total,
    ICollection<T> Items);