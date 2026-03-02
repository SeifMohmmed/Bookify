using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;
public interface IBookingRepository
{
    Task<Booking> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an apartment has overlapping bookings
    /// within a given date range.
    /// </summary>
    Task<bool> IsOverlappingAsync(
    Apartment apartment,
    DateRange duration,
    CancellationToken cancellationToken = default);

    void Add(Booking booking);

}
