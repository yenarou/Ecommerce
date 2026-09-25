namespace ECommerce.Domain.Exceptions;

public class InvalidCredentialsException(string email) : DomainException($"Invalid credentials when login to {email}")
{
    public string Email { get; } = email;
}