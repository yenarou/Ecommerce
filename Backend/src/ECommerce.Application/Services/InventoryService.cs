using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Services;

public class InventoryService(
    IProductRepository productRepository,
    IStockMovementRepository movementRepository)
{
    //camvios de stock
    public async Task<Product> Apply(
        Guid productId,
        int delta,
        string reason,
        Guid? referenceId = null,
        string createdBy = "admin")
    {
        if (delta == 0)
            throw new ArgumentException("Quantity cannot be zero.", nameof(delta));

        var product = await productRepository.GetById(productId)
            ?? throw new ProductNotFoundException(productId.ToString());

        var current = product.Stock.Value;
        var next = current + delta;

        if (next < 0)
            throw new InsufficientQuantityException(current, -delta);

        await movementRepository.Add(
            StockMovement.Create(product.Id, product.Name, delta, reason, referenceId, createdBy));

        product.UpdateStock(Quantity.Create(next));
        await productRepository.Save(product);

        return product;
    }

    //movimientos
    public async Task Log(
        Product product,
        int delta,
        string reason,
        Guid? referenceId = null,
        string createdBy = "admin")
    {
        if (delta == 0) return;

        await movementRepository.Add(
            StockMovement.Create(product.Id, product.Name, delta, reason, referenceId, createdBy));
    }
}