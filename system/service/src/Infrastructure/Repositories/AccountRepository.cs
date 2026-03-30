using Microsoft.EntityFrameworkCore;
using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.Repositories;
using Netnol.Identity.Service.Domain.ValueObjects;
using Netnol.Identity.Service.Infrastructure.Data;

namespace Netnol.Identity.Service.Infrastructure.Repositories;

/// <inheritdoc />
public class AccountRepository(DatabaseContext context) : IAccountRepository
{
    /// <inheritdoc />
    public async Task<Account?> GetByIdAsync(Identification id)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByUsernameAsync(Username username)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Username == username);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUsernameAsync(Username username)
    {
        return await context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.Username == username);
    }

    /// <inheritdoc />
    public async Task AddAsync(Account account)
    {
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Account account)
    {
        context.Accounts.Attach(account);
        context.Entry(account).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}