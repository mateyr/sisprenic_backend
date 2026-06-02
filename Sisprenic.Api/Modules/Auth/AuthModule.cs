using Sisprenic.Api.Modules.Auth.Logout;

namespace Sisprenic.Api.Modules.Auth;

public static class AuthModule
{
    public static void MapAuthModule(this IEndpointRouteBuilder app)
    {
        app.MapLogout();
    }
}
