using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

namespace Sisprenic.Api.Modules.Clients.GetClientById;

public static class GetClientByIdEndpoint
{
    public static void MapGetClientById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", Handle).RequireAuthorization(Permissions.Clients.Read);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(cancellationToken);

        return client is null ? TypedResults.NotFound() : TypedResults.Ok(client);
    }
}
