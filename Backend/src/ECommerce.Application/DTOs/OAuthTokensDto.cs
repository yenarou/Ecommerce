namespace ECommerce.Application.DTOs;

public record OAuthTokensDto(string AccessToken, string RefreshToken, int ExpiresIn);