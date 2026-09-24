using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Address
{
    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    public static Address Create(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new InvalidAddressException("Street");
        if (string.IsNullOrWhiteSpace(city)) throw new InvalidAddressException("City");
        if (string.IsNullOrWhiteSpace(state)) throw new InvalidAddressException("State");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new InvalidAddressException("Zip code");
        if (string.IsNullOrWhiteSpace(country)) throw new InvalidAddressException("Country");

        return new Address(street.Trim(), city.Trim(), state.Trim(), zipCode.Trim(), country.Trim());
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}