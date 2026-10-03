
namespace ECommerce.Api.GraphQL.Mocks;

public class Query
{
    public List<CategoryMock> Categories()
    {
        return MockData.Categories;
    }

    public CategoryMock? Category(string slug)
    {
        return MockData.Categories
            .FirstOrDefault(category => category.Slug == slug);
    }

    public ProductPageMock Products(
        int page = 1,
        int pageSize = 12,
        string? categorySlug = null)
    {
        var products = MockData.Products.AsEnumerable();

        if (!string.IsNullOrEmpty(categorySlug))
        {
            var category = MockData.Categories
                .FirstOrDefault(c => c.Slug == categorySlug);

            if (category is null)
            {
                products = [];
            }
            else
            {
                products = products.Where(
                    product => product.CategoryId == category.Id);
            }
        }

        products = products
            .OrderByDescending(product => product.CreatedAt);

        var list = products.ToList();

        var total = list.Count;

        var items = list
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new ProductPageMock(
            items,
            total,
            page,
            pageSize,
            Math.Max(1, (int)Math.Ceiling(
                (double)total / pageSize))
        );
    }

    public ProductMock? Product(Guid id)
    {
        return MockData.Products
            .FirstOrDefault(product => product.Id == id);
    }
}