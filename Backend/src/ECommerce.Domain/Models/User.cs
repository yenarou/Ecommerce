using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Models;

public class User
{
    public Guid Id { get; private set;}
    public string Name { get; private set;}
    public Email Email { get; private set;}
    public string? PasswordHash { get; private set;}
    public string? GoogleId { get; private set;}
    public AuthProvider AuthProvider {get; private set;}
    
    private User() { }

    private User(Guid id, string name, Email email, string? passwordHash, string? googleId, AuthProvider authProvider)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        GoogleId = googleId;
        AuthProvider = authProvider;
    }

    public static User CreateLocalUser(string name, Email email, string passwordHash)
    {
        return new User(Guid.NewGuid(), name, email, passwordHash, null, AuthProvider.Local);
    }

    public static User CreateGoogleUser(string name, Email email, string googleId)
    {
        return new User(Guid.NewGuid(), name, email, null, googleId, AuthProvider.Google);
    }

    public void UpdateProfile(string name, Email email)
    {
        Name = name;
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

}