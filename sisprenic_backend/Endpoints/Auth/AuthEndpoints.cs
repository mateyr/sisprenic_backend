using System.Net;

using Microsoft.AspNetCore.Identity;

namespace sisprenic_backend.Endpoints.Auth;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Authentication");

        return app;
    }
}
