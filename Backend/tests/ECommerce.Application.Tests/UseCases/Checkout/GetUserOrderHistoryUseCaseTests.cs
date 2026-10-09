using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Checkout;

public class GetUserOrderHistoryUseCaseTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetUserOrderHistoryUseCase _useCase;

    public GetUserOrderHistoryUseCaseTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _useCase = new GetUserOrderHistoryUseCase(
            _currentUserMock.Object,
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object
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
    public async Task Execute_ShouldRestoreProductsFromStoredProductIds()
    {
        var user = CreateUser();
        var category = Category.Create("test", "Test");
        var product = Product.Create(
            "Test product",
            "Test description",
            Money.Create(10m, "MXN"),
            Quantity.Create(5),
            category,
            []);
        var cart = Cart.Create(user);
        cart.AddCartItem(product, Quantity.Create(2));
        var order = CreateOrder(user);
        order.AddOrderItems(cart);

        _currentUserMock
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);
        _orderRepositoryMock
            .Setup(x => x.GetByUserId(user.Id))
            .ReturnsAsync([order]);
        _productRepositoryMock
            .Setup(x => x.GetById(product.Id))
            .ReturnsAsync(product);

        var result = await _useCase.Execute();

        var responseItem = Assert.Single(Assert.Single(result).Items);
        Assert.Equal(product.Id, responseItem.ProductId);
        Assert.Equal(product.Id, responseItem.Product.Id);
        _productRepositoryMock.Verify(x => x.GetById(product.Id), Times.Once);
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