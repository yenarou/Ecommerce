namespace ECommerce.Application.DTOs.Responses;

public record AuthResponse(string Token, Guid UserId, string Username);