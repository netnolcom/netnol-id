using System.Diagnostics;
using Netnol.Identity.Service.Infrastructure.Configuration;
using StackExchange.Redis;

namespace Netnol.Identity.Service.Infrastructure.Extensions;

public static class CacheServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        if (string.IsNullOrWhiteSpace(EnvironmentInitializer.CacheUri))
        {
            Console.WriteLine("No cache URL provided, using memory cache");
            services.AddDistributedMemoryCache();
        }
        else
        {
            Console.WriteLine("Cache URL provided, using Redis cache");
            services.AddStackExchangeRedisCache(x =>
                x.ConfigurationOptions = ConfigurationOptions.Parse(EnvironmentInitializer.CacheUri));
        }

        return services;
    }
}