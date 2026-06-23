using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Sisprenic.Api.Modules.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Handle)
            .AllowAnonymous();
    }

    // Mirrors MapIdentityApi's "/login", trimmed to what this system needs: an httpOnly
    // cookie sign-in only (no bearer tokens, no cookie/session query toggles, no 2FA).
    private static async Task<Results<EmptyHttpResult, ProblemHttpResult>> Handle(
        LoginRequest login,
        SignInManager<IdentityUser> signInManager)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        SignInResult result = await signInManager.PasswordSignInAsync(
            login.UserName, login.Password, isPersistent: false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        return TypedResults.Empty;
    }
}
