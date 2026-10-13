using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoDbCategoryRepository(IMongoDatabase database) : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categories =
        database.GetCollection<Category>("categories");
    private readonly IMongoCollection<Product> _products =
        database.GetCollection<Product>("products");

    public async Task<Category?> GetById(Guid categoryId)
    {
        return await _categories
            .Find(category => category.Id == categoryId)
            .FirstOrDefaultAsync();
    }

    public async Task<Category?> GetBySlug(string slug)
    {
        return await _categories
            .Find(category => category.Slug == slug)
            .FirstOrDefaultAsync();
    }

    public async Task<ICollection<Category>> GetAll()
    {
        return await _categories
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task Save(Category category)
    {
        var filter = Builders<Category>.Filter
            .Eq(existing => existing.Id, category.Id);

        await _categories.ReplaceOneAsync(
            filter,
            category,
            new ReplaceOptions
            {
                IsUpsert = true
            });
    }

    public async Task Delete(Guid categoryId)
    {
        var hasProducts = await _products
            .Find(product => product.CategoryId == categoryId)
            .AnyAsync();

        if (hasProducts)
            throw new InvalidOperationException(
                $"Cannot delete category {categoryId} while products reference it.");

        var filter = Builders<Category>.Filter
            .Eq(category => category.Id, categoryId);

        await _categories.DeleteOneAsync(filter);
    }
}