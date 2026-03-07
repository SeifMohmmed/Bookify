using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Data;

namespace Bookify.Infrastructure.Outbox;
/// <summary>
/// Quartz background job responsible for processing
/// pending Outbox messages and publishing domain events.
/// </summary>
internal class ProcessOutboxMessagesJob(
    ISqlConnectionFactory sqlConnectionFactory,
    IPublisher publisher,
    OutboxOptions outboxOptions,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessOutboxMessagesJob> logger) : IJob
{
    // JSON serializer settings used to restore domain event types.
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    /// <summary>
    /// Quartz job execution entry point.
    /// Fetches unprocessed messages and publishes them.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Begining to process outbox messages");

        using var connection = sqlConnectionFactory.CreateConnection();

        using var transaction = connection.BeginTransaction();

        // Fetch unprocessed outbox messages
        var outboxMessages = await GetOutboxMessageAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                // Deserialize stored event
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings);

                // Publish event using MediatR
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            // Mark message as processed
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        logger.LogInformation("Completed processing outbox messages");
    }

    /// <summary>
    /// Retrieves a batch of unprocessed Outbox messages.
    /// </summary>
    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $"""
            SELECT id, content
            FROM outbox_messages
            WHERE processed_on_utc IS NULL
            ORDER BY occured_on_utc
            LIMIT {outboxOptions.BatchSize}
            FOR UPDATE
            """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction);

        return outboxMessages.ToList();
    }

    /// <summary>
    /// Updates Outbox message after processing.
    /// Marks it as processed and stores any error.
    /// </summary>
    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error=@Error
            WHERE id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProceessedOnUtc = dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction);
    }

    /// <summary>
    /// Lightweight projection used when reading Outbox messages.
    /// </summary>
    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
