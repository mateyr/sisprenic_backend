using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Extensions;

namespace Sisprenic.Api.Modules.Auth.Register;

public static class RegisterEndpoint
{
    public static void MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", Handle)
            .RequireAuthorization(Permissions.Users.Create)
            .RequireRateLimiting(RateLimitingExtensions.AuthPolicy);
    }

    // Unlike MapIdentityApi's /register
    // it is never anonymous and skips the email-confirmation flow.
    private static async Task<Results<Ok, ValidationProblem>> Handle(
        RegisterRequest registration,
        IValidator<RegisterRequest> validator,
        UserManager<IdentityUser> userManager,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(registration, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        IdentityUser user = new() { UserName = registration.UserName, Email = registration.Email };

        IdentityResult result = await userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        Dictionary<string, string[]> errors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Description).ToArray());

        return TypedResults.ValidationProblem(errors);
    }
}
