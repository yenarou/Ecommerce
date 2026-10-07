using System.Text;
using ECommerce.Api.GraphQL;
using ECommerce.Api.GraphQL.Mutations;
using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Auth;
using ECommerce.Application.UseCases.Catalog;
using ECommerce.Application.UseCases.Checkout;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Auth;
using ECommerce.Infrastructure.Persistence.MongoDB.Configuration;
using ECommerce.Infrastructure.Persistence.MongoDB.Repositories;
using ECommerce.Infrastructure.Persistence.MongoDB.Seed;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Context;
using ECommerce.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

using ExceptionHandlerMiddleware = ECommerce.Api.Middleware.ExceptionHandlerMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure - Persistence - PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));



// Infrastructure - Persistence - MongoDB
MongoDbConfiguration.Configure();

builder.Services.AddSingleton<IMongoClient>(new MongoClient(
    builder.Configuration.GetConnectionString("MongoDB")));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("ECommerce"));

builder.Services.AddScoped<MongoDbSeeder>();

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
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Use Cases - Auth
builder.Services.AddScoped<AuthenticateWithGoogleUseCase>();
builder.Services.AddScoped<LoginWithEmailUseCase>();
builder.Services.AddScoped<RegisterWithEmailUseCase>();

// Use Cases - Catalog
builder.Services.AddScoped<GetCategoriesUseCase>();
builder.Services.AddScoped<GetCategoryUseCase>();
builder.Services.AddScoped<GetProductDetailsUseCase>();
builder.Services.AddScoped<GetPublishedProductsCase>();

// Use Cases - Checkout
builder.Services.AddScoped<CreateOrderUseCase>();
builder.Services.AddScoped<UpdateCartUseCase>();


builder.Services.AddControllers();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection("Google"));

// Autenticación: el backend valida el token (JWT) en las rutas con [Authorize]
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:15174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<CheckoutMutation>()
    .ModifyRequestOptions(options =>
    {
        options.IncludeExceptionDetails = true;
    });
var app = builder.Build();

// Errores con código y mensaje claros (en lugar de un 500 genérico)
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var (status, message) = error switch
    {
        EmailAlreadyRegisteredException => (409, "Ese correo ya está registrado."),
        InvalidCredentialsException or UserNotFoundException => (401, "Correo o contraseña incorrectos."),
        InvalidEmailException => (400, "El correo no es válido."),
        ArgumentException => (400, "Los datos enviados no son válidos."),
        _ => (500, "Error interno del servidor.")
    };

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new { message });
}));

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

using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider
            .GetRequiredService<MongoDbSeeder>();

        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudieron insertar los datos iniciales de MongoDB");
    }
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();