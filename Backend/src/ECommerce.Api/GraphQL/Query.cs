using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.UseCases.Catalog;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Application.UseCases.Admin.Inventory;
using ECommerce.Application.UseCases.Admin.Products;
using ECommerce.Application.UseCases.Admin.Orders;

namespace ECommerce.Api.GraphQL;

public class Query
{
    //backoffice
    public async Task<ICollection<StockMovementResponse>> AdminStockMovements([Service] GetStockMovementsUseCase useCase, Guid? productId = null, int take = 100) =>
        await useCase.Execute(productId, take);

    public async Task<ICollection<ProductResponse>> AdminProducts([Service] GetAdminProductsUseCase useCase) =>
        await useCase.Execute();

    public async Task<ICollection<AdminOrderResponse>> AdminOrders([Service] GetAdminOrdersUseCase useCase) =>
        await useCase.Execute();

    public async Task<AdminOrderResponse?> AdminOrder(Guid id, [Service] GetAdminOrderUseCase useCase) =>
        await useCase.Execute(id);

    public async Task<ICollection<CategoryResponse>> Categories(
        [Service] GetCategoriesUseCase useCase)
    {
        return await useCase.Execute();
    }

    public async Task<CategoryResponse?> Category(
        string slug,
        [Service] GetCategoryUseCase useCase)
    {
        return await useCase.Execute(slug);
    }

    public async Task<PaginatedCollectionResponse<ProductResponse>> Products(
        [Service] GetPublishedProductsCase useCase,
        int page = 1,
        int pageSize = 12,
        string? categorySlug = null)
    {
        return await useCase.Execute(
            page,
            pageSize,
            categorySlug);
    }

    public async Task<ProductResponse?> Product(
        Guid id,
        [Service] GetProductDetailsUseCase useCase)
    {
        return await useCase.Execute(id);
    }

    public async Task<CartResponse> ActiveCart(
        [Service] GetActiveCartUseCase useCase)
    {
        return await useCase.Execute();
    }

    public async Task<ICollection<OrderResponse>> GetOrders(
        [Service] GetUserOrderHistoryUseCase useCase)
    {
        var orders = await useCase.Execute();

        return orders;
    }
    
    public async Task<CartResponse> GetCart(GetActiveCartUseCase useCase)
    {
        return await useCase.Execute();
    }
    
}