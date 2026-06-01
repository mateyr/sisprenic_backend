using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Entities;

namespace sisprenic_backend.Modules.Loans.DeleteLoan;

public static class DeleteLoanEndpoint
{
    public static void MapDeleteLoan(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle);
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
