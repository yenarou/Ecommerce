namespace ECommerce.Application.DTOs.Requests;

public record AuthenticateWithGoogleRequest(string AuthorizationCode, string? RedirectUri);