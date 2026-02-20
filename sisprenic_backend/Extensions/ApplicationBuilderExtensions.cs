using Microsoft.EntityFrameworkCore;
using sisprenic_backend.Endpoints.Clients;
using sisprenic_backend.Endpoints.Users;
using sisprenic.Database;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SisprenicContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapClientEndpoints().WithTags("Clients");
        app.MapUserEndpoints().WithTags("Users");
    }
}
