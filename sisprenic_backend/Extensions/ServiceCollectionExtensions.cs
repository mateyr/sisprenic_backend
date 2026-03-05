using Microsoft.EntityFrameworkCore;

using sisprenic.Database;
using sisprenic.Entities;

using sisprenic_backend.Database;

namespace sisprenic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connString = configuration.GetConnectionString("Sisprenic");

        services.AddDbContext<SisprenicContext>(options =>
            options.UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .UseSeeding(async (context, _) =>
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
