using Netnol.Identity.Service.Application.Common;

namespace Netnol.Identity.Service.Contracts.Common;

/// <summary>
///     Represents a descriptive response for service failures.
/// </summary>
/// <param name="Message">A detailed description providing insights into the error's cause.</param>
public record ErrorMessageResponse(string Message)
{
    /// <summary>
    /// Creates an <see cref="ErrorMessageResponse"/> from the provided <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The result from which the error message will be constructed.</param>
    /// <typeparam name="T">The type of the value within the provided result.</typeparam>
    /// <returns>An instance of <see cref="ErrorMessageResponse"/> containing the error type and message from the result.</returns>
    public static ErrorMessageResponse FromResult<T>(Result<T> result)
    {
        return new ErrorMessageResponse($"[{result.ErrorType.ToString().ToUpper()}] {result.Message}");
    }
}