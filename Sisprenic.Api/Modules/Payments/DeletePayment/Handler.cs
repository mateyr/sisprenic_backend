using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Common;
using Sisprenic.Api.Database;

using Sisprenic.Domain.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Payments.DeletePayment;

public static class DeletePaymentHandler
{
    public static async Task Execute(
        Loan? loan,
        Payment payment,
        SisprenicContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (loan is not null)
        {
            DateOnly today = BusinessClock.Today();

            List<Payment> remainingPayments = await dbContext.Payment
                .AsNoTracking()
                .Where(p => p.LoanId == loan.Id
                         && p.Id != payment.Id
                         && p.PaymentDay >= loan.StartDate
                         && p.PaymentDay <= today)
                .ToListAsync(cancellationToken);

            LoanSummaryDto summary = LoanSummaryService.Calculate(loan, remainingPayments, today);

            loan.Status = LoanStatusService.Resolve(summary.PrincipalCurrent, summary.InterestOutstanding);
        }

        dbContext.Payment.Remove(payment);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
