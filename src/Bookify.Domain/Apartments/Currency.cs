namespace Bookify.Domain.Apartments;
/// <summary>
/// Represents a currency in the domain as a Value Object.
/// This is not just a string, but a strongly-typed concept in the domain model.
/// </summary>
public sealed record Currency
{
    internal static readonly Currency None = new(""); // Represents the absence of a currency.

    public static readonly Currency Usd = new("USD"); // US Dollar currency instance.

    public static readonly Currency Eur = new("EUR"); // Euro currency instance.

    /// <summary>
    /// Private constructor to enforce controlled creation.
    /// Prevents creating arbitrary currency instances from outside.
    /// </summary>
    /// <param name="code">ISO currency code</param>
    private Currency(string code) => Code = code;

    public string Code { get; init; } // The ISO currency code (e.g., USD, EUR).

    public static readonly IReadOnlyCollection<Currency> All = new[]  // Collection of all supported currencies in the system.
    {
        Usd,
        Eur
    };

    /// <summary>
    /// Creates a Currency from a given code.
    /// Throws if the code is not supported.
    /// </summary>
    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
            throw new ApplicationException("The currency code is invalid");
    }
}
