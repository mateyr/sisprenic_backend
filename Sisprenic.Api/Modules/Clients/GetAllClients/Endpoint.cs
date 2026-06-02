using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

namespace Sisprenic.Api.Modules.Clients.GetAllClients;

public static class GetAllClientsEndpoint
{
    public static void MapGetAllClients(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle).RequireAuthorization("clients:read");
    }

    private static async Task<IResult> Handle(SisprenicContext dbContext)
    {
        List<GetAllClientsResponse> clients = await dbContext.Client
            .AsNoTracking()
            .Select(c => new GetAllClientsResponse(
                c.Id,
                c.FirstName,
                c.SecondName,
                c.LastName,
                c.SecondLastName,
                c.Identification,
                c.PhoneNumber))
            .ToListAsync();

        return TypedResults.Ok(clients);
    }
}
