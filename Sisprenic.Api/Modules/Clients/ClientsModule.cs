using Sisprenic.Api.Modules.Clients.CreateClient;
using Sisprenic.Api.Modules.Clients.DeleteClient;
using Sisprenic.Api.Modules.Clients.GetAllClients;
using Sisprenic.Api.Modules.Clients.GetClientById;
using Sisprenic.Api.Modules.Clients.UpdateClient;

namespace Sisprenic.Api.Modules.Clients;

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
