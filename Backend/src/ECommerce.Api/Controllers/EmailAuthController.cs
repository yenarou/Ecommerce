using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tareino.Application.DTO.Requests;
using Tareino.Application.UseCases.Auth;

namespace Tareino.API.Controllers.Auth;

[ApiController]
[Route("api/v1/auth")]
public class EmailAuthController(RegisterWithEmailUseCase registerUseCase, LoginWithEmailUseCase loginUseCase)
    : ApiControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterWithEmailRequest request)
    {
        var authResponse = await registerUseCase.Execute(request);
        return StatusCode(201, authResponse);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginEmailRequest request)
    {
        var authResponse = await loginUseCase.Execute(request);
        return Ok(authResponse);
    }
}