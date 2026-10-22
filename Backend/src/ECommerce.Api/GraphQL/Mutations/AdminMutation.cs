using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using System.Security.Claims;
using ECommerce.Application.UseCases.Admin.Inventory;
using ECommerce.Application.UseCases.Admin.Products;
using ECommerce.Application.UseCases.Admin.Categories;
using ECommerce.Application.UseCases.Admin.Orders;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Services;
using HotChocolate;
using HotChocolate.Types;

namespace ECommerce.Api.GraphQL.Mutations;

public record CreateProductInput(string Name, string Description, decimal Price, string Currency, int Stock, Guid CategoryId, List<string>? ImageUrls = null);
public record UpdateProductInput(string Name, string Description, decimal Price, string Currency, int Stock, Guid CategoryId);
public record CreateCategoryInput(string Slug, string Name, string Description);
public record UpdateCategoryInput(string Name, string Description);

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AdminMutation
{
    public async Task<ProductResponse> CreateProduct(CreateProductInput input, [Service] CreateProductUseCase useCase) =>
        await useCase.Execute(new ECommerce.Application.DTOs.Requests.Admin.CreateProductRequest(
            input.Name, input.Description, input.Price, input.Currency, input.Stock, input.CategoryId, input.ImageUrls));

    public Task<ProductResponse> UpdateProduct(Guid productId, UpdateProductInput input, [Service] UpdateProductUseCase useCase) =>
        useCase.Execute(productId, input.Name, input.Description, input.Price, input.Currency, input.Stock, input.CategoryId);

    public Task<ProductResponse> UpdateProductPrice(Guid productId, decimal price, string currency, [Service] UpdateProductPriceUseCase useCase) =>
        useCase.Execute(productId, price, currency);

    //modificacion para backoffice
    public Task<ProductResponse> UpdateProductStock(Guid productId, int stock, ClaimsPrincipal user, [Service] UpdateProductStockUseCase useCase) =>
        useCase.Execute(productId, stock, GetUserEmail(user));

    //ajuste de stock
    public Task<ProductResponse> AdjustProductStock(Guid productId, int quantity, ClaimsPrincipal user, [Service] AdjustProductStockUseCase useCase) =>
        useCase.Execute(productId, quantity, GetUserEmail(user));

    public Task<ProductResponse> SetProductPublication(Guid productId, bool published, [Service] SetProductPublicationUseCase useCase) =>
        useCase.Execute(productId, published);

    public Task<ProductResponse> UpdateProductImage(Guid productId, string imageUrl, [Service] UpdateProductImageUseCase useCase) =>
        useCase.Execute(productId, imageUrl);

    public async Task<bool> DeleteProduct(Guid productId, [Service] DeleteProductUseCase useCase)
    {
        await useCase.Execute(productId);
        return true;
    }

    public async Task<CategoryResponse> CreateCategory(CreateCategoryInput input, [Service] CreateCategoryUseCase useCase) =>
        CategoryMapper.ToResponse(await useCase.Execute(input.Slug, input.Name, input.Description));

    public async Task<CategoryResponse> UpdateCategory(Guid categoryId, UpdateCategoryInput input, [Service] UpdateCategoryUseCase useCase)
    {
        var category = await useCase.Execute(categoryId, input.Name, input.Description);
        return CategoryMapper.ToResponse(category);
    }

    public async Task<bool> DeleteCategory(Guid categoryId, [Service] DeleteCategoryUseCase useCase)
    {
        await useCase.Execute(categoryId);
        return true;
    }

    public async Task<bool> UpdateOrderStatus(Guid orderId, string status, [Service] UpdateOrderStatusUseCase useCase)
    {
        await useCase.Execute(orderId, status);
        return true;
    }

    public async Task<string> UploadProductImage(IFile file, IImageStorage imageStorage)
    {
        await using var stream = file.OpenReadStream();
        return await imageStorage.SaveAsync(stream, file.Name, file.ContentType);
    }

    //email para backoffice
    private static string GetUserEmail(ClaimsPrincipal user) =>
        user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value ?? "admin";
}
