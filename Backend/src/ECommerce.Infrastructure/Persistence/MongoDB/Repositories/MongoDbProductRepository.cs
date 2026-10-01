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

    public async Task<ICollection<Product>> FilterPublished(
        CatalogFilter catalogFilter,
        int page,
        int size)
    {
        if (catalogFilter == null)
            throw new ArgumentNullException(nameof(catalogFilter));

        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size));

        var filter = Builders<Product>.Filter.Eq(
            product => product.IsPublished,
            true);
        
        return await _products
            .Find(filter)
            .Skip((page - 1) * size)
            .Limit(size)
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
}