using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Persistence.MongoDB.Configuration;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ECommerce.Infrastructure.Tests;

public class ForeignKeyMappingTests
{
    [Fact]
    public void RelationshipForeignKeys_UseDomainIdProperties()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=ecommerce_test;Username=test;Password=test")
            .Options;

        using var context = new ApplicationDbContext(options);
        var model = context.Model;

        AssertForeignKey<Cart>(model, nameof(Cart.UserId));
        AssertForeignKey<CartItem>(model, nameof(CartItem.CartId));
        AssertForeignKey<CartItem>(model, nameof(CartItem.CustomizationId));
        AssertForeignKey<Order>(model, nameof(Order.UserId));
        AssertForeignKey<OrderItem>(model, nameof(OrderItem.OrderId));
        AssertForeignKey<OrderItem>(model, nameof(OrderItem.CustomizationId));
        AssertNotShadow<CartItem>(model, nameof(CartItem.ProductId));
        AssertNotShadow<OrderItem>(model, nameof(OrderItem.ProductId));
    }

    [Fact]
    public void ProductMongoDocument_StoresCategoryIdInsteadOfEmbeddedCategory()
    {
        MongoDbConfiguration.Configure();
        var category = Category.Create("test", "Test");
        var product = Product.Create(
            "Test product",
            "Test description",
            Money.Create(10m, "MXN"),
            Quantity.Create(5),
            category,
            []);

        var document = product.ToBsonDocument();
        var restored = BsonSerializer.Deserialize<Product>(document);

        Assert.Equal(category.Id, document[nameof(Product.CategoryId)].AsGuid);
        Assert.False(document.Contains(nameof(Product.Category)));
        restored.RestoreCategory(category);
        Assert.Equal(category.Id, restored.Category.Id);
    }

    private static void AssertForeignKey<TEntity>(IModel model, string propertyName)
    {
        var entityType = model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entityType);

        var property = entityType.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.False(property.IsShadowProperty());
        Assert.Contains(
            entityType.GetForeignKeys(),
            foreignKey => foreignKey.Properties.Contains(property));
    }

    private static void AssertNotShadow<TEntity>(IModel model, string propertyName)
    {
        var entityType = model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entityType);

        var property = entityType.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.False(property.IsShadowProperty());
    }
}
