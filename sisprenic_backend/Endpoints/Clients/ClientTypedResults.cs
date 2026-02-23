using Microsoft.EntityFrameworkCore;
using sisprenic_backend.Dtos.Clients;
using sisprenic.Database;
using sisprenic.Entities;

namespace sisprenic_backend.Endpoints.Clients;

public static class ClientTypedResults
{
    public static async Task<IResult> GetAllClients(SisprenicContext dbContext)
    {
        var clients = await dbContext.Client.AsNoTracking().ToListAsync();

        return TypedResults.Ok(clients);
    }

    public static async Task<IResult> GetClient(int id, SisprenicContext dbContext)
    {
        Client? client = await dbContext.Client.FindAsync(id);

        return client is null ? TypedResults.NotFound() : TypedResults.Ok(client);
    }

    public static async Task<IResult> CreateClient(Client client, SisprenicContext dbContext)
    {
        dbContext.Client.Add(client);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/clients/{client.Id}", client);
    }

    public static async Task<IResult> UpdateClient(
        int id,
        UpdateClientDto updateClient,
        SisprenicContext dbContext
    )
    {
        Client? client = await dbContext.Client.FindAsync(id);
        if (client is null)
            return TypedResults.NotFound();

        dbContext.Entry(client).CurrentValues.SetValues(updateClient);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteClient(int id, SisprenicContext dbContext)
    {
        Client? client = await dbContext.Client.FindAsync(id);
        if (client is null)
            return TypedResults.NotFound();

        dbContext.Client.Remove(client);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
