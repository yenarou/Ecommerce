using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Application.Services;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Application.DTOs.Requests.Admin;
using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Products;

public class UpdateProductStockUseCase(
    IProductRepository productRepository,
    InventoryService inventory)
{
    public async Task<ProductResponse> Execute(Guid productId, int stock, string createdBy = "admin")
    {
        if (stock < 0)
            throw new ArgumentOutOfRangeException(nameof(stock), stock, "Stock cannot be negative.");

        var product = await productRepository.GetById(productId)
            ?? throw new ProductNotFoundException(productId.ToString());

        var delta = stock - product.Stock.Value;
        if (delta == 0)
            return ProductMapper.ToResponse(product);

        product = await inventory.Apply(productId, delta, StockMovementReason.Adjustment, null, createdBy);

        return ProductMapper.ToResponse(product);
    }
}