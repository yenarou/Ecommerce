using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Application.DTOs.Requests.Admin;
using   ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Products;

public class CreateProductUseCase(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ProductResponse> Execute(
        CreateProductRequest request)
    {
        var category =
            await categoryRepository.GetById(request.CategoryId);

        if (category is null)
            throw new ArgumentException("Category not found.");

        var price = Money.Create(
            request.Price,
            request.Currency);

        var stock = Quantity.Create(
            request.Stock);

        var product = Product.Create(
            request.Name,
            request.Description,
            price,
            stock,
            category,
            new List<Image>());

        foreach (var url in request.ImageUrls ?? [])
            product.AddImage(url, request.Name);

        await productRepository.Save(product);

        return ProductMapper.ToResponse(product);
    }
}