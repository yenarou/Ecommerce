using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Tests;

public class ProductStockDomainEventTests
{
    [Fact]
    public void UpdateStock_WhenStockChanges_AddsDomainEventWithPreviousAndNewStock()
    {
        var product = CreateProduct(5);

        product.UpdateStock(Quantity.Create(8));

        var domainEvent = Assert.IsType<StockChangedDomainEvent>(
            Assert.Single(product.DomainEvents));
        Assert.Equal(product.Id, domainEvent.ProductId);
        Assert.Equal(5, domainEvent.PreviousStock.Value);
        Assert.Equal(8, domainEvent.NewStock.Value);
        Assert.Equal(8, product.Stock.Value);
    }

    [Fact]
    public void UpdateStock_WhenStockIsUnchanged_DoesNotAddDomainEvent()
    {
        var product = CreateProduct(5);

        product.UpdateStock(Quantity.Create(5));

        Assert.Empty(product.DomainEvents);
    }

    [Fact]
    public void UpdateStock_WhenCalledMoreThanOnce_AddsAnEventForEachChange()
    {
        var product = CreateProduct(5);

        product.UpdateStock(Quantity.Create(8));
        product.UpdateStock(Quantity.Create(3));

        var events = product.DomainEvents
            .Cast<StockChangedDomainEvent>()
            .ToArray();

        Assert.Collection(
            events,
            first =>
            {
                Assert.Equal(5, first.PreviousStock.Value);
                Assert.Equal(8, first.NewStock.Value);
            },
            second =>
            {
                Assert.Equal(8, second.PreviousStock.Value);
                Assert.Equal(3, second.NewStock.Value);
            });
    }

    [Fact]
    public void UpdateStock_WhenStockIsNull_ThrowsAndDoesNotAddDomainEvent()
    {
        var product = CreateProduct(5);

        Assert.Throws<ArgumentNullException>(() => product.UpdateStock(null!));

        Assert.Empty(product.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_RemovesPendingEvents()
    {
        var product = CreateProduct(5);
        product.UpdateStock(Quantity.Create(8));

        product.ClearDomainEvents();

        Assert.Empty(product.DomainEvents);
    }

    private static Product CreateProduct(int stock)
    {
        return Product.Create(
            "Test product",
            "Test description",
            Money.Create(10m, "MXN"),
            Quantity.Create(stock),
            Category.Create("test-category", "Test category"),
            []);
    }
}
