using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PostgreSqlStockMovementRepository(ApplicationDbContext context) : IStockMovementRepository
{
    public async Task Add(StockMovement movement)
    {
        await context.StockMovements.AddAsync(movement);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<StockMovement>> GetRecent(Guid? productId, int take)
    {
        var query = context.StockMovements.AsNoTracking();

        if (productId.HasValue)
            query = query.Where(m => m.ProductId == productId.Value);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}