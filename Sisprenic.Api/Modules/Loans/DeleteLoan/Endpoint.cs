using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Modules.Loans.Shared;
using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Modules.Loans.DeleteLoan;

public static class DeleteLoanEndpoint
{
    public static void MapDeleteLoan(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", Handle).RequireAuthorization(Permissions.Loans.Delete);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        Loan? loan = await LoanLockService.LoadForUpdateAsync(dbContext, id, cancellationToken);
        if (loan is null) return TypedResults.NotFound();

        bool hasPayments = await dbContext.Payment.AnyAsync(p => p.LoanId == id, cancellationToken);
        if (hasPayments)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["payments"] = ["No se puede eliminar un préstamo que cuenta con pagos registrados."]
                });
        }

        dbContext.Loan.Remove(loan);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
