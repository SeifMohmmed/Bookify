using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Authorization;
using Bookify.Infrastructure.Caching;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        AddAuthentication(services, configuration);

        AddAuthorization(services);

        AddCaching(services, configuration);

        AddHealthChecks(services, configuration);

        return services;
    }

    private static void AddAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure authentication middleware
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Bind Authentication settings from configuration (appsettings.json)
        services.Configure<AuthenticationOptions>(
            configuration.GetSection("Authentication"));

        // Apply custom JWT options setup
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Bind the "Keycloak" section from appsettings.json to the KeycloakOptions class.
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));


        // Register the DelegatingHandler responsible for attaching the admin JWT token
        // to outgoing HTTP requests sent to the Keycloak Admin API.
        services.AddTransient<AdminAuthorizationDelegatingHandler>();


        // Register a typed HttpClient for the AuthenticationService.
        // This HttpClient will be used internally by AuthenticationService
        // to communicate with the Keycloak Admin REST API.
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpclient) =>
        {
            // Resolve Keycloak configuration values using IOptions
            var keycloakOptions = serviceProvider
                .GetRequiredService<IOptions<KeycloakOptions>>()
                .Value;

            // Set the base URL for all Keycloak Admin API requests
            // Example: http://bookify-idp:8080/auth/admin/realms/bookify/
            httpclient.BaseAddress = new Uri(keycloakOptions.AdminUrl);

        })
        // Add a DelegatingHandler to the HttpClient pipeline.
        // This handler automatically requests a Keycloak access token
        // and attaches it as a Bearer token to every outgoing request.
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        // HttpClient used to request JWT tokens from Keycloak
        services.AddHttpClient<IJwtService, JwtService>(
            (serviceProvider, httpclient) =>
            {
                var keycloakOptions = serviceProvider
                .GetRequiredService<IOptions<KeycloakOptions>>()
                .Value;

                httpclient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
            });

        // Register HttpContextAccessor so services can access the current HTTP context
        services.AddHttpContextAccessor();

        // Register IUserContext implementation for accessing the logged-in user identity
        services.AddScoped<IUserContext, UserContext>();

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

    /// <summary>
    /// Registers authorization services and claim transformations.
    /// </summary>
    private static void AddAuthorization(IServiceCollection services)
    {
        // Service used to retrieve roles from the database
        services.AddScoped<AuthorizationService>();

        // Adds custom claim transformation to enrich the ClaimsPrincipal
        services.AddTransient<
            Microsoft.AspNetCore.Authentication.IClaimsTransformation,
            CustomClaimTransformation>();

        // Registers the authorization handler responsible
        // for validating permission-based requirements.
        services.AddTransient<IAuthorizationHandler, PermissionAuthoizationHandler>();

        // Registers the custom policy provider that dynamically
        // generates authorization policies for permissions.
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    /// <summary>
    /// Registers Redis distributed caching and the application caching service.
    /// </summary>
    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        // Retrieve Redis connection string from configuration
        var connectionString = configuration.GetConnectionString("Cache") ??
                                throw new ArgumentNullException(nameof(configuration));

        // Register Redis distributed cache
        services.AddStackExchangeRedisCache(options =>
        options.Configuration = connectionString);

        // Register cache abstraction
        services.AddSingleton<ICacheService, CacheService>();
    }

    /// <summary>
    /// Registers health checks for external dependencies used by the application.
    /// These checks help monitoring tools determine the health of the system.
    /// </summary>
    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            // Checks connectivity to the PostgreSQL database
            .AddNpgSql(configuration.GetConnectionString("Database")!)

            // Checks connectivity to the Redis distributed cache
            .AddRedis(configuration.GetConnectionString("Cache")!)

            // Checks availability of the Keycloak identity server
            .AddUrlGroup(
                new Uri(configuration["KeyCloak:BaseUrl"]!),
                HttpMethod.Get,
                "keycloak");
    }
}
