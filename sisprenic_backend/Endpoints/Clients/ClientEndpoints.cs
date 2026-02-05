using static sisprenic_backend.Endpoints.clients.ClientTypedResults;

namespace sisprenic_backend.Endpoints.clients
{
    public static class ClientEndpoints
    {
        public static RouteGroupBuilder MapClientEndpoints(this WebApplication app)
        {
            var route = app.MapGroup("/clients");

            route.MapGet("/", GetAllClients);
            route.MapGet("/{id}", GetClient);
            route.MapPost("/", CreateClient);
            route.MapPut("/{id}", UpdateClient);
            route.MapDelete("/{id}", DeleteClient);

            return route;
        }
    }
}
