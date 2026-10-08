using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Seed;

public sealed class MongoDbSeeder(
    IMongoDatabase database,
    ILogger<MongoDbSeeder> logger)
{
    public async Task SeedAsync()
    {
        logger.LogInformation("Iniciando seeding de MongoDB...");

        var categories = database.GetCollection<Category>("categories");
        var products = database.GetCollection<Product>("products");

        var existingProducts = await products.CountDocumentsAsync(
            FilterDefinition<Product>.Empty);
        if (existingProducts > 0)
        {
            logger.LogInformation(
                "Se conservan los {Count} productos existentes; no se ejecuta el seeding.",
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
                        "http://localhost:14001/images/products/shyguy.png",
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
                        "http://localhost:14001/images/products/marioycapi.png",
                        "Mario tejido a mano"
                    ),
                    Image.Create(
                        "http://localhost:14001/images/products/mario.png",
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
                        "http://localhost:14001/images/products/snoopy.png",
                        "Snoop Dogg tejido"
                    ),
                    Image.Create(
                        "http://localhost:14001/images/products/snoop.png",
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
                        "http://localhost:14001/images/products/sunflower.png",
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
                        "http://localhost:14001/images/products/flores.png",
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
                        "http://localhost:14001/images/products/mandalas.png",
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