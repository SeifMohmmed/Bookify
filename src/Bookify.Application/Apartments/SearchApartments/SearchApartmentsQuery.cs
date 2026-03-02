using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Apartments.SearchApartments;
/// <summary>
/// Retrieves available apartments within a given date range.
/// </summary>
/// <param name="StartDate">Reservation start date.</param>
/// <param name="EndDate">Reservation end date.</param>
public record SearchApartmentsQuery(
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<IReadOnlyList<ApartmentResponse>>;
