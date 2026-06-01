using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Modules.Auth;
using sisprenic_backend.Modules.Clients;
using sisprenic_backend.Modules.Loans;
using sisprenic_backend.Modules.Payments;
using sisprenic_backend.Modules.Users;

using Swashbuckle.AspNetCore.SwaggerUI;

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
            options.DocExpansion(DocExpansion.None);
        });

        return app;
    }

    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SisprenicContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<SisprenicContext>();
        await AdminSeeder.SeedAsync(userManager, dbContext);
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapUsersModule();
        app.MapClientsModule();
        app.MapLoansModule();
        app.MapPaymentsModule();
        app.MapAuthModule();
    }
}
