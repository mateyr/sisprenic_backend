using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetAllLoans;

public static class GetAllLoansEndpoint
{
    public static void MapGetAllLoans(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle).RequireAuthorization(Permissions.Loans.Read);
    }

    private static async Task<IResult> Handle(SisprenicContext dbContext)
    {
        List<GetAllLoansResponse> loans = await dbContext.Loan
            .AsNoTracking()
            .Include(l => l.Client)
            .Select(l => new GetAllLoansResponse(
                l.Id,
                l.Principal,
                l.InterestRate,
                l.TermMonths,
                l.StartDate,
                l.Status.ToDisplayName(),
                new ClientSummaryResponse(
                    l.Client.Id,
                    l.Client.FirstName,
                    l.Client.SecondName,
                    l.Client.LastName,
                    l.Client.SecondLastName,
                    l.Client.Identification,
                    l.Client.PhoneNumber)))
            .ToListAsync();

        return TypedResults.Ok(loans);
    }
}
