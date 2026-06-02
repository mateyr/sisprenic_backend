using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Clients.UpdateClient;

public static class UpdateClientEndpoint
{
    public static void MapUpdateClient(this RouteGroupBuilder group)
    {
        group.MapPut("/{id}", Handle);
    }

    private static async Task<IResult> Handle(
        int id,
        UpdateClientRequest request,
        SisprenicContext dbContext)
    {
        Client? client = await dbContext.Client.FindAsync(id);
        if (client is null)
            return TypedResults.NotFound();

        dbContext.Entry(client).CurrentValues.SetValues(request);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
