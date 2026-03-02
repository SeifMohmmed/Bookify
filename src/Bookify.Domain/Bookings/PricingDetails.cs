using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;
public record PricingDetails(
    Money PriceForPeriod,
    Money CleaningIpFee,
    Money AmanitiesUpCharge,
    Money TotalPrice);
