namespace Bookify.Infrastructure.Outbox;
/// <summary>
/// Represents a domain event stored in the database
/// using the Transactional Outbox pattern.
/// 
/// Instead of publishing events directly,
/// events are stored here and processed later by a background job.
/// </summary>
public sealed class OutboxMessage
{
    public OutboxMessage(
        Guid id,
        DateTime occurredOnUtc,
        string type,
        string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Time when the domain event occurred.
    /// </summary>
    public DateTime OccurredOnUtc { get; init; }

    /// <summary>
    /// Name of the domain event type.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Serialized domain event payload (JSON).
    /// </summary>
    public string Content { get; init; }

    /// <summary>
    /// Timestamp when the message was successfully processed.
    /// Null means it has not been processed yet.
    /// </summary>
    public DateTime? ProcessedOnUtc { get; init; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? Error { get; init; }
}