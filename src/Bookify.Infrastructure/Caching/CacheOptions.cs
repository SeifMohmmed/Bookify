using Microsoft.Extensions.Caching.Distributed;

namespace Bookify.Infrastructure.Caching;
/// <summary>
/// Provides helper methods and default settings
/// for configuring distributed cache expiration policies.
/// </summary>
public static class CacheOptions
{
    /// <summary>
    /// Default cache expiration configuration.
    /// </summary>
    public static DistributedCacheEntryOptions DefaultExpiration = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    /// <summary>
    /// Creates cache entry options with a custom expiration time.
    /// If no expiration is provided, the default expiration is used.
    /// </summary>
    /// <param name="expiration">Optional expiration duration.</param>
    /// <returns>Configured <see cref="DistributedCacheEntryOptions"/>.</returns>
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration = null) =>
        expiration is not null ?
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } :
            DefaultExpiration;
}
