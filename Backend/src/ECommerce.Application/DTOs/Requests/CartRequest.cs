namespace ECommerce.Application.DTOs.Requests;

public record CartRequest(
    IReadOnlyCollection<CartItemRequest> Items
);