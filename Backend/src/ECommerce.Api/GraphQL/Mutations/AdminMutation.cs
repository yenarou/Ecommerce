using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Application.UseCases.Admin.Products;
using ECommerce.Application.UseCases.Admin.Categories;
using ECommerce.Application.UseCases.Admin.Orders;
using ECommerce.Domain.Models;
using ECommerce.Domain.Services;
using HotChocolate;

namespace ECommerce.Api.GraphQL.Mutations;

public record CreateProductInput(string Name, string Description, decimal Price, string Currency, int Stock, Guid CategoryId);
public record UpdateProductInput(string Name, string Description, decimal Price, string Currency, int Stock, Guid CategoryId);
public record CreateCategoryInput(string Slug, string Name, string Description);
public record UpdateCategoryInput(string Name, string Description);

public class AdminMutation
{
    public async Task<ProductResponse> CreateProduct(CreateProductInput input, [Service] CreateProductUseCase useCase) =>
        await useCase.Execute(new ECommerce.Application.DTOs.Requests.Admin.CreateProductRequest(
            input.Name, input.Description, input.Price, input.Currency, input.Stock, input.CategoryId));

    public Task<ProductResponse> UpdateProduct(Guid productId, UpdateProductInput input, [Service] UpdateProductUseCase useCase) =>
        useCase.Execute(productId, input.Name, input.Description, input.Price, input.Currency, input.Stock, input.CategoryId);

    public Task<ProductResponse> UpdateProductPrice(Guid productId, decimal price, string currency, [Service] UpdateProductPriceUseCase useCase) =>
        useCase.Execute(productId, price, currency);

    public Task<ProductResponse> UpdateProductStock(Guid productId, int stock, [Service] UpdateProductStockUseCase useCase) =>
        useCase.Execute(productId, stock);

    public Task<ProductResponse> SetProductPublication(Guid productId, bool published, [Service] SetProductPublicationUseCase useCase) =>
        useCase.Execute(productId, published);

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
}
