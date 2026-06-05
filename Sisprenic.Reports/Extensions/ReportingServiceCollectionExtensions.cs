using Microsoft.Extensions.DependencyInjection;

using Sisprenic.Reports.Abstractions;
using Sisprenic.Reports.Infrastructure;

namespace Sisprenic.Reports.Extensions;


public static class ReportingServiceCollectionExtensions
{
    public static IServiceCollection AddReporting(this IServiceCollection services)
    {
        services.AddRazorTemplating();

        // Keeps a single Chromium instance alive and reuses it.
        services.AddSingleton<PlaywrightBrowserProvider>();

        // One report renderer per request.
        services.AddScoped<IReportRenderer, ReportRenderer>();

        return services;
    }
}
