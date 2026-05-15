using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Dtos.Payments;
using sisprenic_backend.Endpoints.Loans.Services;
using sisprenic_backend.Entities;

namespace sisprenic_backend.Endpoints.Payments;

public sealed record CreatePaymentResult
{
    public Payment? Payment { get; init; }
    public string? Message { get; init; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public bool IsSuccess => Errors is null;

    public static CreatePaymentResult Failure(Dictionary<string, string[]> errors) =>
        new() { Errors = errors };

    public static CreatePaymentResult Success(Payment payment, string? message = null) =>
        new() { Payment = payment, Message = message };
}

public static class CreatePaymentService
{
    public static async Task<CreatePaymentResult> Execute(
        Loan loan,
        CreatePaymentDto createPayment,
        SisprenicContext dbContext)
    {
        if (createPayment.PaymentDay < loan.StartDate)
        {
            return CreatePaymentResult.Failure(new()
            {
                ["paymentDay"] = ["La fecha de pago no puede ser anterior al inicio del préstamo."]
            });
        }

        List<Payment> payments = await dbContext.Payment
            .AsNoTracking()
            .Where(p =>
                p.LoanId == createPayment.LoanId
                && p.PaymentDay >= loan.StartDate
                && p.PaymentDay <= createPayment.PaymentDay)
            .ToListAsync();

        LoanSummaryDto summary = LoanSummaryService.Calculate(loan, payments, createPayment.PaymentDay);

        decimal totalInterestOutstanding = summary.InterestPending + summary.InterestThisPeriod;

        if (summary.PrincipalCurrent == 0 && totalInterestOutstanding == 0)
        {
            return CreatePaymentResult.Failure(new()
            {
                ["loan"] = ["El préstamo ya ha sido pagado en su totalidad."]
            });
        }

        decimal requestedInterest = createPayment.Interest;
        decimal requestedPrincipal = createPayment.Principal;
        decimal requestedTotal = requestedInterest + requestedPrincipal;

        // Aplica primero el interés con tope en lo pendiente; cualquier sobrante intenta cubrir capital.
        decimal interestApplied = Math.Min(requestedInterest, totalInterestOutstanding);
        decimal interestOverflow = requestedInterest - interestApplied;

        // Capital intentado = lo enviado por el usuario + el sobrante de interés.
        // Tope final: capital pendiente del préstamo. Lo que exceda no se registra.
        decimal principalAttempt = requestedPrincipal + interestOverflow;
        decimal principalApplied = Math.Min(principalAttempt, summary.PrincipalCurrent);
        decimal unapplied = principalAttempt - principalApplied;

        decimal totalApplied = interestApplied + principalApplied;
        string? message = BuildMessage(
            totalInterestOutstanding,
            interestOverflow,
            unapplied,
            requestedTotal,
            totalApplied);

        Payment payment = new()
        {
            Principal = principalApplied,
            Interest = interestApplied,
            PaymentDay = createPayment.PaymentDay,
            Note = createPayment.Note,
            LoanId = loan.Id,
            Loan = null!
        };

        dbContext.Payment.Add(payment);
        await dbContext.SaveChangesAsync();

        return CreatePaymentResult.Success(payment, message);
    }

    private static string? BuildMessage(
        decimal totalInterestOutstanding,
        decimal interestOverflow,
        decimal unapplied,
        decimal requestedTotal,
        decimal totalApplied)
    {
        List<string> notes = new();

        // Sólo informa el reruteo cuando el sobrante de interés efectivamente se registró como capital
        // (si ese sobrante también terminó descartado, el siguiente mensaje ya lo cubre).
        decimal interestRoutedToPrincipal = Math.Max(interestOverflow - unapplied, 0m);
        if (interestRoutedToPrincipal > 0m)
        {
            notes.Add(totalInterestOutstanding == 0m
                ? "No había interés pendiente; el monto enviado como interés se registró como capital."
                : "El interés excedía el monto pendiente; el exceso se aplicó a capital.");
        }

        if (unapplied > 0m)
        {
            notes.Add(
                $"Se enviaron {requestedTotal:0.00} pero el sistema solo aplicó {totalApplied:0.00} " +
                $"para no exceder la deuda total. {unapplied:0.00} no fueron registrados.");
        }

        return notes.Count == 0 ? null : string.Join(" ", notes);
    }
}
