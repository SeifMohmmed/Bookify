namespace Bookify.Domain.Shared;
/// <summary>
/// Represents a monetary value in the domain.
/// Money is a Value Object composed of an amount and a currency.
/// </summary>
public record Money(decimal Amount, Currency Currency)
{
    /// <summary>
    /// Adds two Money objects together.
    /// Both must have the same currency.
    /// </summary>
    public static Money operator +(Money first, Money second)
    {
        // Ensure both money values use the same currency
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        // Return a new Money instance with summed amount
        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money Zero() => new(0, Currency.None); // Represents zero money with no currency.

    public static Money Zero(Currency currency) => new(0, currency);

    public bool IsZero() => this == Zero(Currency);
}
