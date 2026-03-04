using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews.Events;
using Bookify.Domain.Reviews.ValueObjects;

namespace Bookify.Domain.Reviews;
public class Review : Entity
{
    private Review(
        Guid id,
        Guid apartmentId,
        Guid bookingId,
        Guid userId,
        Rating rating,
        string comment,
        DateTime createdOnUtc) : base(id)
    {
        ApartmentId = apartmentId;
        BookingId = bookingId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
        CreatedOnUtc = createdOnUtc;
    }

    private Review()
    {

    }
    public Guid ApartmentId { get; }
    public Guid BookingId { get; }
    public Guid UserId { get; }
    public Rating Rating { get; }
    public string Comment { get; }
    public DateTime CreatedOnUtc { get; }

    public static Result<Review> Create(
        Booking booking,
        Rating rating,
        string comment,
        DateTime createdOnUtc)
    {
        if (booking.Status != BookingStatus.Completed)
            return Result.Failure<Review>(ReviewErrors.NotEligible);

        var review = new Review(
            Guid.NewGuid(),
            booking.ApartmentId,
            booking.Id,
            booking.UserId,
            rating,
            comment,
            createdOnUtc);

        review.RaiseDomainEvent(new ReviewCreatedDomainEvent(review.Id));

        return review;
    }
}