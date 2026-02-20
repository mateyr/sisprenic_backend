using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using sisprenic.Database;
using sisprenic.Entities;

using sisprenic_backend.Dtos.Users;

using static Microsoft.AspNetCore.Http.Results;

namespace sisprenic_backend.Endpoints.Users
{
    public static class UserTypedResults
    {
        public static async Task<IResult> GetCurrentUser(
            ClaimsPrincipal claimsPrincipal,
            SisprenicContext db
        )
        {
            string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            // TODO: Create the menu

            return TypedResults.Ok(new { userId, email });
        }
    }
}
