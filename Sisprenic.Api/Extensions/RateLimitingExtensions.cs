using System.Threading.RateLimiting;

namespace Sisprenic.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string AuthPolicy = "auth";
    public const string PdfPolicy = "pdf";

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Baseline protection for every endpoint, keyed by client IP.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetPartitionKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 100,
                        QueueLimit = 0
                    }));

            // Tighter limit on login/register to slow down credential stuffing and brute force.
            options.AddPolicy(AuthPolicy, context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    GetPartitionKey(context),
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        PermitLimit = 5,
                        QueueLimit = 0
                    }));

            // Limits concurrent renders to prevent overloading the shared browser.
            options.AddPolicy(PdfPolicy, context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    GetPartitionKey(context),
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        PermitLimit = 10,
                        QueueLimit = 0
                    }));
        });

        return services;
    }

    public static IApplicationBuilder UseApiRateLimiting(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        return app;
    }

    private static string GetPartitionKey(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
