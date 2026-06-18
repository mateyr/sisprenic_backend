using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Loans.DeleteLoan;

public static class DeleteLoanEndpoint
{
    public static void MapDeleteLoan(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle).RequireAuthorization(Permissions.Loans.Delete);
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        Loan? loan = await dbContext.Loan.FindAsync(id);
        if (loan is null) return TypedResults.NotFound();

        bool hasPayments = await dbContext.Payment.AnyAsync(p => p.LoanId == id);
        if (hasPayments)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["payments"] = ["No se puede eliminar un préstamo que cuenta con pagos registrados."]
                });
        }

        dbContext.Loan.Remove(loan);
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
