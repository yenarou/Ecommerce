namespace ECommerce.Api.GraphQL.Mocks;
public static class MockData
{
    public static readonly Guid FigurasId =
        Guid.Parse("10000000-0000-0000-0000-000000000001");

    public static readonly Guid LlaverosId =
        Guid.Parse("10000000-0000-0000-0000-000000000002");

    public static readonly Guid DecoracionId =
        Guid.Parse("10000000-0000-0000-0000-000000000003");

    public static readonly List<CategoryMock> Categories =
    [
        new(
            FigurasId,
            "figuras",
            "Figuras",
            "Figuras de peluche tejidas, ideales para regalo o colección."
        ),

        new(
            LlaverosId,
            "llaveros",
            "Llaveros",
            "Piezas pequeñas para mochila, bolsa o llaves."
        ),

        new(
            DecoracionId,
            "decoracion",
            "Decoración",
            "Piezas para repisa, escritorio o pared."
        )
    ];

    public static readonly List<ProductMock> Products =
    [
        new(
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            "Shy-Guy",
            "Es un personaje tejido a mano de 15cm de alto.",
            320,
            5,
            "/images/shyguy.png",
            "/images/shyguy.png",
            5,
            "2026-09-01T10:00:00.000Z",
            FigurasId
        ),

        new(
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            "Mario",
            "Fontanero tejido a mano.",
            310,
            3,
            "/images/marioycapi.png",
            "/images/mario.png",
            4,
            "2026-09-02T10:00:00.000Z",
            FigurasId
        ),

        new(
            Guid.Parse("20000000-0000-0000-0000-000000000003"),
            "Snoop Dogg",
            "Perro blanco y negro tejido.",
            95,
            20,
            "/images/snoopy.png",
            "/images/snoop.png",
            1,
            "2026-09-03T10:00:00.000Z",
            LlaverosId
        ),

        new(
            Guid.Parse("20000000-0000-0000-0000-000000000004"),
            "Flor del sol",
            "Flor tejida.",
            110,
            0,
            "/images/sunflower.png",
            "/images/snoop.png",
            1,
            "2026-09-04T10:00:00.000Z",
            LlaverosId
        ),

        new(
            Guid.Parse("20000000-0000-0000-0000-000000000005"),
            "Ramo de flores",
            "Flores de crochet.",
            560,
            2,
            "/images/flores.png",
            "/images/flores.png",
            6,
            "2026-09-05T10:00:00.000Z",
            DecoracionId
        ),

        new(
            Guid.Parse("20000000-0000-0000-0000-000000000006"),
            "Mandalas colgantes",
            "Decoración de mandalas.",
            310,
            9,
            "/images/mandalas.png",
            "/images/snoop.png",
            3,
            "2026-09-06T10:00:00.000Z",
            DecoracionId
        )
    ];

    public static readonly List<CustomizationMock> Customizations =
    [
        new(
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            "Cambiar color base",
            "https://images.unsplash.com/photo-1520903920243-7ce9d02d3a08",
            0
        ),

        new(
            Guid.Parse("30000000-0000-0000-0000-000000000002"),
            "Bordar iniciales",
            "https://images.unsplash.com/photo-1519241047957-be31d7379a5d",
            60
        ),

        new(
            Guid.Parse("30000000-0000-0000-0000-000000000003"),
            "Agregar moño o accesorio",
            "https://images.unsplash.com/photo-1517705008128-361805f42e07",
            45
        ),

        new(
            Guid.Parse("30000000-0000-0000-0000-000000000004"),
            "Empaque de regalo",
            "https://images.unsplash.com/photo-1549465220-1a8b9238cd48",
            35
        )
    ];
}
public record CategoryMock(
    Guid Id,
    string Slug,
    string Name,
    string? Description
);

public record CustomizationMock(
    Guid Id,
    string Description,
    string? ImageUrl,
    double AdditionalPrice
);

public record ProductMock(
    Guid Id,
    string Name,
    string? Description,
    double Price,
    int Stock,
    string? ImageUrl,
    string? ImageUrlAlt,
    int DaysToMake,
    string CreatedAt,
    Guid CategoryId
);

public record ProductPageMock(
    List<ProductMock> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);