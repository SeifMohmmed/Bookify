using MediatR;

namespace Bookify.Domain.Abstractions;
/// <summary>
/// Marker interface for domain events.
/// 
/// Domain events represent something important that happened
/// inside the domain model (e.g., BookingCreated, ApartmentReserved).
///
/// Inherits from MediatR's INotification so events can be dispatched
/// through MediatR without coupling the domain to infrastructure logic.
/// </summary>
public interface IDomainEvent : INotification
{
}
