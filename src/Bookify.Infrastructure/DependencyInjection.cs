using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services such as
    /// database, repositories, external services, etc.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DateTime provider abstraction
        // Used to avoid direct dependency on DateTime.UtcNow
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // Register Email service implementation
        services.AddTransient<IEmailService, EmailService>();

        // Register database and repositories
        AddPersistence(services, configuration);

        // Configure authentication middleware
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Bind Authentication settings from configuration (appsettings.json)
        services.Configure<AuthenticationOptions>(
            configuration.GetSection("Authentication"));

        // Apply custom JWT options setup
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        return services;
    }

    /// <summary>
    /// Registers persistence layer services
    /// (EF Core, repositories, Dapper)
    /// </summary>
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {

        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        // Register EF Core DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        // Register repositories (Scoped per request)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();

        // Register Unit of Work
        // ApplicationDbContext implements IUnitOfWork
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Register SQL connection factory for Dapper usage (Query side)
        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        // Register custom Dapper type handler globally
        // This ensures DateOnly works across all Dapper queries
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }
}
