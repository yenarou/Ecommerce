using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion(
                status => status.Value,
                value => OrderStatus.FromString(value))
            .IsRequired();

        // Address
        builder.OwnsOne(order => order.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("State")
                .IsRequired();

            address.Property(a => a.ZipCode)
                .HasColumnName("ZipCode")
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .IsRequired();
        });
        
        #warning implementar costo total

        // User
        builder.HasOne(order => order.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();

        // Items
        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}