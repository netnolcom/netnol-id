namespace Netnol.Identity.Service.Application.Common;

/// <summary>
///     Represents the possible categories of application errors.
/// </summary>
public enum ErrorType
{
    /// <summary>
    ///     No error (success state).
    /// </summary>
    None = 0,

    /// <summary>
    ///     The request contains invalid data or malformed syntax.
    /// </summary>
    InvalidInput = 400,

    /// <summary>
    ///     The requested resource was not found.
    /// </summary>
    ResourceNotFound = 404,

    /// <summary>
    ///     The resource already exists and cannot be created again.
    /// </summary>
    AlreadyExists = 409,

    /// <summary>
    ///     The client must authenticate itself to get the requested response.
    /// </summary>
    AuthenticationRequired = 401,

    /// <summary>
    ///     The client does not have sufficient permission to perform the operation.
    /// </summary>
    PermissionDenied = 403,

    /// <summary>
    ///     An unexpected internal error occurred that cannot be classified.
    /// </summary>
    Unexpected = 500
}