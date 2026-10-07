using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace ECommerce.Infrastructure.Persistence.MongoDB.Configuration;

public static class MongoDbConfiguration
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured)
            return;

        BsonSerializer.RegisterSerializer(
            new GuidSerializer(GuidRepresentation.Standard)
        );

        if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
        {
            BsonClassMap.RegisterClassMap<Product>(map =>
            {
                map.AutoMap();

                map.MapMember(product => product.Price);
                map.MapMember(product => product.Stock);
                map.MapMember(product => product.Images);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Image)))
        {
            BsonClassMap.RegisterClassMap<Image>(map =>
            {
                map.AutoMap();

                map.MapIdMember(image => image.Id);
                map.MapMember(image => image.Url);
                map.MapMember(image => image.Alt);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Money)))
        {
            BsonClassMap.RegisterClassMap<Money>(map =>
            {
                map.AutoMap();

                map.MapMember(money => money.Amount);
                map.MapMember(money => money.Currency);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Quantity)))
        {
            BsonClassMap.RegisterClassMap<Quantity>(map =>
            {
                map.AutoMap();

                map.MapMember(quantity => quantity.Value);
            });
        }

        _configured = true;
    }
}