using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetAllLoans;

public static class GetAllLoansEndpoint
{
    public static void MapGetAllLoans(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle);
    }

    private static async Task<IResult> Handle(SisprenicContext dbContext)
    {
        List<GetAllLoansResponse> loans = await dbContext.Loan
            .Include(l => l.Client)
            .AsNoTracking()
            .Select(l => new GetAllLoansResponse(
                l.Id,
                l.Principal,
                l.InterestRate,
                l.TermMonths,
                l.StartDate,
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
