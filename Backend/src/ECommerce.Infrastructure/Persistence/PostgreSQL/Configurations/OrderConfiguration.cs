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
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(order => order.Address, address =>
        {
            address.Property(a => a.Street)
            .HasColumnName("address_street")
            .IsRequired();

            address.Property(a => a.City)
            .HasColumnName("address_city")
            .IsRequired();

            address.Property(a => a.State)
            .HasColumnName("address_state")
            .IsRequired();

            address.Property(a => a.ZipCode)
            .HasColumnName("address_zip_code")
            .IsRequired();

            address.Property(a => a.Country)
            .HasColumnName("address_country")
            .IsRequired();
        });

        builder.OwnsOne(order => order.Total, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(order => order.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}