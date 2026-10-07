namespace ECommerce.Domain.Repositories;

public record PaginatedResult<T>(
    List<T> Items,
    int Total);