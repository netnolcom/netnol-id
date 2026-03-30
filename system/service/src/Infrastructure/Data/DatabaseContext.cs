using Microsoft.EntityFrameworkCore;
using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Infrastructure.Configuration;
using Npgsql;

namespace Netnol.Identity.Service.Infrastructure.Data;

/// <inheritdoc />
public class DatabaseContext : DbContext
{
    /// <summary>
    ///     <see cref="DatabaseContext.Accounts" />
    /// </summary>
    public DbSet<Account> Accounts { get; set; }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (string.IsNullOrWhiteSpace(EnvironmentInitializer.DatabaseUri))
        {
            options.UseSqlite("Data Source=netnol-id.db");
        }
        else
        {
            options.UseNpgsql(new NpgsqlConnectionStringBuilder(EnvironmentInitializer.DatabaseUri).ConnectionString);
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder model)
    {
    }
}