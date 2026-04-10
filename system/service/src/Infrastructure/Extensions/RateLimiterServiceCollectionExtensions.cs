using System.Globalization;
using System.Threading.RateLimiting;

namespace Netnol.Identity.Service.Infrastructure.Extensions;

public static class RateLimiterServiceCollectionExtensions
{
    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
    {
        const int maxRequests = 50;
        var windowTimer = TimeSpan.FromMinutes(5);

        services.AddRateLimiter(x =>
        {
            x.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
            {
                var client = http.Request.Headers["True-Client-IP"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(client))
                    client = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(client, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = maxRequests,
                    Window = windowTimer
                });
            });

            x.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            x.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = $"{windowTimer.TotalSeconds}";
                context.HttpContext.Response.Headers.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync(
                    "Rate limit exceeded. Please wait a few minutes before retrying", token);
            };
        });

        return services;
    }
}