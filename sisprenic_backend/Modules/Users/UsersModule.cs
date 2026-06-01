using sisprenic_backend.Modules.Users.GetCurrentUser;

namespace sisprenic_backend.Modules.Users;

public static class UsersModule
{
    public static void MapUsersModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");
        group.RequireAuthorization();

        group.MapGetCurrentUser();
    }
}
