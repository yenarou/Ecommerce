using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Tests;

public class ForeignKeyIdTests
{
    [Fact]
    public void CreatingCartAndOrderItems_PopulatesExplicitForeignKeyIds()
    {
        var user = User.CreateLocalUser(
            "test-user",
            Email.Create("test@example.com"),
            "Password123!");
        var category = Category.Create("test", "Test");
        var product = Product.Create(
            "Test product",
            "Test description",
            Money.Create(10m, "MXN"),
            Quantity.Create(5),
            category,
            []);
        var customization = Customization.Create("Test personalization", wrap: true);
        var cart = Cart.Create(user);

        cart.AddCartItem(product, Quantity.Create(2), customization);

        var cartItem = Assert.Single(cart.Items);
        var order = Order.Create(user, CreateAddress());
        order.AddOrderItems(cart);
        var orderItem = Assert.Single(order.Items);

        Assert.Equal(category.Id, product.CategoryId);
        Assert.Equal(user.Id, cart.UserId);
        Assert.Equal(cart.Id, cartItem.CartId);
        Assert.Equal(product.Id, cartItem.ProductId);
        Assert.Equal(customization.Id, cartItem.CustomizationId);
        Assert.Equal(user.Id, order.UserId);
        Assert.Equal(order.Id, orderItem.OrderId);
        Assert.Equal(product.Id, orderItem.ProductId);
        Assert.Equal(customization.Id, orderItem.CustomizationId);
    }

    [Fact]
    public void CreatingCategory_PreservesSlug()
    {
        var category = Category.Create("decoracion", "Decoración");

        Assert.Equal("decoracion", category.Slug);
    }

    private static Address CreateAddress()
    {
        return Address.Create(
            "Test Street 123",
            "Guadalajara",
            "Jalisco",
            "44100",
            "Mexico");
    }
}
