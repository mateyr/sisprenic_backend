using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Payments.DeletePayment;

public static class DeletePaymentEndpoint
{
    public static void MapDeletePayment(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle).RequireAuthorization(Permissions.Payments.Delete);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        Payment? payment = await dbContext.Payment.FindAsync([id], cancellationToken);
        if (payment is null) return TypedResults.NotFound();

        Loan? loan = await dbContext.Loan.FindAsync([payment.LoanId], cancellationToken);

        if (loan is not null && loan.Status == LoanStatus.Paid)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            List<Payment> remainingPayments = await dbContext.Payment
                .AsNoTracking()
                .Where(p => p.LoanId == loan.Id
                         && p.Id != id
                         && p.PaymentDay >= loan.StartDate
                         && p.PaymentDay <= today)
                .ToListAsync(cancellationToken);

            LoanSummaryDto summary = LoanSummaryService.Calculate(loan, remainingPayments, today);

            bool hasOutstandingBalance =
                summary.PrincipalCurrent > 0 ||
                summary.InterestPending > 0 ||
                summary.InterestThisPeriod > 0;

            if (hasOutstandingBalance)
            {
                loan.Status = LoanStatus.Active;
            }
        }

        dbContext.Payment.Remove(payment);

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
