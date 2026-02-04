using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

namespace sisprenic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connString = configuration.GetConnectionString("Sisprenic");

        services.AddDbContext<SisprenicContext>(options =>
            options.UseNpgsql(connString).UseSnakeCaseNamingConvention());

        return services;
    }
}
