namespace Tareino.Application.Interfaces;

public interface IGoogleTokenProvider
{
    Task<string> GetValidAccessTokenAsync(string refreshToken);
}