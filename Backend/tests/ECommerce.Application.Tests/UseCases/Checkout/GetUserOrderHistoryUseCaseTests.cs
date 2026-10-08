using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Checkout;

public class GetUserOrderHistoryUseCaseTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetUserOrderHistoryUseCase _useCase;

    public GetUserOrderHistoryUseCaseTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _orderRepositoryMock = new Mock<IOrderRepository>();

        _useCase = new GetUserOrderHistoryUseCase(
            _currentUserMock.Object,
            _orderRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldReturnUserOrders()
    {
        // Arrange
        var user = CreateUser();
        var orders = new List<Order>
        {
            CreateOrder(user),
            CreateOrder(user)
        };

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync(orders);

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orders.Count, result.Count);
    }

    [Fact]
    public async Task Execute_ShouldGetOrdersUsingCurrentUserId()
    {
        // Arrange
        var user = CreateUser();
        var orders = new List<Order>();

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync(orders);

        // Act
        await _useCase.Execute();

        // Assert
        _orderRepositoryMock.Verify(
            x => x.GetByUserId(user.Id),
            Times.Once
        );
    }

    [Fact]
    public async Task Execute_ShouldGetCurrentUser()
    {
        // Arrange
        var user = CreateUser();

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync([]);

        // Act
        await _useCase.Execute();

        // Assert
        _currentUserMock.Verify(
            x => x.GetUserAsync(),
            Times.Once
        );
    }

    [Fact]
    public async Task Execute_WhenUserHasNoOrders_ShouldReturnEmptyCollection()
    {
        // Arrange
        var user = CreateUser();

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync([]);

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private static User CreateUser()
    {
        return User.CreateLocalUser(
            "papoii",
            Email.Create("papoi@gmail.com"),
            "Password123!"
        );
    }

    private static Order CreateOrder(User user)
    {
        return Order.Create(
            user,
            CreateAddress()
        );
    }

    private static Address CreateAddress()
    {
        return Address.Create(
            "Test Street 123",
            "Guadalajara",
            "Jalisco",
            "44100",
            "Mexico"
        );
    }
}