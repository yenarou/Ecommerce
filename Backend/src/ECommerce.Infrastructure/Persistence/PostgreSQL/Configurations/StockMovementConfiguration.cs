using ECommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.ProductId)
            .IsRequired();

        builder.Property(m => m.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Quantity)
            .IsRequired();

        builder.Property(m => m.Reason)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
        .IsRequired();

        builder.HasIndex(m => m.ProductId);
        builder.HasIndex(m => m.CreatedAt);
    }
}