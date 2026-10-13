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
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Image> Images => Set<Image>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Cart>();
        modelBuilder.Ignore<CartItem>();
        modelBuilder.Ignore<Image>();
        modelBuilder.Ignore<Product>();
        modelBuilder.Ignore<Category>();
        /*modelBuilder.Ignore<Order>();
        modelBuilder.Ignore<OrderItem>();
        modelBuilder.Ignore<Customization>();*/

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        /*modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());*/
    }
}