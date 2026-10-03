using HotChocolate.Types;

namespace ECommerce.Api.GraphQL.Mocks;

public class ProductType : ObjectType<ProductMock>
{
    protected override void Configure(IObjectTypeDescriptor<ProductMock> descriptor)
    {
        descriptor.Field("category")
            .Resolve(context =>
            {
                var product = context.Parent<ProductMock>();
                return MockData.Categories
                    .First(category => category.Id == product.CategoryId);
            });

        descriptor.Field("customizationOptions")
            .Resolve(_ => MockData.Customizations);
    }
}

public class CategoryType : ObjectType<CategoryMock>
{
    protected override void Configure(IObjectTypeDescriptor<CategoryMock> descriptor)
    {
        descriptor.Field("products")
            .Resolve(context =>
            {
                var category = context.Parent<CategoryMock>();
                return MockData.Products
                    .Where(product => product.CategoryId == category.Id)
                    .ToList();
            });
    }
}