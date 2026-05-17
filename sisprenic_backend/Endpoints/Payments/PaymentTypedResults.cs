using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Common;
using sisprenic_backend.Dtos.Payments;
using sisprenic_backend.Entities;
using sisprenic_backend.Mapping;

namespace sisprenic_backend.Endpoints.Payments;

public static class PaymentTypedResults
{
    public static async Task<IResult> GetAllPayments(SisprenicContext dbContext)
    {
        List<GetPaymentDto> payments = await dbContext.Payment
            .AsNoTracking()
            .Select(p => p.ToPaymentDto())
            .ToListAsync();

        return TypedResults.Ok(payments);
    }

    public static async Task<IResult> GetPayment(int id, SisprenicContext dbContext)
    {
        Payment? payment = await dbContext.Payment
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return payment is null ? TypedResults.NotFound() : TypedResults.Ok(payment.ToPaymentDto());
    }

    public static async Task<IResult> CreatePayment(
        IValidator<CreatePaymentDto> validator,
        CreatePaymentDto createPayment,
        SisprenicContext dbContext)
    {
        ValidationResult validationResult = await validator.ValidateAsync(createPayment);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Loan? loan = await dbContext.Loan
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == createPayment.LoanId);

        if (loan is null)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["loanId"] = [$"No existe un préstamo con el id {createPayment.LoanId}."]
                });
        }

        CreatePaymentResult result = await CreatePaymentService.Execute(loan, createPayment, dbContext);

        if (!result.IsSuccess)
        {
            return Results.ValidationProblem(result.Errors!);
        }

        ApiResponse<GetPaymentDto> body = new(
            Data: result.Payment!.ToPaymentDto(),
            Messages: result.Messages);

        return TypedResults.Created($"/payments/{result.Payment!.Id}", body);
    }

    public static async Task<IResult> DeletePayment(int id, SisprenicContext dbContext)
    {
        Payment? payment = await dbContext.Payment.FindAsync(id);
        if (payment is null) return TypedResults.NotFound();

        dbContext.Payment.Remove(payment);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
