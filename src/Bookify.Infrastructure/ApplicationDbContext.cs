using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions options,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically apply IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Saves changes to the database.
    /// After successful persistence, domain events are published.
    /// </summary>
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            // Persist changes first (transaction happens here)
            var result = await base.SaveChangesAsync(cancellationToken);

            return result;

        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Wrap EF-specific exception
            // Prevent infrastructure leakage
            throw new ConcurrencyException(
                "Concurrency exception ocurred.",
                ex);
        }

    }

    /// <summary>
    /// Collects domain events from entities and stores them
    /// as Outbox messages before saving to the database.
    /// This ensures reliable event publishing.
    /// </summary>
    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<Entity>() // All tracked domain entities
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                // Clear events to avoid publishing duplicates
                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.CreateVersion7(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        // Store events in the Outbox table
        AddRange(outboxMessages);
    }
}
