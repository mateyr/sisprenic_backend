using static sisprenic_backend.Endpoints.Clients.ClientTypedResults;

namespace sisprenic_backend.Endpoints.Clients
{
    public static class ClientEndpoints
    {
        public static RouteGroupBuilder MapClientEndpoints(this WebApplication app)
        {
            var route = app.MapGroup("/clients");

            route.MapGet("/", GetAllClients).RequireAuthorization("clients:read");
            route.MapGet("/{id}", GetClient);
            route.MapPost("/", CreateClient);
            route.MapPut("/{id}", UpdateClient);
            route.MapDelete("/{id}", DeleteClient);

            return route;
        }
    }
}
