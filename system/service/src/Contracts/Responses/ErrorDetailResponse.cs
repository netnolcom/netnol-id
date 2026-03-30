namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Represents a descriptive response for service failures.
/// </summary>
/// <param name="Message">The explanation of the error.</param>
public record ErrorDetailResponse(string Message);