using ECommerce.Domain.Models;

namespace ECommerce.Application.DTOs.Requests;

public record UpdateCartItemRequest(
    Guid ProductId,
    Customization Customization,
    int Quantity
);