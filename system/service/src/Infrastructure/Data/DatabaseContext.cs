using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Netnol.Identity.Core;
using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.ValueObjects;
using Netnol.Identity.Service.Infrastructure.Configuration;
using Npgsql;
using StringQueryMap;

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
            options.UseSqlite("Data Source=netnol-id.sqlite");
        }
        else
        {
            options.UseNpgsql(new NpgsqlConnectionStringBuilder(EnvironmentInitializer.DatabaseUri).ConnectionString);
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Account>(builder =>
        {
            builder.Property(x => x.Id).HasConversion(
                identification => identification.ToString(),
                value => Identification.Parse(value));

            builder.Property(x => x.Username).HasConversion(
                username => username.ToString(),
                value => Username.Parse(value));

            builder.Property(x => x.Keys).HasConversion(
                keys => keys.ToString(),
                value => KeyPair.Parse(value));

            builder.Property(x => x.Seed).HasConversion(
                seed => seed.ToString(),
                value => Seed.Parse(value));

            builder.Property(x => x.Password).HasConversion(
                password => password.ToString(),
                value => Password.Parse(value)
            );

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Username).IsUnique();
        });
    }
}