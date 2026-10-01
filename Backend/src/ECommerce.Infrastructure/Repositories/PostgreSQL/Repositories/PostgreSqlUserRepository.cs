using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.PostgreSQL.Repositories;

public class PostgreSqlUserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetById(Guid userId)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Id == userId);
    }
    
    public async Task<User?> GetByEmail(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Email.Value == email);
    }

    public async Task<User?> GetByGoogleId(string googleId)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.GoogleId == googleId);
    }

    public async Task Save(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid userId)
    {
        var user = await GetById(userId);

        if (user is null)
            return;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}