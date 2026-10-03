namespace ECommerce.Domain.Exceptions;

public class UserNotFoundException(string userId) : DomainException($"User {userId} was not found")
{
    public string UserId { get; } = userId;
}