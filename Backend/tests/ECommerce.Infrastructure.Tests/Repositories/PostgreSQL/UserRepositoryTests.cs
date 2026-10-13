using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;
using ECommerce.Infrastructure.Tests.Fixtures;
using FluentAssertions;

namespace ECommerce.Infrastructure.Tests.Repositories.PostgreSQL;

public class UserRepositoryTests(PostgreSqlFixture fixture)
    : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlUserRepository _repository =
        new(fixture.Context);

    [Fact]
    public async Task GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = User.CreateLocalUser(
            "John Doe",
            Email.Create("john@example.com"),
            "hashed-password");

        await _repository.Save(user);

        // Act
        var result = await _repository.GetById(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }
}