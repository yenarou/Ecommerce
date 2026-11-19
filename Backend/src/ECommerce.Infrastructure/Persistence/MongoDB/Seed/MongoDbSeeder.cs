using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Seed;

public sealed class MongoDbSeeder(
    IMongoDatabase database,
    IConfiguration configuration,
    ILogger<MongoDbSeeder> logger)
{
    public async Task SeedAsync()
    {
        logger.LogInformation("Iniciando seeding de MongoDB...");

        var categories = database.GetCollection<Category>("categories");
        var products = database.GetCollection<Product>("products");

        await MigrateCategorySlugsAsync(database);
        await MigrateProductCategoryIdsAsync(database);
        await MigrateLegacyProductImageUrlsAsync(database);
        await products.Indexes.CreateOneAsync(
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(product => product.CategoryId)));

        var existingProducts = await products.CountDocumentsAsync(
            FilterDefinition<Product>.Empty);
        if (existingProducts > 0)
        {
            logger.LogInformation(
                "Se conservan los {Count} productos existentes; se normalizan las URLs de imagen legacy.",
                existingProducts);
            return;
        }

        logger.LogInformation("Creando o reutilizando categorías...");

        var figuras = await GetOrCreateCategoryAsync(
            categories,
            "figuras",
            "Figuras",
            "Figuras de peluche tejidas, ideales para regalo o colección."
        );

        var llaveros = await GetOrCreateCategoryAsync(
            categories,
            "llaveros",
            "Llaveros",
            "Piezas pequeñas para mochila, bolsa o llaves."
        );

        var decoracion = await GetOrCreateCategoryAsync(
            categories,
            "decoracion",
            "Decoración",
            "Piezas para repisa, escritorio o pared."
        );

        logger.LogInformation("Creando productos...");

        var imageBaseUrl = (configuration["ApiBaseUrl"] ?? "http://localhost:14001")
            .TrimEnd('/');
        string ImageUrl(string fileName) =>
            $"{imageBaseUrl}/images/products/{fileName}";

        var productsToInsert = new List<Product>
        {
            Product.Create(
                "Shy-Guy",
                "Es un personaje tejido a mano de 15cm de alto.",
                Money.Create(320m, "MXN"),
                Quantity.Create(5),
                figuras,
                [
                    Image.Create(
                        ImageUrl("shyguy.webp"),
                        "Shy-Guy tejido a mano"
                    )
                ]
            ),

            Product.Create(
                "Mario",
                "Fontanero tejido a mano.",
                Money.Create(310m, "MXN"),
                Quantity.Create(3),
                figuras,
                [
                    Image.Create(
                        ImageUrl("marioycapi.webp"),
                        "Mario tejido a mano"
                    ),
                    Image.Create(
                        ImageUrl("mario.webp"),
                        "Mario tejido a mano"
                    )
                ]
            ),

            Product.Create(
                "Snoop Dogg",
                "Perro blanco y negro tejido.",
                Money.Create(95m, "MXN"),
                Quantity.Create(20),
                llaveros,
                [
                    Image.Create(
                        ImageUrl("snoopy.webp"),
                        "Snoop Dogg tejido"
                    ),
                    Image.Create(
                        ImageUrl("snoop.webp"),
                        "Snoop Dogg tejido"
                    )
                ]
            ),

            Product.Create(
                "Flor del sol",
                "Flor tejida.",
                Money.Create(110m, "MXN"),
                Quantity.Create(0),
                llaveros,
                [
                    Image.Create(
                        ImageUrl("sunflower.webp"),
                        "Flor del sol tejida"
                    )
                ]
            ),

            Product.Create(
                "Ramo de flores",
                "Flores de crochet.",
                Money.Create(560m, "MXN"),
                Quantity.Create(2),
                decoracion,
                [
                    Image.Create(
                        ImageUrl("flores.webp"),
                        "Ramo de flores de crochet"
                    )
                ]
            ),

            Product.Create(
                "Mandalas colgantes",
                "Decoración de mandalas.",
                Money.Create(310m, "MXN"),
                Quantity.Create(9),
                decoracion,
                [
                    Image.Create(
                        ImageUrl("mandalas.webp"),
                        "Mandalas colgantes"
                    )
                ]
            )
        };

        await products.InsertManyAsync(productsToInsert);

        logger.LogInformation(
            "Se insertaron {Count} productos.",
            productsToInsert.Count);

        logger.LogInformation(
            "Seeding de MongoDB completado correctamente.");
    }

    private async Task MigrateLegacyProductImageUrlsAsync(IMongoDatabase database)
    {
        var products = database.GetCollection<BsonDocument>("products");
        var productCursor = await products.FindAsync(new BsonDocument());
        var legacyProducts = await productCursor.ToListAsync();
        var imageBaseUrl = (configuration["ApiBaseUrl"] ?? "http://localhost:14001")
            .TrimEnd('/');

        foreach (var product in legacyProducts)
        {
            if (!product.TryGetValue("Images", out var imagesValue) ||
                imagesValue is not BsonArray imagesArray)
            {
                continue;
            }

            var normalized = false;
            var updatedImages = new BsonArray();

            foreach (var imageEntry in imagesArray)
            {
                if (imageEntry is not BsonDocument imageDocument)
                {
                    updatedImages.Add(imageEntry);
                    continue;
                }

                if (!imageDocument.TryGetValue("Url", out var urlValue) ||
                    !urlValue.IsString)
                {
                    updatedImages.Add(imageDocument);
                    continue;
                }

                var currentUrl = urlValue.AsString;
                if (!Uri.TryCreate(currentUrl, UriKind.Absolute, out var imageUri))
                {
                    updatedImages.Add(imageDocument);
                    continue;
                }

                var imagePath = imageUri.AbsolutePath;
                if (!imagePath.StartsWith("/images/products/", StringComparison.OrdinalIgnoreCase) &&
                    !imagePath.StartsWith("/uploads/products/", StringComparison.OrdinalIgnoreCase))
                {
                    updatedImages.Add(imageDocument);
                    continue;
                }

                var fileName = Path.GetFileName(imagePath);
                var isUploadedImage = Guid.TryParse(
                    Path.GetFileNameWithoutExtension(fileName),
                    out _);
                if (!isUploadedImage)
                {
                    fileName = fileName.Replace(
                        ".png",
                        ".webp",
                        StringComparison.OrdinalIgnoreCase);
                }

                var imageFolder = isUploadedImage
                    ? "uploads/products"
                    : "images/products";
                var normalizedUrl = $"{imageBaseUrl}/{imageFolder}/{Uri.EscapeDataString(fileName)}";
                if (currentUrl == normalizedUrl)
                {
                    updatedImages.Add(imageDocument);
                    continue;
                }

                imageDocument["Url"] = normalizedUrl;
                updatedImages.Add(imageDocument);
                normalized = true;
            }

            if (normalized)
            {
                await products.UpdateOneAsync(
                    new BsonDocument("_id", product["_id"]),
                    Builders<BsonDocument>.Update.Set("Images", updatedImages));
            }
        }
    }

    private static async Task MigrateProductCategoryIdsAsync(IMongoDatabase database)
    {
        var products = database.GetCollection<BsonDocument>("products");
        var missingCategoryId = new BsonDocument(
            "CategoryId",
            new BsonDocument("$exists", false));

        var legacyProducts = await products
            .Find(missingCategoryId)
            .ToListAsync();

        foreach (var product in legacyProducts)
        {
            if (!product.TryGetValue("Category", out var categoryValue) ||
                !categoryValue.IsBsonDocument)
            {
                throw new InvalidOperationException(
                    $"Product {product.GetValue("_id", BsonNull.Value)} has no embedded category to migrate.");
            }

            var embeddedCategory = categoryValue.AsBsonDocument;
            var categoryId = embeddedCategory.GetValue(
                "_id",
                embeddedCategory.GetValue("Id", BsonNull.Value));

            if (categoryId.IsBsonNull)
                throw new InvalidOperationException(
                    $"Product {product.GetValue("_id", BsonNull.Value)} has no embedded category ID to migrate.");

            var filter = new BsonDocument("_id", product["_id"]);
            var update = Builders<BsonDocument>.Update
                .Set("CategoryId", categoryId)
                .Unset("Category");
            await products.UpdateOneAsync(filter, update);
        }
    }

    private static async Task MigrateCategorySlugsAsync(IMongoDatabase database)
    {
        var categories = database.GetCollection<BsonDocument>("categories");
        var missingSlug = Builders<BsonDocument>.Filter.Or(
            Builders<BsonDocument>.Filter.Exists("Slug", false),
            Builders<BsonDocument>.Filter.Eq("Slug", ""));

        var legacyCategories = await categories.Find(missingSlug).ToListAsync();

        foreach (var category in legacyCategories)
        {
            if (!category.TryGetValue("Name", out var nameValue) ||
                !nameValue.IsString ||
                string.IsNullOrWhiteSpace(nameValue.AsString))
            {
                throw new InvalidOperationException(
                    $"Category {category.GetValue("_id", BsonNull.Value)} has no name to migrate its slug.");
            }

            var slug = nameValue.AsString
                .Trim()
                .ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD);
            var slugChars = slug
                .Where(character => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) !=
                                    System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray();
            slug = new string(slugChars)
                .Replace(' ', '-')
                .Replace("--", "-");

            var update = Builders<BsonDocument>.Update.Set("Slug", slug);
            await categories.UpdateOneAsync(
                new BsonDocument("_id", category["_id"]),
                update);
        }
    }

    private static async Task<Category> GetOrCreateCategoryAsync(
        IMongoCollection<Category> categories,
        string slug,
        string name,
        string description)
    {
        var existingCategory = await categories
            .Find(category => category.Slug == slug)
            .FirstOrDefaultAsync();

        if (existingCategory is not null)
            return existingCategory;

        var category = Category.Create(slug, name, description);
        await categories.InsertOneAsync(category);
        return category;
    }
}