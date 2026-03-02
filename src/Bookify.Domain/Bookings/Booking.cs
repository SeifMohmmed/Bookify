using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;
public sealed class Booking : Entity
{
    private Booking(
        Guid id,
        Guid aparetmentId,
        Guid userId,
        DateRange duration,
        Money priceForPeriod,
        Money cleaningFee,
        Money amanitiesUpCharge,
        Money totalPrice,
        BookingStatus status,
        DateTime createdOnUtc)
        : base(id)
    {
        ApartmentId = aparetmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaningFee = cleaningFee;
        AmenitiesUpChange = amanitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid ApartmentId { get; private set; }

    public Guid UserId { get; private set; }

    public DateRange Duration { get; private set; }

    public Money PriceForPeriod { get; private set; }

    public Money CleaningFee { get; private set; }

    public Money AmenitiesUpChange { get; private set; }

    public Money TotalPrice { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? ConfirmedOnUtc { get; private set; }

    public DateTime? RejectedOnUtc { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public DateTime? CanceledOnUtc { get; private set; }

    public static Booking Reserve(
        Guid apartmentId,
        Guid userId,
        DateRange duration,
        DateTime utcNow,
        PricingDetails pricingDetails)
    {
        var booking = new Booking(
            Guid.CreateVersion7(),
            apartmentId,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaningIpFee,
            pricingDetails.AmanitiesUpCharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            utcNow);

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        return booking;
    }
}
