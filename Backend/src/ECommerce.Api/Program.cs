
using ECommerce.Api.GraphQL.Mocks;
using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Auth;
using ECommerce.Application.UseCases.Catalog;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Auth;
using ECommerce.Infrastructure.Persistence.MongoDB.Repositories;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Context;
using ECommerce.Infrastructure.Repositories.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure - Persistence - PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Infrastructure - Persistence - MongoDB
builder.Services.AddSingleton<IMongoClient>(new MongoClient(
    builder.Configuration.GetConnectionString("MongoDB")));
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IMongoClient>().GetDatabase("ECommerce"));

// Repositories
builder.Services.AddScoped<ICartRepository, PostgreSqlCartRepository>();
builder.Services.AddScoped<ICategoryRepository, MongoDbCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, PostgreSqlOrderRepository>();
builder.Services.AddScoped<IProductRepository, MongoDbProductRepository>();
builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();

// Services
builder.Services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Use Cases - Auth
builder.Services.AddScoped<AuthenticateWithGoogleUseCase>();
builder.Services.AddScoped<LoginWithEmailUseCase>();
builder.Services.AddScoped<RegisterWithEmailUseCase>();

// Use Cases - Catalog
builder.Services.AddScoped<FilterCategoryPublishedProducts>();
builder.Services.AddScoped<FilterPublishedProductsUseCase>();
builder.Services.AddScoped<GetCategoriesUseCase>();
builder.Services.AddScoped<GetCategoryUseCase>();
builder.Services.AddScoped<GetProductDetailsUseCase>();
builder.Services.AddScoped<GetPublishedProductsCase>();


builder.Services.AddControllers();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection("Google"));

builder.Services
    .AddGraphQLServer()
    .AddMutationType<AuthMutation>()
    .AddQueryType<Query>()
    .AddType<ProductType>()
    .AddType<CategoryType>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudieron crear las tablas");
    }
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGraphQL();

app.Run();