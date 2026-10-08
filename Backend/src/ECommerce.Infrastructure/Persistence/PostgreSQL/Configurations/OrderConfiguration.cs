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
            .HasConversion<string>()
            .IsRequired();

        // Address
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
        
        #warning implementar costo total

        // User
        builder.HasOne(order => order.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();

        // Items
        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.Order.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}