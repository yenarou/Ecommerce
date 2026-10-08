using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Factories;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Checkout;

public class UpdateCartUseCaseTests
{
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly Mock<ICartFactory> _cartFactory = new();

    private UpdateCartUseCase CreateUseCase()
    {
        return new UpdateCartUseCase(
            _currentUser.Object,
            _cartRepository.Object,
            _cartFactory.Object);
    }

    [Fact]
    public async Task Execute_ShouldUpdateExistingCart()
    {
        // Arrange
        var user = CreateUser();

        var cart = CreateCart(user);
        var newCart = CreateCart(user);

        var request = CreateRequest();

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        _cartFactory
            .Setup(x => x.Create(user, request))
            .ReturnsAsync(newCart);

        var useCase = CreateUseCase();

        // Act
        await useCase.Execute(request);

        // Assert
        _currentUser.Verify(
            x => x.GetUserAsync(),
            Times.Once);

        _cartRepository.Verify(
            x => x.GetActiveByUserId(user.Id),
            Times.Once);

        _cartFactory.Verify(
            x => x.Create(user, request),
            Times.Once);

        _cartRepository.Verify(
            x => x.Save(cart),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldCreateNewCart_WhenActiveCartDoesNotExist()
    {
        // Arrange
        var user = CreateUser();

        var newCart = CreateCart(user);

        var request = CreateRequest();

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync((Cart?)null);

        _cartFactory
            .Setup(x => x.Create(user, request))
            .ReturnsAsync(newCart);

        var useCase = CreateUseCase();

        // Act
        await useCase.Execute(request);

        // Assert
        _currentUser.Verify(
            x => x.GetUserAsync(),
            Times.Once);

        _cartRepository.Verify(
            x => x.GetActiveByUserId(user.Id),
            Times.Once);

        _cartFactory.Verify(
            x => x.Create(user, request),
            Times.Once);

        _cartRepository.Verify(
            x => x.Save(It.IsAny<Cart>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldNotSaveCart_WhenFactoryThrowsException()
    {
        // Arrange
        var user = CreateUser();

        var cart = CreateCart(user);

        var request = CreateRequest();

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        _cartFactory
            .Setup(x => x.Create(user, request))
            .ThrowsAsync(
                new KeyNotFoundException("Product not found."));

        var useCase = CreateUseCase();

        // Act
        var act = () => useCase.Execute(request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>();

        _cartRepository.Verify(
            x => x.Save(It.IsAny<Cart>()),
            Times.Never);
    }

    private static CartRequest CreateRequest()
    {
        return new CartRequest(
            new List<CartItemRequest>
            {
                new(
                    Guid.NewGuid(),
                    new CustomizationRequest(
                        "Test personalization",
                        false),
                    2)
            });
    }

    private static User CreateUser()
    {
        return User.CreateLocalUser(
            "testuser",
            Email.Create("test@gmail.com"),
            "Password123!");
    }

    private static Cart CreateCart(User user)
    {
        return Cart.Create(user);
    }
}