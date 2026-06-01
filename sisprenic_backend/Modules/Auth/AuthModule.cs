using sisprenic_backend.Modules.Auth.Logout;

namespace sisprenic_backend.Modules.Auth;

public static class AuthModule
{
    public static void MapAuthModule(this IEndpointRouteBuilder app)
    {
        app.MapLogout();
    }
}
