namespace Netnol.Identity.Service.Infrastructure.Configuration;

/// <summary>Infrastructure synchronization for the application hosting environment.</summary>
public static class EnvironmentInitializer
{
    /// <summary>Gets the database connection URI</summary>
    public static string? DatabaseUri { get; private set; }

    /// <summary>Gets the cache connection URI</summary>
    public static string? CacheUri { get; private set; }

    /// <summary>Gets the application environment name</summary>
    public static string? Environment { get; private set; }

    /// <summary>Gets the configured host</summary>
    public static string? Host { get; private set; }

    /// <summary>Prepares the host environment settings for the service startup.</summary>
    public static void Initialize()
    {
        _ = DotNetEnv.Env.Load();

        Environment = System.Environment.GetEnvironmentVariable("ENVIRONMENT")?.Trim();
        Host = System.Environment.GetEnvironmentVariable("HOST")?.Trim().ToLower();
        DatabaseUri = System.Environment.GetEnvironmentVariable("DATABASE_URI")?.Trim();
        CacheUri = System.Environment.GetEnvironmentVariable("CACHE_URI")?.Trim();

        if (!string.IsNullOrWhiteSpace(Environment))
        {
            System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", Environment);
            System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environment);
        }

        if (!string.IsNullOrWhiteSpace(Host))
            System.Environment.SetEnvironmentVariable("ASPNETCORE_URLS",
                Host.Contains("://") ? Host : $"http://{Host}");
    }
}