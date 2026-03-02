using System.Diagnostics.CodeAnalysis;

namespace Bookify.Domain.Abstractions;
/// <summary>
/// Represents the result of an operation (success or failure).
/// </summary>
public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        // Success must have Error.None
        if (isSuccess && error != Error.None) throw new InvalidOperationException();

        // Failure must have a real error
        if (!isSuccess && error == Error.None) throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failure.</summary>
    public Error Error { get; }

    /// <summary>Create a successful result.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Create a failed result.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Create a successful result with a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>Create a failed result with a value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>Create a result from a nullable value.</summary>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is null ? Failure<TValue>(Error.NullValue) : Success(value);
}

/// <summary>
/// Represents the result of an operation that returns a value.
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if success; throws if failure.
    /// </summary>
    [NotNull]
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    /// <summary>
    /// Allows implicit conversion from TValue to Result&lt;TValue&gt;.
    /// </summary>
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}