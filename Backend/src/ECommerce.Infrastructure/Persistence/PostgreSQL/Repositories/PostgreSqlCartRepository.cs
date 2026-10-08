using ECommerce.Domain.Models;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PostgreSqlCartRepository(
    ApplicationDbContext context,
    IProductRepository productRepository) : ICartRepository
{
    public async Task<Cart?> GetById(Guid cartId)
    {
        return await context.Carts
            .Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.Id == cartId);
    }

    public async Task<ICollection<Cart>?> GetByUserId(Guid userId)
    {
        return await context.Carts
            .Include(cart => cart.Items)
            .Where(cart => cart.User.Id == userId)
            .ToListAsync();
    }

    public async Task<Cart?> GetActiveByUserId(Guid userId)
    {
        var cart = await context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Customization)
            .Where(cart => cart.User.Id == userId && cart.Status == CartStatus.Active)
            .FirstOrDefaultAsync();

        if (cart is null)
            return null;

        foreach (var item in cart.Items)
        {
            var productId = context.Entry(item)
                .Property<Guid?>("ProductId")
                .CurrentValue;

            if (!productId.HasValue)
                throw new InvalidOperationException(
                    $"Cart item {item.Id} has no stored product ID. Remove it and add the product again.");

            var product = await productRepository.GetById(productId.Value);
            if (product is null)
                throw new ProductNotFoundException(productId.Value.ToString());

            item.RestoreProduct(product);
        }

        return cart;
    }

    public async Task Save(Cart cart)
    {
        foreach (var item in cart.Items)
        {
            context.Entry(item)
                .Property<Guid?>("ProductId")
                .CurrentValue = item.Product.Id;
        }

        var exists = await context.Carts
            .AnyAsync(existing => existing.Id == cart.Id);

        if (exists)
            context.Carts.Update(cart);
        else
            await context.Carts.AddAsync(cart);

        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid cartId)
    {
        var cart = await context.Carts
            .FirstOrDefaultAsync(cart => cart.Id == cartId);

        if (cart is null)
            return;

        context.Carts.Remove(cart);
        await context.SaveChangesAsync();
    }
}