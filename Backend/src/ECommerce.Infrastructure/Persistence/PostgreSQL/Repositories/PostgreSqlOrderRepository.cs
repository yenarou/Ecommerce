using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PostgreSqlOrderRepository(ApplicationDbContext context) : IOrderRepository
{
    private IQueryable<Order> OrdersWithRelations =>
        context.Orders
            .Include(o => o.User)
            .Include(o => o.Items);

    public async Task<Order?> GetById(Guid orderId) =>
        await OrdersWithRelations.FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task<ICollection<Order>> GetByUserId(Guid userId) =>
        await OrdersWithRelations
            .Where(o => EF.Property<Guid>(o, "UserId") == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<ICollection<Order>> GetAll() =>
        await OrdersWithRelations
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task Save(Order order)
    {
        if (context.Entry(order).State == EntityState.Detached)
            await context.Orders.AddAsync(order);

        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return;

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }
}

/*using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PostgreSqlOrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<ICollection<Order>> GetAll()
    {
        return await context.Orders
            .Include(order => order.Items)
            .Include(order => order.User)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetById(Guid orderId)
    {
        return await context.Orders
            .Include(order => order.Items)
            .Include(order => order.User)
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
}*/