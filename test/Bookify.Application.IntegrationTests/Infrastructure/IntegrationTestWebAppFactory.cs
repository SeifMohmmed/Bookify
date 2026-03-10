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
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Application.IntegrationTests.Infrastructure;
/// <summary>
/// Custom WebApplicationFactory used for integration testing.
/// It starts real infrastructure dependencies using Testcontainers:
/// - PostgreSQL database
/// - Redis cache
/// - Keycloak authentication server
/// </summary>
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    /// <summary>
    /// PostgreSQL container used for the test database
    /// </summary>
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithDatabase("bookify")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    /// <summary>
    /// Redis container used for caching during integration tests
    /// </summary>
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    /// <summary>
    /// Keycloak container used to simulate authentication server
    /// </summary>
    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithResourceMapping(
            new FileInfo(".files/bookify-realm-export.json"), // local realm config
            new FileInfo("/opt/keycloak/data/import/realm.json")) // container path
        .WithCommand("--import-realm") // import realm during startup
        .Build();

    /// <summary>
    /// Configure the web host for the integration test environment
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            #region Configure database container

            // Remove the existing DbContext registration
            // because it points to the real application database
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // Register DbContext using PostgreSQL test container
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            // Remove existing SQL connection factory
            services.RemoveAll(typeof(ISqlConnectionFactory));

            // Register connection factory pointing to test container
            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(_dbContainer.GetConnectionString()));

            #endregion


            #region Configure Redis container

            // Replace Redis connection string with the container connection
            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

            #endregion


            #region Configure Keycloak container

            // Retrieve base address of Keycloak container
            var keycloakAddress = _keycloakContainer.GetBaseAddress();

            // Configure authentication endpoints used by the application
            services.Configure<KeycloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
                o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
            });

            #endregion
        });
    }

    /// <summary>
    /// Starts the infrastructure containers before tests run
    /// </summary>
    public async Task InitializeAsync()
    {
        // Seed data could also be done here if needed
        // but currently seeding happens in Program.cs

        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();
    }

    /// <summary>
    /// Stops the containers after all tests finish
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _keycloakContainer.StopAsync();
    }
}