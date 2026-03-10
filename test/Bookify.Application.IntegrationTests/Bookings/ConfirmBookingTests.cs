using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Bookings;
public class ConfirmBookingTests : BaseIntegrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();

    public ConfirmBookingTests(IntegrationTestWebAppFactory webAppFactory)
        : base(webAppFactory)
    {
    }

    [Fact]
    public async Task ConfirmBooking_ShouldReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var command = new GetBookingQuery(BookingId);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(BookingErrors.NotFound);
    }
}
