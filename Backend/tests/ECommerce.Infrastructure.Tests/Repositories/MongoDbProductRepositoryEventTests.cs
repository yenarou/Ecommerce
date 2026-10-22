using ECommerce.Application.Events;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace ECommerce.Infrastructure.Tests.Repositories;

public class MongoDbProductRepositoryEventTests
{
    [Fact]
    public async Task Save_WhenProductHasDomainEvents_DispatchesThemAfterSavingAndClearsThem()
    {
        var product = CreateProduct();
        product.UpdateStock(Quantity.Create(8));
        var (repository, productCollection, eventDispatcher) =
            CreateRepository(product.Category);
        eventDispatcher
            .Setup(dispatcher => dispatcher.DispatchAsync(
                It.IsAny<IEnumerable<IDomainEvent>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<IDomainEvent>, CancellationToken>((_, _) =>
                productCollection.Verify(collection => collection.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<Product>>(),
                        product,
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once))
            .Returns(Task.CompletedTask);

        await repository.Save(product);

        productCollection.Verify(collection => collection.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                product,
                It.Is<ReplaceOptions>(options => options.IsUpsert),
                It.IsAny<CancellationToken>()),
            Times.Once);
        eventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(
                It.Is<IEnumerable<IDomainEvent>>(events =>
                    events.OfType<StockChangedDomainEvent>().Single().NewStock.Value == 8),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Empty(product.DomainEvents);
    }

    [Fact]
    public async Task Save_WhenEventDispatchFails_KeepsDomainEventsForRetry()
    {
        var product = CreateProduct();
        product.UpdateStock(Quantity.Create(8));
        var (repository, _, eventDispatcher) = CreateRepository(product.Category);
        eventDispatcher
            .Setup(dispatcher => dispatcher.DispatchAsync(
                It.IsAny<IEnumerable<IDomainEvent>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Dispatch failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.Save(product));

        Assert.Single(product.DomainEvents);
    }

    [Fact]
    public async Task Save_WhenProductHasNoDomainEvents_DoesNotDispatch()
    {
        var product = CreateProduct();
        var (repository, _, eventDispatcher) = CreateRepository(product.Category);

        await repository.Save(product);

        eventDispatcher.Verify(
            dispatcher => dispatcher.DispatchAsync(
                It.IsAny<IEnumerable<IDomainEvent>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static (
        MongoDbProductRepository Repository,
        Mock<IMongoCollection<Product>> ProductCollection,
        Mock<IDomainEventDispatcher> EventDispatcher)
        CreateRepository(Category category)
    {
        var database = new Mock<IMongoDatabase>();
        var productCollection = new Mock<IMongoCollection<Product>>();
        var categoryCollection = new Mock<IMongoCollection<Category>>(MockBehavior.Strict);
        var eventDispatcher = new Mock<IDomainEventDispatcher>();
        var categoryCursor = new Mock<IAsyncCursor<BsonDocument>>();

        categoryCursor
            .SetupGet(cursor => cursor.Current)
            .Returns([new BsonDocument()]);
        categoryCursor
            .Setup(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        categoryCollection
            .Setup(collection => collection.FindAsync<BsonDocument>(
                It.IsAny<FilterDefinition<Category>>(),
                It.IsAny<FindOptions<Category, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryCursor.Object);
        categoryCollection.SetReturnsDefault(
            Task.FromResult<IAsyncCursor<BsonDocument>>(categoryCursor.Object));
        productCollection
            .Setup(collection => collection.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReplaceOneResult)null!);
        database
            .Setup(db => db.GetCollection<Product>(
                "products",
                It.IsAny<MongoCollectionSettings>()))
            .Returns(productCollection.Object);
        database
            .Setup(db => db.GetCollection<Category>(
                "categories",
                It.IsAny<MongoCollectionSettings>()))
            .Returns(categoryCollection.Object);

        return (
            new MongoDbProductRepository(database.Object, eventDispatcher.Object),
            productCollection,
            eventDispatcher);
    }

    private static Product CreateProduct()
    {
        return Product.Create(
            "Test product",
            "Test description",
            Money.Create(10m, "MXN"),
            Quantity.Create(5),
            Category.Create("test-category", "Test category"),
            []);
    }
}
