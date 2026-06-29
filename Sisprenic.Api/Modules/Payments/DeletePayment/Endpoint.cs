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
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        Payment? payment = await dbContext.Payment.FindAsync([id], cancellationToken);
        if (payment is null) return TypedResults.NotFound();

        Loan? loan = await LoanLockService.LoadForUpdateAsync(dbContext, payment.LoanId, cancellationToken);

        await DeletePaymentHandler.Execute(loan, payment, dbContext, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
