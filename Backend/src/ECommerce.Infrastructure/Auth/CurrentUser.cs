using System.Security.Claims;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Infrastructure.Auth;

public class CurrentUser(
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : ICurrentUser
{
    private User? _user;

    public async Task<User> GetUserAsync()
    {
        if (_user is not null)
            return _user;

        var userIdClaim = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID.");

        var user = await userRepository.GetById(userId);

        if (user is null)
            throw new UnauthorizedAccessException("User does not exist.");

        _user = user;

        return _user;
    }
}