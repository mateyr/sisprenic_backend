using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

namespace Sisprenic.Api.Modules.Loans.GetLoanPayments;

public static class GetLoanPaymentsEndpoint
{
    public static void MapGetLoanPayments(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/payments", Handle).RequireAuthorization(Permissions.Loans.Read);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        bool loanExists = await dbContext.Loan.AnyAsync(l => l.Id == id, cancellationToken);

        if (!loanExists)
        {
            return TypedResults.NotFound();
        }

        List<LoanPaymentResponse> payments = await dbContext.Payment
            .Where(p => p.LoanId == id)
            .OrderBy(p => p.PaymentDay)
            .AsNoTracking()
            .Select(p => new LoanPaymentResponse(
                p.Id,
                p.Principal,
                p.Interest,
                p.PaymentDay,
                p.Note))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(payments);
    }
}
