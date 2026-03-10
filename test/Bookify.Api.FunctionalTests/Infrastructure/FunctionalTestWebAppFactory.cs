using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Api.FunctionalTests.Infrastructure;
/// <summary>
/// Custom WebApplicationFactory used to configure the application for functional testing.
/// It replaces real infrastructure with Testcontainers (PostgreSQL, Redis, Keycloak).
/// </summary>
public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    /// <summary>
    /// PostgreSQL container used as the database for tests.
    /// </summary>
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("bookify")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    /// <summary>
    /// Redis container used for caching during tests.
    /// </summary>
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    /// <summary>
    /// Keycloak container used to simulate the authentication server.
    /// A predefined realm is imported on startup.
    /// </summary>
    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithResourceMapping(
            new FileInfo(".files/bookify-realm-export.json"), // Local realm configuration
            new FileInfo("/opt/keycloak/data/import/realm.json")) // Container path
        .WithCommand("--import-realm") // Import realm during container startup
        .Build();

    /// <summary>
    /// Configure services for the test environment.
    /// Replaces real infrastructure dependencies with containerized services.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            #region Configure database container

            // Remove the application's existing DbContext configuration
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // Register DbContext using PostgreSQL container
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            // Remove the default SQL connection factory
            services.RemoveAll(typeof(ISqlConnectionFactory));

            // Register a connection factory pointing to the test PostgreSQL container
            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(_dbContainer.GetConnectionString()));

            #endregion


            #region Configure Redis container

            // Replace Redis configuration with the Redis test container connection
            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

            #endregion


            #region Configure Keycloak container

            // Retrieve Keycloak base address
            var keycloakAddress = _keycloakContainer.GetBaseAddress();

            // Configure Keycloak endpoints used by the application
            services.Configure<KeycloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
                o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
            });

            #endregion

            // Configure authentication metadata for validating tokens
            services.Configure<AuthenticationOptions>(o =>
            {
                o.ValidIssuer = $"{keycloakAddress}realms/bookify/";
                o.MetadataUrl = $"{keycloakAddress}realms/bookify/.well-known/openid-configuration";
            });

        });
    }

    /// <summary>
    /// Called before any tests run.
    /// Starts all required containers and initializes test data.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();

        await InitializeTestUserAsync();
    }

    /// <summary>
    /// Called after all tests complete.
    /// Stops and cleans up all containers.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _keycloakContainer.StopAsync();
    }

    /// <summary>
    /// Creates a default test user in the system.
    /// This user is used by functional tests that require authentication.
    /// </summary>
    private async Task InitializeTestUserAsync()
    {
        var httpClient = CreateClient();

        // Register the predefined test user
        await httpClient.PostAsJsonAsync("v1/users/register", UserData.RegisterTestUserRequest);
    }
}