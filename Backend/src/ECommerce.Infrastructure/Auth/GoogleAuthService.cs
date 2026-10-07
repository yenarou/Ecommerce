using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Auth;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleOptions _options;

    public GoogleAuthService(
        HttpClient httpClient,
        IOptions<GoogleOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<OAuthTokensDto> ExchangeAuthorizationCodeAsync(
        string authorizationCode,
        string? redirectUri = null)
    {
        var response = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri ?? _options.WebRedirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            ]));

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(body);
        }

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new Exception("Invalid token response.");

        return new OAuthTokensDto(
            tokenResponse.AccessToken,
            tokenResponse.RefreshToken ?? string.Empty,
            tokenResponse.ExpiresIn);
    }

    public async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://openidconnect.googleapis.com/v1/userinfo");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var user =
            await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>()
            ?? throw new Exception("Invalid user info response.");

        return new GoogleUserInfo(
            user.Id,
            user.Email,
            user.Name);
    }


    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("refresh_token")] public string? RefreshToken { get; init; }

        [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; }
    }

    private sealed class GoogleUserInfoResponse
    {
        [JsonPropertyName("sub")] public string Id { get; init; } = string.Empty;

        [JsonPropertyName("email")] public string Email { get; init; } = string.Empty;

        [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    }
}