using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;
public class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;

        var priceForPeriod = new Money(
            apartment.Price.Amount * period.LengthInDays,
            currency);

        decimal perentageUpCharge = 0;

        foreach (var amanity in apartment.Amanities)
        {
            perentageUpCharge += amanity switch
            {
                Amanity.GardenView or Amanity.MountainView => 0.05m,
                Amanity.AirConditioning => 0.01m,
                Amanity.Parking => 0.01m,
                _ => 0
            };
        }

        var amanitesUpCharge = Money.Zero();
        if (perentageUpCharge > 0)
        {
            amanitesUpCharge = new Money(
                priceForPeriod.Amount * perentageUpCharge,
                currency);
        }

        var totalPrice = Money.Zero(currency);

        totalPrice += priceForPeriod;

        if (!apartment.CleaningFee.IsZero())
        {
            totalPrice += apartment.CleaningFee;
        }

        totalPrice += amanitesUpCharge;

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amanitesUpCharge, totalPrice);
    }
}
