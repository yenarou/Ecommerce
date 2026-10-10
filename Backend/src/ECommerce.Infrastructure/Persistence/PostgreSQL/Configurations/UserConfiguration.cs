using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Repositories.PostgreSQL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired();

        //roles
        builder.Property(user => user.Role)
                .HasConversion<int>()
                .HasDefaultValue(UserRole.Customer)
                .IsRequired();

        builder.Property(user => user.PasswordHash)
            .IsRequired(false);

        builder.Property(user => user.GoogleId)
            .IsRequired(false);

        builder.Property(user => user.AuthProvider)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.HasIndex(user => user.GoogleId)
            .IsUnique();
    }
}