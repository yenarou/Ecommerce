using ECommerce.Domain.Entities;
using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItem");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Ignore(item => item.Product);

        builder.Property(item => item.Quantity)
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.Create(value))
            .IsRequired();

        builder.Property(item => item.CartId)
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired(false);

        builder.Property(item => item.CustomizationId)
            .IsRequired(false);

        builder.HasOne(item => item.Cart)
            .WithMany(cart => cart.Items)
            .HasForeignKey(item => item.CartId)
            .IsRequired();

        builder.HasOne(item => item.Customization)
            .WithMany()
            .HasForeignKey(item => item.CustomizationId)
            .IsRequired(false);
    }
}