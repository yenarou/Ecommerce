using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.UseCases.Auth;
using ECommerce.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/auth/google")]
public class GoogleAuthController(
    AuthenticateWithGoogleUseCase authenticateWithGoogleUseCase,
    IOptions<GoogleOptions> googleOptions) : ControllerBase
{
    private readonly GoogleOptions _googleOptions = googleOptions.Value;

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        var authResponse =
            await authenticateWithGoogleUseCase.Execute(
                new AuthenticateWithGoogleRequest(
                    code,
                    _googleOptions.WebRedirectUri));

        var frontendCallback =
            $"{_googleOptions.ApiRedirectUri}" +
            $"?userId={Uri.EscapeDataString(authResponse.UserId.ToString())}" +
            $"&username={Uri.EscapeDataString(authResponse.Username)}" +
            $"#token={Uri.EscapeDataString(authResponse.Token)}";

        return Redirect(frontendCallback);
    }

    [HttpGet("url")]
    public IActionResult GetGoogleAuthUrl()
    {
        var url =
            $"https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={_googleOptions.ClientId}" +
            $"&redirect_uri={Uri.EscapeDataString(_googleOptions.WebRedirectUri)}" +
            $"&response_type=code" +
            $"&scope=openid%20email%20profile";

        return Ok(new { url });
    }
}