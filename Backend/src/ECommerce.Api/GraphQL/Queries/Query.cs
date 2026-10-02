namespace ECommerce.Api.GraphQL.Queries;

public class Query
{
    public Task<List<CategoryResponse>> Categories()
    {
        // Application
    }

    public Task<CategoryResponse?> Category(string slug)
    {
        // Application
    }

    public Task<ProductPageResponse> Products(
        int page = 1,
        int pageSize = 12,
        string? categorySlug = null)
    {
        // Application
    }

    public Task<ProductResponse?> Product(Guid id)
    {
        // Application
    }

    public Task<List<OrderResponse>> Orders(Guid userId)
    {
        // Application
    }
}