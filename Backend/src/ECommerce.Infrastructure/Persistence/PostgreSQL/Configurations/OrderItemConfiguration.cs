using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.ProductName)
            .IsRequired();
        

        builder.Property(item => item.UnitPrice)
            .HasConversion(
                money => money.Amount,
                value => Money.Create(value, "MXN"))
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.Create(value))
            .IsRequired();

        builder.Property(item => item.OrderId)
            .IsRequired();

        builder.Property(item => item.CustomizationId)
            .IsRequired(false);

        builder.HasOne(item => item.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(item => item.OrderId)
            .IsRequired();

        builder.HasOne(item => item.Customization)
            .WithMany()
            .HasForeignKey(item => item.CustomizationId)
            .IsRequired(false);
        
        builder.Property(item => item.ProductImageId)
            .IsRequired(false);

        builder.Ignore(item => item.ProductImage);
    }
}