using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Configurations;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Cart>();
        modelBuilder.Ignore<CartItem>();
        modelBuilder.Ignore<Order>();
        modelBuilder.Ignore<OrderItem>();
        modelBuilder.Ignore<Product>();
        modelBuilder.Ignore<Category>();
        modelBuilder.Ignore<Customization>();

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        //modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}