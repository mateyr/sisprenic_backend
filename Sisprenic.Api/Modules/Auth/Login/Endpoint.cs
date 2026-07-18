using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Sisprenic.Api.Extensions;

namespace Sisprenic.Api.Modules.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Handle)
            .AllowAnonymous()
            .RequireRateLimiting(RateLimitingExtensions.AuthPolicy);
    }

    // Mirrors MapIdentityApi's "/login", trimmed to what this system needs: an httpOnly
    // cookie sign-in only (no bearer tokens, no cookie/session query toggles, no 2FA).
    private static async Task<Results<EmptyHttpResult, ValidationProblem, ProblemHttpResult>> Handle(
        LoginRequest login,
        IValidator<LoginRequest> validator,
        SignInManager<IdentityUser> signInManager,
        ILogger<LoginRequest> logger,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(login, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        SignInResult result = await signInManager.PasswordSignInAsync(
            login.UserName, login.Password, isPersistent: false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Login failed for user {UserName}: {Result}", login.UserName, result);

            return TypedResults.Problem("Invalid username or password.", statusCode: StatusCodes.Status401Unauthorized);
        }

        return TypedResults.Empty;
    }
}
