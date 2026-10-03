using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tareino.Application.DTO.Requests;
using Tareino.Application.UseCases.Auth;
using Tareino.Infrastructure.Auth;

namespace Tareino.API.Controllers.Auth;

[ApiController]
[Route("api/v1/auth/google")]
public class GoogleAuthController(
    AuthenticateWithGoogleUseCase authenticateWithGoogleUseCase,
    IOptions<GoogleOptions> googleOptions) : ApiControllerBase
{
    private readonly GoogleOptions _googleOptions = googleOptions.Value;

    [HttpGet("web-callback")]
    public async Task<IActionResult> Callback(string code)
    {
        var authResponse =
            await authenticateWithGoogleUseCase.Execute(
                new AuthenticateWithGoogleRequest(code, _googleOptions.WebRedirectUri));

        return Ok(authResponse);
    }
    
    [HttpGet("mobile-callback")]
    public async Task<IActionResult> MobileCallback(string code)
    {
        Console.WriteLine("Entró al callback");

        var authResponse =
            await authenticateWithGoogleUseCase.Execute(
                new AuthenticateWithGoogleRequest(
                    code,
                    _googleOptions.MobileRedirectUri));

        Console.WriteLine("JWT creado");

        var url =
            $"tareino://auth/callback?token={Uri.EscapeDataString(authResponse.Token)}&username={Uri.EscapeDataString(authResponse.Username)}";

        Console.WriteLine(url);

        return Redirect(url);
    }

    [HttpGet("url")]
    public IActionResult GetGoogleAuthUrl([FromQuery] string platform)
    {
        var redirectUri = platform switch
        {
            "mobile" => _googleOptions.MobileRedirectUri,
            "web" => _googleOptions.WebRedirectUri,
            _ => throw new ArgumentException("Invalid platform")
        };

        var url =
            $"https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={_googleOptions.ClientId}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&response_type=code" +
            $"&scope=openid%20email%20profile";

        return Ok(new { url });
    }
}