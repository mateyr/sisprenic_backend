using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;

using Sisprenic.Api.Common;
using Sisprenic.Api.Entities;
using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Payments.CreatePayment;

public sealed record CreatePaymentResult
{
    public Payment? Payment { get; init; }
    public IReadOnlyList<ApiMessage>? Messages { get; init; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public bool IsSuccess => Errors is null;

    public static CreatePaymentResult Failure(Dictionary<string, string[]> errors) =>
        new() { Errors = errors };

    public static CreatePaymentResult Success(Payment payment, IReadOnlyList<ApiMessage>? message = null) =>
        new() { Payment = payment, Messages = message };
}

public static class CreatePaymentHandler
{
    public static async Task<CreatePaymentResult> Execute(
        Loan loan,
        CreatePaymentRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (request.PaymentDay < loan.StartDate)
        {
            return CreatePaymentResult.Failure(new()
            {
                ["paymentDay"] = ["La fecha de pago no puede ser anterior al inicio del préstamo."]
            });
        }

        List<Payment> payments = await dbContext.Payment
            .AsNoTracking()
            .Where(p =>
                p.LoanId == request.LoanId
                && p.PaymentDay >= loan.StartDate
                && p.PaymentDay <= request.PaymentDay)
            .ToListAsync(cancellationToken);

        LoanSummaryDto summary = LoanSummaryService.Calculate(loan, payments, request.PaymentDay);

        decimal totalInterestOutstanding = summary.InterestOutstanding;

        if (summary.PrincipalCurrent == 0 && totalInterestOutstanding == 0)
        {
            return CreatePaymentResult.Failure(new()
            {
                ["loan"] = ["El préstamo ya ha sido pagado en su totalidad."]
            });
        }

        decimal requestedInterest = request.Interest;
        decimal requestedPrincipal = request.Principal;
        decimal requestedTotal = requestedInterest + requestedPrincipal;

        // Aplica primero el interés con tope en lo pendiente; cualquier sobrante intenta cubrir capital.
        decimal interestApplied = Math.Min(requestedTotal, totalInterestOutstanding);
        decimal remainingAfterInterest = requestedTotal - interestApplied;

        // Capital intentado = lo enviado por el usuario + el sobrante de interés.
        // Tope final: capital pendiente del préstamo. Lo que exceda no se registra.
        decimal principalApplied = Math.Min(remainingAfterInterest, summary.PrincipalCurrent);

        decimal totalApplied = interestApplied + principalApplied;
        decimal unapplied = requestedTotal - totalApplied;

        IReadOnlyList<ApiMessage>? message = PaymentMessages.Build(
            requestedInterest,
            requestedPrincipal,
            totalInterestOutstanding,
            interestApplied,
            principalApplied,
            requestedTotal,
            totalApplied,
            unapplied);

        Payment payment = new()
        {
            Principal = principalApplied,
            Interest = interestApplied,
            PaymentDay = request.PaymentDay,
            Note = request.Note,
            LoanId = loan.Id,
            Loan = null!
        };

        decimal remainingPrincipal = summary.PrincipalCurrent - principalApplied;
        decimal remainingInterest = totalInterestOutstanding - interestApplied;

        loan.Status = LoanStatusService.Resolve(remainingPrincipal, remainingInterest);

        dbContext.Payment.Add(payment);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatePaymentResult.Success(payment, message);
    }
}
