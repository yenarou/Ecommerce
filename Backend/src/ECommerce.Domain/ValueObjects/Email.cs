using System.Text.RegularExpressions;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(value, "Email cannot be empty.");

        if (!Pattern.IsMatch(value))
            throw new InvalidEmailException(value);

        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}