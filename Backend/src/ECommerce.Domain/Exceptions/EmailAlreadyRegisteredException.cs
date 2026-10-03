namespace ECommerce.Domain.Exceptions;

public class EmailAlreadyRegisteredException(string email) : DomainException($"Email {email} already registered")
{
    public string Email { get; } = email;
}