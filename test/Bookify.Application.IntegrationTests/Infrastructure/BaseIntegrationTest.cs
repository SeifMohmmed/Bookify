using Bookify.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application.IntegrationTests.Infrastructure;
/// <summary>
/// Base class for all integration tests.
/// Provides common infrastructure like DI scope,
/// MediatR sender, and database context.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    // Creates a service scope so each test has its own dependency scope
    private readonly IServiceScope _serviceScope;

    // Used to send commands and queries through MediatR
    // This simulates real application behavior
    protected readonly ISender Sender;

    // Database context used for assertions and data verification
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory webAppFactory)
    {
        // Create a new DI scope from the test web application factory
        _serviceScope = webAppFactory.Services.CreateScope();

        // Resolve MediatR sender to execute commands/queries
        Sender = _serviceScope.ServiceProvider.GetRequiredService<ISender>();

        // Resolve the application's DbContext
        // Used to verify database state during tests
        DbContext = _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        // TODO:
        // When running multiple integration tests in parallel,
        // Quartz may throw:
        // Quartz.SchedulerException : Scheduler with name 'QuartzScheduler' already exists.
        // Possible solutions:
        // - Disable Quartz in test environment
        // - Use unique scheduler names
        // - Run tests sequentially
    }
}