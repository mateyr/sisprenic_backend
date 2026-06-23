using Sisprenic.Api.Modules.Auth.Login;
using Sisprenic.Api.Modules.Auth.Logout;
using Sisprenic.Api.Modules.Auth.Register;

namespace Sisprenic.Api.Modules.Auth;

public static class AuthModule
{
    public static void MapAuthModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group.MapLogin();
        group.MapRegister();
        group.MapLogout();
    }
}
