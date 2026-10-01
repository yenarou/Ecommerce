using ECommerce.Infrastructure.Repositories.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ECommerce.Infrastructure.Tests.Fixtures;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder()
            .WithDatabase("ecommerce_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, 5432)
            .Build();
    public ApplicationDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        Context = new ApplicationDbContext(options);

        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.DisposeAsync();
    }
}