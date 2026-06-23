using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Sisprenic.Api.Authorization;

namespace Sisprenic.Api.Modules.Auth.Register;

public static class RegisterEndpoint
{
    public static void MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", Handle)
            .RequireAuthorization(Permissions.Users.Create);
    }

    // Unlike MapIdentityApi's /register
    // it is never anonymous and skips the email-confirmation flow.
    private static async Task<Results<Ok, ValidationProblem>> Handle(
        RegisterRequest registration,
        UserManager<IdentityUser> userManager)
    {
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
