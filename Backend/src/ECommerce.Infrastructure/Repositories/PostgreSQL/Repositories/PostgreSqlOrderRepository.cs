using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.PostgreSQL.Repositories;

public class PostgreSqlOrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order?> GetById(Guid orderId)
    {
        return await context.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == orderId);
    }

    public async Task<ICollection<Order>> GetByUserId(Guid userId)
    {
        return await context.Orders
            .Include(order => order.Items)
            .Where(order => order.User.Id == userId)
            .ToListAsync();
    }

    public async Task Save(Order order)
    {
        var exists = await context.Orders
            .AnyAsync(existing => existing.Id == order.Id);

        if (exists)
            context.Orders.Update(order);
        else
            await context.Orders.AddAsync(order);

        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid orderId)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == orderId);

        if (order is null)
            return;

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }
}