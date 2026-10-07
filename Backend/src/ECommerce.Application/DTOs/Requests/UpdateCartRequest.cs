namespace ECommerce.Application.DTOs.Requests;

public record UpdateCartRequest(
    IReadOnlyCollection<UpdateCartItemRequest> Items
);