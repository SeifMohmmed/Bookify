using Bookify.Application.Abstractions.Data;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
/// <summary>
/// Custom health check used to verify that the application's primary database
/// connection is working correctly.
/// </summary>
public class CustomSqlHealthCheck(
    ISqlConnectionFactory sqlConnectionFactory) : IHealthCheck
{
    /// <summary>
    /// Executes the health check by attempting a simple query against the database.
    /// If the query succeeds, the database is considered healthy.
    /// </summary>
    /// <param name="context">Provides contextual information about the health check.</param>
    /// <param name="cancellationToken">Token used to cancel the health check operation.</param>
    /// <returns>
    /// A <see cref="HealthCheckResult"/> indicating whether the database is healthy or unhealthy.
    /// </returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create a database connection using the application's SQL connection factory
            using var connection = sqlConnectionFactory.CreateConnection();

            // Execute a lightweight query to ensure the database is reachable
            await connection.ExecuteScalarAsync("SELECT 1;");

            // If the query succeeds, the database is considered healthy
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            // If any exception occurs, the database is considered unhealthy
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}