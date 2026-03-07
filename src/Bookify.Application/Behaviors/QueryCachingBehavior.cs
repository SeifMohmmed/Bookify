using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Behaviors;
/// <summary>
/// MediatR pipeline behavior responsible for caching query results.
/// </summary>
/// <remarks>
/// This behavior intercepts queries implementing <see cref="ICachedQuery"/> and:
/// 1. Attempts to retrieve the result from the cache.
/// 2. If found, returns the cached result.
/// 3. If not found, executes the query handler and stores the result in the cache.
/// </remarks>
internal sealed class QueryCachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<QueryCachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    /// <summary>
    /// Handles the request and applies caching logic.
    /// </summary>
    /// <param name="request">The incoming query request.</param>
    /// <param name="next">Delegate to invoke the next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly generated query result.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Attempt to retrieve the response from the cache
        var cachedResult = await cacheService.GetAsync<TResponse>(
            request.CacheKey, cancellationToken);

        var name = typeof(TRequest).Name;

        // If a cached result exists, return it immediately
        if (cachedResult is not null)
        {
            logger.LogInformation("Cache hit for {Query}", name);

            return cachedResult;
        }

        logger.LogInformation("Cache miss for {Query}", name);

        // Execute the next handler in the pipeline
        var result = await next();

        // Cache the result only if the operation succeeded
        if (result.IsSuccess)
            await cacheService.SetAsync(request.CacheKey, result, request.Expiration, cancellationToken);

        return result;
    }
}