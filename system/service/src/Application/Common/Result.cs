namespace Netnol.Identity.Service.Application.Common;

/// <summary>
///     Represents the outcome of an operation that can either succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public readonly struct Result<T>
{
    /// <summary>
    ///     Indicates whether the operation succeeded.
    /// </summary>
    public readonly bool IsSuccess;

    /// <summary>
    ///     Gets the value produced by the operation.
    ///     Only valid when <see cref="IsSuccess" /> is true.
    /// </summary>
    public readonly T Value;

    /// <summary>
    ///     Gets the type of error that occurred, if the operation failed.
    ///     When <see cref="IsSuccess" /> is true, this will be <see cref="ErrorType.None" />.
    /// </summary>
    public readonly ErrorType ErrorType;

    /// <summary>
    ///     Gets a human-readable message describing the outcome.
    ///     For failures, it contains details about the error.
    ///     For successes, it may contain an optional informational message.
    /// </summary>
    public readonly string Message;

    private Result(bool isSuccess, T value, string message, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        ErrorType = errorType;
    }

    /// <summary>
    ///     Creates a successful result containing the given value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <param name="message">An optional informational message.</param>
    /// <returns>A result representing a successful operation.</returns>
    public static Result<T> FromSuccess(T value, string? message = null)
    {
        return new Result<T>(true, value, message ?? string.Empty, ErrorType.None);
    }

    /// <summary>
    ///     Creates a failed result with the specified error category and message.
    /// </summary>
    /// <param name="errorType">The type of error that occurred.</param>
    /// <param name="message">A description of the error.</param>
    /// <returns>A result representing a failed operation.</returns>
    public static Result<T> FromFailure(ErrorType errorType, string? message = null)
    {
        return new Result<T>(false, default!, message ?? string.Empty, errorType);
    }
}