using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoDbProductRepository(IMongoDatabase database) : IProductRepository
{
    private readonly IMongoCollection<Product> _products =
        database.GetCollection<Product>("products");
    private readonly IMongoCollection<Category> _categories =
        database.GetCollection<Category>("categories");

    public async Task<Product?> GetById(Guid productId)
    {
        var product = await _products
            .Find(product => product.Id == productId)
            .FirstOrDefaultAsync();

        if (product is not null)
            await RestoreCategoriesAsync([product]);

        return product;
    }

    public async Task<PaginatedResult<Product>> FilterPublished(
        CatalogFilter catalogFilter,
        int page,
        int size)
    {
        if (catalogFilter is null)
            throw new ArgumentNullException(nameof(catalogFilter));

        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));

        var filter = Builders<Product>.Filter.Eq(
            product => product.IsPublished,
            true);

        if (catalogFilter.CategoryId.HasValue)
            filter &= Builders<Product>.Filter.Eq(
                product => product.CategoryId,
                catalogFilter.CategoryId.Value);

        var total = await _products.CountDocumentsAsync(filter);

        var items = await _products
            .Find(filter)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();

        await RestoreCategoriesAsync(items);

        return new PaginatedResult<Product>(
            items,
            (int)total);
    }

    public async Task<ICollection<Product>> FilterPublished(
        CatalogFilter catalogFilter)
    {
        if (catalogFilter == null)
            throw new ArgumentNullException(nameof(catalogFilter));

        var filter = Builders<Product>.Filter.Eq(
            product => product.IsPublished,
            true);

        if (catalogFilter.CategoryId.HasValue)
            filter &= Builders<Product>.Filter.Eq(
                product => product.CategoryId,
                catalogFilter.CategoryId.Value);

        var products = await _products
            .Find(filter)
            .ToListAsync();

        await RestoreCategoriesAsync(products);
        return products;
    }

    public async Task Save(Product product)
    {
        var categoryExists = await _categories
            .Find(category => category.Id == product.CategoryId)
            .AnyAsync();

        if (!categoryExists)
            throw new InvalidOperationException(
                $"Cannot save product {product.Id}: category {product.CategoryId} does not exist.");

        var filter = Builders<Product>.Filter.Eq(
            existing => existing.Id,
            product.Id);

        await _products.ReplaceOneAsync(
            filter,
            product,
            new ReplaceOptions
            {
                IsUpsert = true
            });
    }

    public async Task Delete(Guid productId)
    {
        var filter = Builders<Product>.Filter.Eq(
            product => product.Id,
            productId);

        await _products.DeleteOneAsync(filter);
    }

    public async Task<ICollection<Product>> GetAll()
    {
        var products = await _products
            .Find(_ => true)
            .ToListAsync();

        await RestoreCategoriesAsync(products);
        return products;
    }

    public async Task<ICollection<Product>> GetAllPublished()
    {
        var products = await _products
            .Find(product => product.IsPublished)
            .ToListAsync();

        await RestoreCategoriesAsync(products);
        return products;
    }

    public async Task<ICollection<Product>> GetPublishedPage(int page, int size)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));

        var products = await _products
            .Find(product => product.IsPublished)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();

        await RestoreCategoriesAsync(products);
        return products;
    }

    private async Task RestoreCategoriesAsync(ICollection<Product> products)
    {
        if (products.Count == 0)
            return;

        var categoryIds = products
            .Select(product => product.CategoryId)
            .Distinct()
            .ToArray();

        var categories = await _categories
            .Find(Builders<Category>.Filter.In(category => category.Id, categoryIds))
            .ToListAsync();

        var categoriesById = categories.ToDictionary(category => category.Id);

        foreach (var product in products)
        {
            if (!categoriesById.TryGetValue(product.CategoryId, out var category))
                throw new InvalidOperationException(
                    $"Product {product.Id} references missing category {product.CategoryId}.");

            product.RestoreCategory(category);
        }
    }
}