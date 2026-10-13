using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class CustomizationMapper
{
    public static CustomizationResponse ToResponse(this Customization customization)
    {
        return new CustomizationResponse(
            customization.Description,
            customization.IsWrap
        );
    }
}