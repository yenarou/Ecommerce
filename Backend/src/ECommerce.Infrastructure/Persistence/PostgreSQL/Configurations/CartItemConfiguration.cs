using ECommerce.Domain.Models;
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

        builder.Property<Guid?>("ProductId");


        builder.Property(item => item.Quantity)
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.Create(value))
            .IsRequired();

        builder.HasOne(item => item.Cart)
            .WithMany(cart => cart.Items)
            .HasForeignKey("CartId")
            .IsRequired();

        builder.HasOne(item => item.Customization)
            .WithMany()
            .HasForeignKey("CustomizationId")
            .IsRequired(false);
    }
}