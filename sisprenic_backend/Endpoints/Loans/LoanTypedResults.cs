using Microsoft.EntityFrameworkCore;
using sisprenic.Database;
using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Entities;
using sisprenic_backend.Mapping;

namespace sisprenic_backend.Endpoints.Loans;

public static class LoanTypedResults
{
    public static async Task<IResult> GetAllLoans(SisprenicContext dbContext)
    {
        var loans = await dbContext.Loan.AsNoTracking().ToListAsync();
        return TypedResults.Ok(loans);
    }

    public static async Task<IResult> GetLoan(int id, SisprenicContext dbContext)
    {
        Loan? loan = await dbContext.Loan
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == id);
        
        return loan is null ? TypedResults.NotFound() : TypedResults.Ok(loan.ToLoanDetailDto());
    }

    public static async Task<IResult> CreateLoan(CreateLoanDto createLoan, SisprenicContext dbContext)
    {
        bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == createLoan.ClientId);
        if (!clientExists) return TypedResults.BadRequest($"Client with id {createLoan.ClientId} does not exist.");

        Loan loan = new()
        {
            Principal = createLoan.Principal,
            InterestRate = createLoan.InterestRate,
            TermMonths = createLoan.TermMonths,
            StartDate = createLoan.StartDate,
            ClientId = createLoan.ClientId,
            Client = null!
        };

        dbContext.Loan.Add(loan);
        await dbContext.SaveChangesAsync();
        return TypedResults.Created($"/loans/{loan.Id}", loan);
    }

    public static async Task<IResult> UpdateLoan(int id, UpdateLoanDto updateLoan, SisprenicContext dbContext)
    {
        Loan? loan = await dbContext.Loan.FindAsync(id);
        if (loan is null) return TypedResults.NotFound();

        if (loan.ClientId != updateLoan.ClientId)
        {
            bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == updateLoan.ClientId);
            if (!clientExists) return TypedResults.BadRequest($"Client with id {updateLoan.ClientId} does not exist.");
        }

        dbContext.Entry(loan).CurrentValues.SetValues(updateLoan);
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteLoan(int id, SisprenicContext dbContext)
    {
        Loan? loan = await dbContext.Loan.FindAsync(id);
        if (loan is null) return TypedResults.NotFound();
        dbContext.Loan.Remove(loan);
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
