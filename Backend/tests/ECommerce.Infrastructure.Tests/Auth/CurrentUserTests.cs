using System.Security.Claims;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Infrastructure.Tests.Auth;

public class CurrentUserTests
{
    [Fact]
    public async Task GetUserAsync_ShouldResolveJwtSubjectClaim()
    {
        var user = User.CreateLocalUser(
            "Test User",
            Email.Create("test@example.com"),
            "password-hash");
        var repository = new StubUserRepository(user);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", user.Id.ToString())],
                "Bearer"))
        };
        var currentUser = new CurrentUser(
            new HttpContextAccessor { HttpContext = httpContext },
            repository);

        var result = await currentUser.GetUserAsync();

        result.Should().BeSameAs(user);
    }

    private sealed class StubUserRepository(User user) : IUserRepository
    {
        public Task<User?> GetById(Guid userId) =>
            Task.FromResult<User?>(userId == user.Id ? user : null);

        public Task<User?> GetByEmail(string email) =>
            throw new NotSupportedException();

        public Task<User?> GetByGoogleId(string googleId) =>
            throw new NotSupportedException();

        public Task Save(User user) =>
            throw new NotSupportedException();

        public Task Delete(Guid userId) =>
            throw new NotSupportedException();
    }
}
