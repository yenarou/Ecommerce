using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new DomainException("Street is required.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("City is required.");
        if (string.IsNullOrWhiteSpace(state)) throw new DomainException("State is required.");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new DomainException("Zip code is required.");
        if (string.IsNullOrWhiteSpace(country)) throw new DomainException("Country is required.");

        return new Address(street.Trim(), city.Trim(), state.Trim(), zipCode.Trim(), country.Trim());
    }

    public override string ToString() => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}