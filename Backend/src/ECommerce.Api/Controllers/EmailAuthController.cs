using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class EmailAuthController(RegisterWithEmailUseCase registerUseCase, LoginWithEmailUseCase loginUseCase, LoginAdminUseCase adminLoginUseCase)
    : ControllerBase
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

    //admin
    [HttpPost("admin/login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> AdminLogin(LoginEmailRequest request)
    {
        var authResponse = await adminLoginUseCase.Execute(request.Email, request.Password);
        return Ok(authResponse);
    }
}