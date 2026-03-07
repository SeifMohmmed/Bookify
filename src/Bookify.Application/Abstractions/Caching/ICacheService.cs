namespace Bookify.Application.Abstractions.Caching;

/// <summary>
/// Abstraction for a distributed caching service.
///
/// Provides methods to store, retrieve, and remove cached values.
/// This abstraction allows the application layer to use caching
/// without depending on a specific implementation such as Redis.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key.
    /// </summary>
    /// <returns>
    /// The cached value if found; otherwise <c>null</c>.
    /// </returns>
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a value in the cache.
    /// </summary>
    /// <typeparam name="T">Type of the object to cache.</typeparam>
    /// <param name="key">Unique cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">
    /// Optional expiration time for the cache entry.
    /// If not provided, a default expiration will be used.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    /// <param name="key">Unique cache key.</param>
    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

}
