using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

using Sisprenic.Api.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Payments.DeletePayment;

public static class DeletePaymentEndpoint
{
    public static void MapDeletePayment(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle).RequireAuthorization("payments:delete");
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        Payment? payment = await dbContext.Payment.FindAsync(id);
        if (payment is null) return TypedResults.NotFound();

        Loan? loan = await dbContext.Loan.FindAsync(payment.LoanId);

        if (loan is not null && loan.Status == LoanStatus.Paid)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            List<Payment> remainingPayments = await dbContext.Payment
                .AsNoTracking()
                .Where(p => p.LoanId == loan.Id
                         && p.Id != id
                         && p.PaymentDay >= loan.StartDate
                         && p.PaymentDay <= today)
                .ToListAsync();

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

        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
