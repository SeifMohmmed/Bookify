namespace Bookify.Infrastructure.Outbox;
/// <summary>
/// Configuration options for the Outbox background processor.
/// Loaded from configuration (appsettings.json).
/// </summary>
internal class OutboxOptions
{
    public int IntervalInSeconds { get; init; } // Interval in seconds between job executions.

    public int BatchSize { get; init; }  // Number of messages processed per batch.

}
