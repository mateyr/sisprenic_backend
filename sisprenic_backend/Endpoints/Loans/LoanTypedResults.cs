using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Dtos.Payments;
using sisprenic_backend.Endpoints.Loans.Services;
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
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan is null) return TypedResults.NotFound();

        LoanSummaryDto summary = await LoanSummaryService.CalculateAsync(loan, dbContext);

        return TypedResults.Ok(loan.ToLoanDetailDto(summary));
    }

    public static async Task<IResult> GetLoanPayments(int id, SisprenicContext dbContext)
    {
        bool loanExists = await dbContext.Loan.AnyAsync(l => l.Id == id);

        if (!loanExists) {
            return TypedResults.NotFound();
        }

        List<GetPaymentDetailDto> payments = await dbContext.Payment
            .Where(p => p.LoanId == id)
            .OrderBy(p => p.PaymentDay)
            .AsNoTracking()
            .Select(p => p.ToPaymentDetailDto())
            .ToListAsync();

        return TypedResults.Ok(payments);
    }

    public static async Task<IResult> CreateLoan(
        IValidator<CreateLoanDto> validator,
        CreateLoanDto createLoan,
        SisprenicContext dbContext)
    {
        ValidationResult validationResult = await validator.ValidateAsync(createLoan);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == createLoan.ClientId);
        if (!clientExists)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["ClientId"] = [$"No existe un cliente con el id {createLoan.ClientId}."]
                });
        }

        Loan loan = new()
        {
            Principal = createLoan.Principal!.Value,
            InterestRate = createLoan.InterestRate!.Value,
            TermMonths = createLoan.TermMonths!.Value,
            StartDate = createLoan.StartDate!.Value,
            ClientId = createLoan.ClientId!.Value,
            Client = null!
        };

        dbContext.Loan.Add(loan);
        await dbContext.SaveChangesAsync();
        return TypedResults.Created($"/loans/{loan.Id}", loan);
    }

    public static async Task<IResult> UpdateLoan(
        int id,
        IValidator<UpdateLoanDto> validator,
        UpdateLoanDto updateLoan,
        SisprenicContext dbContext)
    {
        ValidationResult validationResult = await validator.ValidateAsync(updateLoan);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Loan? loan = await dbContext.Loan.FindAsync(id);
        if (loan is null) return TypedResults.NotFound();

        bool hasPayments = await dbContext.Payment.AnyAsync(p => p.LoanId == id);

        if (hasPayments)
        {
            bool tryingToChangeRestrictedFields =
                updateLoan.Principal.HasValue ||
                updateLoan.InterestRate.HasValue ||
                updateLoan.TermMonths.HasValue ||
                updateLoan.StartDate.HasValue;

            if (tryingToChangeRestrictedFields)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [""] = ["No se puede modificar un préstamo que ya tiene pagos registrados. Solo está permitido cambiar el cliente."]
                });
        }

        if (updateLoan.ClientId.HasValue && loan.ClientId != updateLoan.ClientId.Value)
        {
            bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == updateLoan.ClientId.Value);
            if (!clientExists)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ClientId"] = [$"No existe un cliente con el id {updateLoan.ClientId.Value}."]
                });
        }

        if (updateLoan.Principal.HasValue)    loan.Principal    = updateLoan.Principal.Value;
        if (updateLoan.InterestRate.HasValue) loan.InterestRate = updateLoan.InterestRate.Value;
        if (updateLoan.TermMonths.HasValue)   loan.TermMonths   = updateLoan.TermMonths.Value;
        if (updateLoan.StartDate.HasValue)    loan.StartDate    = updateLoan.StartDate.Value;
        if (updateLoan.ClientId.HasValue)     loan.ClientId     = updateLoan.ClientId.Value;

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
