using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.PostgreSQL.Repositories;

public class PostgreSqlCartRepository(ApplicationDbContext context) : ICartRepository
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
        return await context.Carts
            .Include(cart => cart.Items)
            .Where(cart => cart.User.Id == userId && cart.Status == CartStatus.Active)
            .FirstOrDefaultAsync();
    }

    public async Task Save(Cart cart)
    {
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