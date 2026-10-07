using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoDbProductRepository(IMongoDatabase database) : IProductRepository
{
    private readonly IMongoCollection<Product> _products =
        database.GetCollection<Product>("products");

    public async Task<Product?> GetById(Guid productId)
    {
        return await _products
            .Find(product => product.Id == productId)
            .FirstOrDefaultAsync();
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

        var total = await _products.CountDocumentsAsync(filter);

        var items = await _products
            .Find(filter)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();

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
                product => product.Category.Id,
                catalogFilter.CategoryId.Value);

        return await _products
            .Find(filter)
            .ToListAsync();
    }

    public async Task Save(Product product)
    {
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
        return await _products
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<ICollection<Product>> GetAllPublished()
    {
        return await _products
            .Find(product => product.IsPublished)
            .ToListAsync();
    }

    public async Task<ICollection<Product>> GetPublishedPage(int page, int size)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));

        return await _products
            .Find(product => product.IsPublished)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync();
    }
}