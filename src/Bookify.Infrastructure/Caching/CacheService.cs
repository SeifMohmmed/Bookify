using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;

namespace Bookify.Infrastructure.Caching;
/// <summary>
/// Redis-based implementation of <see cref="ICacheService"/>.
/// 
/// This service serializes objects into JSON before storing them
/// in the distributed cache and deserializes them when retrieving.
/// </summary>
internal class CacheService
    (IDistributedCache cache) : ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key.
    /// </summary>
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    /// <summary>
    /// Stores a value in the distributed cache.
    /// </summary>
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken); ;
    }

    /// <summary>
    /// Removes a cached value.
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    /// <summary>
    /// Deserializes cached JSON bytes into the specified type.
    /// </summary>
    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    /// <summary>
    /// Serializes an object into JSON bytes for storage in cache.
    /// </summary>
    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer, value);

        return buffer.WrittenSpan.ToArray();
    }
}
