using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

namespace sisprenic_backend.Modules.Clients.GetClientById;

public static class GetClientByIdEndpoint
{
    public static void MapGetClientById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", Handle).RequireAuthorization("clients:read");
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        GetClientByIdResponse? client = await dbContext.Client
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new GetClientByIdResponse(
                c.Id,
                c.FirstName,
                c.SecondName,
                c.LastName,
                c.SecondLastName,
                c.Identification,
                c.PhoneNumber,
                c.Loans
                    .Select(l => new ClientLoanResponse(
                        l.Id,
                        l.Principal,
                        l.InterestRate,
                        l.TermMonths,
                        l.StartDate))
                    .ToList()))
            .FirstOrDefaultAsync();

        return client is null ? TypedResults.NotFound() : TypedResults.Ok(client);
    }
}
