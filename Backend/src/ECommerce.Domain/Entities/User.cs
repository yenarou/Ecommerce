using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class User
{
    private User()
    {
    }

    private User(Guid id, string name, Email email, string? passwordHash, string? googleId, AuthProvider authProvider)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        GoogleId = googleId;
        AuthProvider = authProvider;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public AuthProvider AuthProvider { get; }

    public static User CreateLocalUser(string name, Email email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be null or empty.", nameof(name));

        if (name.Length > 100) throw new ArgumentException("User name cannot exceed 100 characters.", nameof(name));

        if (email == null) throw new ArgumentNullException(nameof(email), "Email cannot be null.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

        return new User(Guid.NewGuid(), name, email, passwordHash, null, AuthProvider.Local);
    }

    public static User CreateGoogleUser(string name, Email email, string googleId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be null or empty.", nameof(name));

        if (name.Length > 100) throw new ArgumentException("User name cannot exceed 100 characters.", nameof(name));

        if (email == null) throw new ArgumentNullException(nameof(email), "Email cannot be null.");

        if (string.IsNullOrWhiteSpace(googleId))
            throw new ArgumentException("Google ID cannot be null or empty.", nameof(googleId));

        return new User(Guid.NewGuid(), name, email, null, googleId, AuthProvider.Google);
    }

    public void UpdateProfile(string name, Email email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be null or empty.", nameof(name));

        if (name.Length > 100) throw new ArgumentException("User name cannot exceed 100 characters.", nameof(name));

        if (email == null) throw new ArgumentNullException(nameof(email), "Email cannot be null.");

        Name = name;
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        if (AuthProvider != AuthProvider.Local)
            throw new InvalidOperationException("Cannot update password for non-local authentication provider.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

        PasswordHash = passwordHash;
    }
}