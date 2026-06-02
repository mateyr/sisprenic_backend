using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

using Sisprenic.Api.Modules.Auth;
using Sisprenic.Api.Modules.Clients;
using Sisprenic.Api.Modules.Loans;
using Sisprenic.Api.Modules.Payments;
using Sisprenic.Api.Modules.Users;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Sisprenic.Api.Extensions;

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
