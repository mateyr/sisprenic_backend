using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Clients.DeleteClient;

public static class DeleteClientEndpoint
{
    public static void MapDeleteClient(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle).RequireAuthorization("clients:delete");
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        Client? client = await dbContext.Client.FindAsync(id);
        if (client is null)
            return TypedResults.NotFound();

        bool hasLoans = await dbContext.Loan.AnyAsync(p => p.ClientId == id);

        if (hasLoans)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["client"] = ["No se puede eliminar el cliente porque tiene préstamos registrados."]
                });
        }

        dbContext.Client.Remove(client);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
