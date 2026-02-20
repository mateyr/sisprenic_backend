using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static sisprenic_backend.Endpoints.Users.UserTypedResults;

namespace sisprenic_backend.Endpoints.Users
{
    public static class UserEndpoints
    {
        public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
        {
            var route = app.MapGroup("/users");
            route.RequireAuthorization();
            route.MapGet("/me", GetCurrentUser);
            return route;
        }
    }
}
