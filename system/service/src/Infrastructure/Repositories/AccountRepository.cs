using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.Repositories;
using Netnol.Identity.Service.Domain.ValueObjects;
using Netnol.Identity.Service.Infrastructure.Data;

namespace Netnol.Identity.Service.Infrastructure.Repositories;

/// <inheritdoc />
public class AccountRepository(DatabaseContext context, IDistributedCache cache) : IAccountRepository
{
    private const string DefaultCachedValue = "*";

    /// <inheritdoc />
    public async Task<Account?> GetByIdAsync(Identification id)
    {
        return await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByUsernameAsync(Username username)
    {
        return await GetCachedAccount(username);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUsernameAsync(Username username)
    {
        return await GetCachedAccount(username) != null;
    }

    /// <inheritdoc />
    public async Task AddAsync(Account account)
    {
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
        await UpdateCachedAccount(account.Username, account);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Account account)
    {
        context.Accounts.Attach(account);
        context.Entry(account).State = EntityState.Modified;
        await context.SaveChangesAsync();
        await UpdateCachedAccount(account.Username, account);
    }

    private async Task<Account?> GetCachedAccount(Username username)
    {
        var accountAsString = await cache.GetStringAsync(username.ToString());

        if (DefaultCachedValue.Equals(accountAsString))
        {
            Debug.WriteLine($"Get empty account from cache: {username}");
            return null;
        }

        if (!string.IsNullOrWhiteSpace(accountAsString))
        {
            Debug.WriteLine($"Get account from cache: {username}");
            return Account.Parse(accountAsString);
        }

        var account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);

        await UpdateCachedAccount(username, account);

        return account;
    }

    private async Task UpdateCachedAccount(Username username, Account? account)
    {
        Debug.WriteLine($"Save to cache: {username}, exists: {account != null}");
        await cache.SetStringAsync(username.ToString(), account == null ? DefaultCachedValue : account.ToString());
    }
}