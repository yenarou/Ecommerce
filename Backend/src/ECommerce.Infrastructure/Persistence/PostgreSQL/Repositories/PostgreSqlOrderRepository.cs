using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PostgreSqlOrderRepository(
    ApplicationDbContext context,
    IProductRepository productRepository) : IOrderRepository
{
    public async Task<Order?> GetById(Guid orderId)
    {
        var order = await context.Orders
            .Include(order => order.User)
            .Include(order => order.Items)
            .ThenInclude(item => item.Customization)
            .FirstOrDefaultAsync(order => order.Id == orderId);

        if (order is null)
            return null;

        await RestoreProducts(order);

        return order;
    }

    public async Task<ICollection<Order>> GetByUserId(Guid userId)
    {
        var orders = await context.Orders
            .Include(order => order.Items)
            .ThenInclude(item => item.Customization)
            .Where(order => order.UserId == userId)
            .ToListAsync();

        foreach (var order in orders)
            await RestoreProducts(order);

        return orders;
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

    private async Task RestoreProducts(Order order)
    {
        foreach (var item in order.Items)
        {
            var productId = context.Entry(item)
                .Property<Guid?>("ProductId")
                .CurrentValue;

            if (!productId.HasValue)
                throw new InvalidOperationException(
                    $"Order item {item.Id} has no stored product ID.");

            var product = await productRepository.GetById(productId.Value);
            if (product is null)
                throw new ProductNotFoundException(productId.Value.ToString());

            item.RestoreProduct(product);
        }
    }
}