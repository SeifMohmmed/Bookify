using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Booking.ReserveBooking;
public record ReserveBookingCommand(
    Guid ApartmentId,
    Guid UserId,
    DateOnly StartDate,
    DateOnly EndDate) : ICommand<Guid>;
