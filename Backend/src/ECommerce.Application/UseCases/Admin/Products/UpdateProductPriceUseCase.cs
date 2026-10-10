using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Application.DTOs.Requests.Admin;
using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Products;

public class UpdateProductPriceUseCase(
    IProductRepository productRepository)
{
    public async Task<ProductResponse> Execute(
        Guid productId,
        decimal price,
        string currency)
    {
        var product =
            await productRepository.GetById(productId);

        if (product is null)
            throw new ProductNotFoundException(productId.ToString());

        product.UpdatePrice(
            Money.Create(price, currency));

        await productRepository.Save(product);

        return ProductMapper.ToResponse(product);
    }
}