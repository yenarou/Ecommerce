using ECommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        // Product vive en MongoDB: no se mapea, solo se guarda una copia plana
        builder.Ignore(i => i.Product);

        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired();

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("unit_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("unit_price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(i => i.Quantity, q =>
        {
            q.Property(x => x.Value)
                .HasColumnName("quantity")
                .IsRequired();
        });

        builder.OwnsOne(i => i.Customization, c =>
        {
            c.Ignore(x => x.Id);

            c.Property(x => x.Description)
                .HasColumnName("customization_description")
                .HasMaxLength(500);

            c.Property(x => x.IsWrap)
                .HasColumnName("customization_is_wrap");

            c.Property(x => x.AdditionalPrice)
                .HasColumnName("customization_additional_price")
                .HasPrecision(18, 2);

            c.Property(x => x.CreatedAt)
                .HasColumnName("customization_created_at");
        });
    }
}