using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IGoogleAuthService
{
    Task<OAuthTokensDto> ExchangeAuthorizationCodeAsync(string authorizationCode, string? redirectUri = null);
    Task<GoogleUserInfo> GetUserInfoAsync(string accessToken);
    
}