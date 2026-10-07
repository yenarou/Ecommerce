using ECommerce.Domain.Models;

namespace ECommerce.Application.DTOs.Requests;

    public record CartItemRequest(
        Guid ProductId,
        CustomizationRequest Customization,
        int Quantity
    );