namespace ECommerce.Domain.Exceptions;

public class InvalidEmailException(string email) : DomainException($"Invalid email {email}")
{
    public string Email { get; } = email;
}