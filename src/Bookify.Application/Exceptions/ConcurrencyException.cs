namespace Bookify.Application.Exceptions;
/// <summary>
/// Custom exception used to wrap EF Core concurrency exceptions.
/// Keeps Infrastructure-specific exceptions from leaking into Application layer.
/// </summary>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}