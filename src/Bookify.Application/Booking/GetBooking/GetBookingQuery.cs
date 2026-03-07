using Bookify.Application.Abstractions.Caching;

namespace Bookify.Application.Booking.GetBooking;
/// <summary>
/// Query used to retrieve a booking by its unique identifier.
/// This query supports caching to improve read performance.
/// </summary>
/// <param name="BookingId">The unique identifier of the booking.</param>
public record GetBookingQuery(Guid BookingId) : ICachedQuery<BookingResponse>
{
    /// <summary>
    /// Unique cache key used to store the booking in the cache.
    /// </summary>
    public string CacheKey => $"bookings-{BookingId}";

    /// <summary>
    /// Cache expiration time.
    /// Returning null means the default cache expiration will be used.
    /// </summary>
    public TimeSpan? Expiration => null;
}