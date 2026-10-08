using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customization> Customizations => Set<Customization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Product>();
        modelBuilder.Ignore<Category>();

        modelBuilder.ApplyConfiguration(new CartConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
        modelBuilder.ApplyConfiguration(new CustomizationConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}