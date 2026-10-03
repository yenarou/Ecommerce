using ECommerce.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class MeController(IUserRepository userRepository) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var sub = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId)) return Unauthorized();

        var user = await userRepository.GetById(userId);
        if (user is null) return Unauthorized();

        return Ok(new { userId = user.Id, username = user.Name, email = user.Email.Value });
    }
}