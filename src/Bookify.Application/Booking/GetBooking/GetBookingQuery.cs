using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Booking.GetBooking;
/// <summary>
/// Retrieves a booking by its ID.
/// </summary>
/// <param name="BookingId">Booking unique identifier.</param>
public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;