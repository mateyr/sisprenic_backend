using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

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

    public static async Task<IResult> CreatePayment(CreatePaymentDto createPayment, SisprenicContext dbContext)
    {
        bool loanExists = await dbContext.Loan.AnyAsync(l => l.Id == createPayment.LoanId);
        if (!loanExists) return TypedResults.BadRequest($"Loan with id {createPayment.LoanId} does not exist.");

        Payment payment = new()
        {
            Principal = createPayment.Principal,
            Interest = createPayment.Interest,
            PaymentDay = createPayment.PaymentDay,
            Note = createPayment.Note,
            LoanId = createPayment.LoanId,
            Loan = null!
        };

        dbContext.Payment.Add(payment);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/payments/{payment.Id}", payment.ToPaymentDto());
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
