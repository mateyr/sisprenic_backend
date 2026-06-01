using sisprenic_backend.Modules.Clients.CreateClient;
using sisprenic_backend.Modules.Clients.DeleteClient;
using sisprenic_backend.Modules.Clients.GetAllClients;
using sisprenic_backend.Modules.Clients.GetClientById;
using sisprenic_backend.Modules.Clients.UpdateClient;

namespace sisprenic_backend.Modules.Clients;

public static class ClientsModule
{
    public static void MapClientsModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/clients").WithTags("Clients");

        group.MapGetAllClients();
        group.MapGetClientById();
        group.MapCreateClient();
        group.MapUpdateClient();
        group.MapDeleteClient();
    }
}
