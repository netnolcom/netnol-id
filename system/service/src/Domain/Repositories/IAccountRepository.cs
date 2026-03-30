using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Domain.Repositories;

/// <summary>
///     Defines the contract for account persistence operations.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    ///     Retrieves an account by its unique identifier.
    /// </summary>
    /// <param name="id">The account identification.</param>
    /// <returns>The account if found; otherwise, null.</returns>
    Task<Account?> GetByIdAsync(Identification id);

    /// <summary>
    ///     Retrieves an account by its username.
    /// </summary>
    /// <param name="username">The username value object.</param>
    /// <returns>The account if found; otherwise, null.</returns>
    Task<Account?> GetByUsernameAsync(Username username);

    /// <summary>
    ///     Checks if an account exists with the given username.
    /// </summary>
    /// <param name="username">The username value object.</param>
    /// <returns>True if exists; otherwise, false.</returns>
    Task<bool> ExistsByUsernameAsync(Username username);

    /// <summary>
    ///     Adds a new account to the persistence store.
    /// </summary>
    /// <param name="account">The account aggregate root.</param>
    Task AddAsync(Account account);

    /// <summary>
    ///     Updates an existing account.
    /// </summary>
    /// <param name="account">The account with modified data.</param>
    Task UpdateAsync(Account account);
}