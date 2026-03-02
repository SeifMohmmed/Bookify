namespace Bookify.Application.Abstractions.Clock;
/// <summary>
/// Abstraction for retrieving the current UTC date and time.
///
/// Why not use DateTime.UtcNow directly?
/// - Improves testability (can mock time in unit tests)
/// - Avoids tight coupling to system clock
/// - Enables deterministic testing for time-based logic
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
