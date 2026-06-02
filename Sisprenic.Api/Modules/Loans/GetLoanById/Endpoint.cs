using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

using Sisprenic.Api.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetLoanById;

public static class GetLoanByIdEndpoint
{
    public static void MapGetLoanById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", Handle);
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        Loan? loan = await dbContext.Loan
            .Include(l => l.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan is null) return TypedResults.NotFound();

        LoanSummaryDto summary = await LoanSummaryService.CalculateAsync(loan, dbContext);

        GetLoanByIdResponse response = new(
            loan.Id,
            loan.Principal,
            loan.InterestRate,
            loan.TermMonths,
            loan.StartDate,
            new ClientSummaryResponse(
                loan.Client.Id,
                loan.Client.FirstName,
                loan.Client.SecondName,
                loan.Client.LastName,
                loan.Client.SecondLastName,
                loan.Client.Identification,
                loan.Client.PhoneNumber),
            summary);

        return TypedResults.Ok(response);
    }
}
