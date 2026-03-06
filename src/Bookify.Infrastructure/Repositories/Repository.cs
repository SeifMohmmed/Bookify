using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;
/// <summary>
/// Generic repository base class.
/// Provides common data access methods for entities.
/// </summary>
internal abstract class Repository<T> where T : Entity
{
    protected readonly ApplicationDbContext context;

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Retrieves an entity by its Id.
    /// Returns null if not found.
    /// </summary>
    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context
            .Set<T>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the context.
    /// Changes are persisted when UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    public virtual void Add(T entity)
    {
        context.Add(entity);
    }
}