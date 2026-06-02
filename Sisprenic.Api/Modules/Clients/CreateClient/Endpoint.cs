using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Clients.CreateClient;

public static class CreateClientEndpoint
{
    public static void MapCreateClient(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle);
    }

    private static async Task<IResult> Handle(CreateClientRequest request, SisprenicContext dbContext)
    {
        Client client = new()
        {
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            LastName = request.LastName,
            SecondLastName = request.SecondLastName,
            Identification = request.Identification,
            PhoneNumber = request.PhoneNumber
        };

        dbContext.Client.Add(client);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/clients/{client.Id}", client);
    }
}
