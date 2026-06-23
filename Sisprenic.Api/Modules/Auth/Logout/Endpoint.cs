using Microsoft.AspNetCore.Identity;

namespace Sisprenic.Api.Modules.Auth.Logout;

public static class LogoutEndpoint
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", Handle)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(SignInManager<IdentityUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.NoContent();
    }
}
