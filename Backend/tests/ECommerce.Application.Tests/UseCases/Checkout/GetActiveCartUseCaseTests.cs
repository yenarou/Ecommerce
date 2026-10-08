using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Checkout;

public class GetActiveCartUseCaseTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly GetActiveCartUseCase _useCase;

    public GetActiveCartUseCaseTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _cartRepositoryMock = new Mock<ICartRepository>();

        _useCase = new GetActiveCartUseCase(
            _currentUserMock.Object,
            _cartRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldReturnActiveCart_WhenUserHasActiveCart()
    {
        // Arrange
        var user = CreateUser();
        var cart = Cart.Create(user);

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepositoryMock
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cart.Id, result.Id);
        Assert.Equal(cart.Status.ToString(), result.Status);

        _currentUserMock.Verify(
            x => x.GetUserAsync(),
            Times.Once
        );

        _cartRepositoryMock.Verify(
            x => x.GetActiveByUserId(user.Id),
            Times.Once
        );
    }

    [Fact]
    public async Task Execute_ShouldCreateNewCart_WhenUserHasNoActiveCart()
    {
        // Arrange
        var user = CreateUser();

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepositoryMock
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);

        _currentUserMock.Verify(
            x => x.GetUserAsync(),
            Times.Once
        );

        _cartRepositoryMock.Verify(
            x => x.GetActiveByUserId(user.Id),
            Times.Once
        );
    }

    private static User CreateUser()
    {
        // Reemplazar con la forma real de crear User en tu dominio.
        return User.CreateLocalUser(
            "testuser",
            Email.Create("test@example.com"),
            "password123"
        );
    }
}