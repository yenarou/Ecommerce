using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Configurations;

public class CustomizationConfiguration : IEntityTypeConfiguration<Customization>
{
    public void Configure(EntityTypeBuilder<Customization> builder)
    {
        builder.ToTable("Customization");

        builder.HasKey(customization => customization.Id);

        builder.Property(customization => customization.Id)
            .ValueGeneratedNever();

        builder.Property(customization => customization.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(customization => customization.CreatedAt)
            .IsRequired();

        builder.Property(customization => customization.IsWrap)
            .IsRequired();

        builder.Property(customization => customization.AdditionalPrice)
            .HasPrecision(10, 2)
            .IsRequired();
    }
}