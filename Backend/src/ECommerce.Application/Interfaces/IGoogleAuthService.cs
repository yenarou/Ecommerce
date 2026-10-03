using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IGoogleAuthService
{
    Task<OAuthTokensDto> ExchangeAuthorizationCodeAsync(string authorizationCode, string? redirectUri = null);
    Task<OAuthTokensDto> ExchangeAuthorizationClassroomCodeAsync(string authorizationCode);
    Task<GoogleUserInfo> GetUserInfoAsync(string accessToken);
    
}