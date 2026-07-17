using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;
using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connString = configuration.GetConnectionString("Sisprenic");

        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<SisprenicContext>((sp, options) =>
            options.UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Menu>().Any())
                {
                    context.Set<Menu>().AddRange(MenuSeed.Menu);
                    context.SaveChanges();
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (!context.Set<Menu>().Any())
                {
                    context.Set<Menu>().AddRange(MenuSeed.Menu);
                    await context.SaveChangesAsync();
                }
            }));

        return services;
    }
}
