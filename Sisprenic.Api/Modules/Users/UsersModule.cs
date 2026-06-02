using Sisprenic.Api.Modules.Users.GetCurrentUser;

namespace Sisprenic.Api.Modules.Users;

public static class UsersModule
{
    public static void MapUsersModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");
        group.RequireAuthorization();

        group.MapGetCurrentUser();
    }
}
