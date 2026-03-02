using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
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

    /// <summary>
    /// Factory method to reserve a booking.
    /// Calculates pricing and raises a domain event.
    /// </summary>
    public static Booking Reserve(
        Apartment apartment,
        Guid userId,
        DateRange duration,
        DateTime utcNow,
        PriceingService priceingService)
    {
        var pricingDetails = priceingService.CalculatePrice(apartment, duration);

        var booking = new Booking(
            Guid.CreateVersion7(),
            apartment.Id,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaningIpFee,
            pricingDetails.AmanitiesUpCharge,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            utcNow);

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        apartment.LastBookedOnUtc = utcNow;

        return booking;
    }

    /// <summary>Confirms a reserved booking.</summary>
    public Result Confirm(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Confirmed;
        ConfirmedOnUtc = utcNow;

        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Rejects a reserved booking.</summary>
    public Result Reject(DateTime utcNow)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Rejected;
        RejectedOnUtc = utcNow;

        RaiseDomainEvent(new BookingRejectedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Marks a confirmed booking as completed.</summary>
    public Result Complete(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        Status = BookingStatus.Completed;
        CompletedOnUtc = utcNow;

        RaiseDomainEvent(new BookingCompletedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Cancels a confirmed booking before it starts.
    /// </summary>
    public Result Cancel(DateTime utcNow)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        var currentDate = DateOnly.FromDateTime(utcNow);
        if (currentDate > Duration.Start)
            return Result.Failure(BookingErrors.AlreadyStarted);

        Status = BookingStatus.Cancelled;
        CompletedOnUtc = utcNow;

        RaiseDomainEvent(new BookingCancelledDomainEvent(Id));

        return Result.Success();
    }
}
