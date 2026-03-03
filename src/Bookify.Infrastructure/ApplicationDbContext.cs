using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    // MediatR publisher used to dispatch domain events
    private readonly IPublisher _publisher;

    public ApplicationDbContext(
        DbContextOptions options,
        IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
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
        // Persist changes first (transaction happens here)
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish domain events after successful commit
        await PublishDomainEventsAsync();

        return result;
    }

    /// <summary>
    /// Collects all domain events from tracked entities
    /// and publishes them via MediatR.
    /// </summary>
    private async Task PublishDomainEventsAsync()
    {
        /*
          1️) Get all tracked entities that inherit from Entity base class.
          2️) Extract their domain events.
          3️) Clear events from entity (to prevent duplicate publishing).
          4️) Publish each event using MediatR.
        */

        var domainEvents = ChangeTracker
            .Entries<Entity>() // All tracked domain entities
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents(); // Prevent duplicate dispatch

                return domainEvents;
            })
            .ToList();

        // Publish events one by one
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
