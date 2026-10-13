using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Checkout;

public class CreateOrderUseCaseTests
{
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICartRepository> _cartRepository = new();

    private CreateOrderUseCase CreateUseCase()
    {
        return new CreateOrderUseCase(
            _currentUser.Object,
            _orderRepository.Object,
            _productRepository.Object,
            _cartRepository.Object);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentNullException_WhenAddressIsNull()
    {
        // Arrange
        var useCase = CreateUseCase();

        var request = new CreateOrderRequest(
            null!);

        // Act
        var act = () => useCase.Execute(request);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("address");

        _currentUser.Verify(
            x => x.GetUserAsync(),
            Times.Never);

        _orderRepository.Verify(
            x => x.Save(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperationException_WhenCartDoesNotExist()
    {
        // Arrange
        var user = CreateUser();

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync((Cart?)null);

        var useCase = CreateUseCase();

        var request = CreateRequest();

        // Act
        var act = () => useCase.Execute(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cart not found.");

        _orderRepository.Verify(
            x => x.Save(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperationException_WhenCartIsEmpty()
    {
        // Arrange
        var user = CreateUser();
        var cart = CreateCart(user);

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        var useCase = CreateUseCase();

        var request = CreateRequest();

        // Act
        var act = () => useCase.Execute(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cart is empty.");

        _orderRepository.Verify(
            x => x.Save(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperationException_WhenProductStockIsInsufficient()
    {
        // Arrange
        var user = CreateUser();

        var product = CreateProduct(price:10,stock: 2);

        var cart = CreateCartWithItem(
            user,
            product,
            quantity: 3);

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        var useCase = CreateUseCase();

        var request = CreateRequest();

        // Act
        var act = () => useCase.Execute(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Product is out of stock.");

        _orderRepository.Verify(
            x => x.Save(It.IsAny<Order>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldCreateAndSaveOrder_WhenRequestIsValid()
    {
        // Arrange
        var user = CreateUser();

        var product = CreateProduct(price:10, stock: 10);

        var cart = CreateCartWithItem(
            user,
            product,
            quantity: 2);

        _currentUser
            .Setup(x => x.GetUserAsync())
            .ReturnsAsync(user);

        _cartRepository
            .Setup(x => x.GetActiveByUserId(user.Id))
            .ReturnsAsync(cart);

        Order? savedOrder = null;

        _orderRepository
            .Setup(x => x.Save(It.IsAny<Order>()))
            .Callback<Order>(order => savedOrder = order)
            .Returns(Task.CompletedTask);

        var useCase = CreateUseCase();

        var request = CreateRequest();

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.Should().NotBeNull();

        savedOrder.Should().NotBeNull();
        savedOrder!.Id.Should().NotBeEmpty();

        _orderRepository.Verify(
            x => x.Save(It.IsAny<Order>()),
            Times.Once);
    }

    private static CreateOrderRequest CreateRequest()
    {
        return new CreateOrderRequest(
            CreateAddress());
    }

    private static Address CreateAddress()
    {
        // Ajustar al constructor real de Address
        return Address.Create(
            "Av. Ejemplo 123",
            "Guadalajara",
            "Jalisco",
            "44100",
            "México");
    }

    private static User CreateUser()
    {
        // Ajustar al constructor/factory real de User
        return User.CreateLocalUser(
            "testuser",
            Email.Create("test@gmail.com"),
            "Password123!");
    }

    private static Cart CreateCart(User user)
    {
        return Cart.Create(user);
    }

    private static Product CreateProduct(decimal price, int stock)
    {
        var money = Money.Create(price, "MXN");
        var quantity = Quantity.Create(stock);

        var category = Category.Create(
            "test-category",
            "Test Category");

        var images = new List<Image>();

        return Product.Create(
            "Test Product",
            "Test product description",
            money,
            quantity,
            category,
            images);
    }

    private static Cart CreateCartWithItem(
        User user,
        Product product,
        int quantity)
    {
        
        
        var cart = Cart.Create(user);

        cart.AddCartItem(product, Quantity.Create(quantity));

        return cart;
    }
}