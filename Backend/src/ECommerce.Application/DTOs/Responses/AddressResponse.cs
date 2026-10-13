namespace ECommerce.Application.DTOs.Responses;

public record AddressResponse(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
);