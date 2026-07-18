using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Domain.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetLoanById;

public static class GetLoanByIdEndpoint
{
    public static void MapGetLoanById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", Handle).RequireAuthorization(Permissions.Loans.Read);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        Loan? loan = await dbContext.Loan
            .Include(l => l.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (loan is null) return TypedResults.NotFound();

        LoanSummaryDto summary = await LoanSummaryService.CalculateAsync(loan, dbContext, cancellationToken);

        GetLoanByIdResponse response = new(
            loan.Id,
            loan.Principal,
            loan.InterestRate,
            loan.TermMonths,
            loan.StartDate,
            loan.Status.ToDisplayName(),
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
