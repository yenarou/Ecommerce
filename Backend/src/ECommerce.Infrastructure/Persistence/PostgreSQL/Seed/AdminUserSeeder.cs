
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence.PostgreSQL.Seed;

public class AdminUserSeeder(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IConfiguration configuration)
{
    public async Task SeedAsync()
    {
        var name = configuration["AdminUser:Name"] ?? "Administrador";
        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Configura AdminUser:Email y AdminUser:Password.");
        }

        var existingUser = await userRepository.GetByEmail(email);

        if (existingUser is not null)
        {
            if (existingUser.Role != UserRole.Admin)
            {
                throw new InvalidOperationException(
                    "El correo configurado ya existe, pero no es administrador. " +
                    "Usa otro correo de prueba o asigna el rol de forma controlada.");
            }

            return;
        }

        var user = User.CreateLocalUser(
            name,
            Email.Create(email),
            passwordHasher.Hash(password),
            UserRole.Admin);

        await userRepository.Save(user);
    }
}
