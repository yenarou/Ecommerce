using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Application.DTOs.Requests.Admin;
using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Products;

public class DeleteProductUseCase(
    IProductRepository productRepository)
{
    public async Task Execute(Guid productId)
    {
        var product =
            await productRepository.GetById(productId);

        if (product is null)
            throw new ProductNotFoundException(productId.ToString());

        await productRepository.Delete(productId);
    }
}