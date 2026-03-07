using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Abstractions.Caching;
/// <summary>
/// Represents a query that supports caching.
/// Inherits from <see cref="IQuery{TResponse}"/> and adds caching metadata.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

/// <summary>
/// Defines caching configuration for a query.
/// Implementations provide a cache key and optional expiration time.
/// </summary>
public interface ICachedQuery
{
    string CacheKey { get; }  // Gets the cache key used to store and retrieve the cached result.

    /// <summary>
    /// Gets the cache expiration duration.
    /// If null, the cache entry will use the default expiration policy.
    /// </summary>
    TimeSpan? Expiration { get; }
}