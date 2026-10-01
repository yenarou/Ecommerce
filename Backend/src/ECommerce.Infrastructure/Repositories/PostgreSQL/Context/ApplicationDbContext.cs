using ECommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.PostgreSQL.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}