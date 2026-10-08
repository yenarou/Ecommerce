using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Mappers;

public static class AddressMapper
{
    public static AddressResponse ToResponse(this Address address)
    {
        return new AddressResponse(
            address.Street,
            address.City,
            address.State,
            address.ZipCode,
            address.Country
        );
    }
}
