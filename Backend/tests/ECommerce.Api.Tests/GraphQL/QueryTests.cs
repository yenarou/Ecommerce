using ECommerce.Api.GraphQL;
using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Moq;

namespace ECommerce.Api.Tests.GraphQL;

public class QueryTests
{
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    [Fact]
    public async Task GetOrders_WhenUserHasOrders_ShouldReturnOrders()
    {
        // Arrange
        var user = User.CreateLocalUser(
            "papoi",
            Email.Create("papoi@gmail.com"),
            "Password123!"
        );

        var orders = new List<Order>
        {
            Order.Create(user, CreateAddress()),
            Order.Create(user, CreateAddress())
        };

        var query = CreateQuery(user, orders);

        // Act
        var result = await query.GetOrders(CreateUseCase());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        _orderRepositoryMock.Verify(
            x => x.GetByUserId(user.Id),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOrders_WhenUserHasNoOrders_ShouldReturnEmptyCollection()
    {
        // Arrange
        var user = User.CreateLocalUser(
            "papoi",
            Email.Create("papoi@gmail.com"),
            "Password123!"
        );

        var orders = new List<Order>();

        CreateQuery(user, orders);

        var useCase = CreateUseCase();
        var query = new Query();

        // Act
        var result = await query.GetOrders(useCase);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _orderRepositoryMock.Verify(
            x => x.GetByUserId(user.Id),
            Times.Once
        );
    }

    private Query CreateQuery(User user, ICollection<Order> orders)
    {
        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync(orders);

        return new Query();
    }

    private GetUserOrderHistoryUseCase CreateUseCase()
    {
        return new GetUserOrderHistoryUseCase(
            _currentUserMock.Object,
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object
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