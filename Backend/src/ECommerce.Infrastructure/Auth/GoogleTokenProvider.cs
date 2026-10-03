using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tareino.Application.Interfaces;

namespace ECommerce.Infrastructure.Auth;

public class GoogleTokenProvider : IGoogleTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly GoogleOptions _options;

    public GoogleTokenProvider(
        HttpClient httpClient,
        IOptions<GoogleOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetValidAccessTokenAsync(
        string refreshToken)
    {
        var response = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            ]));

        response.EnsureSuccessStatusCode();

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new Exception("Invalid refresh response.");

        return tokenResponse.AccessToken;
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
    }
}