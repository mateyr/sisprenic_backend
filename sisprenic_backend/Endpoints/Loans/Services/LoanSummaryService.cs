using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Entities;

namespace sisprenic_backend.Endpoints.Loans.Services;

// Se recalcula el estado en cada lectura en lugar de persistirlo,
// dado que el volumen de pagos por préstamo es bajo.

// Si el volumen o la frecuencia de lecturas lo exigen más adelante, se puede
// valorar persistir un snapshot del estado al momento de cada pago y usarlo para el resumen.
public static class LoanSummaryService
{
    public static async Task<LoanSummaryDto> CalculateAsync(Loan loan, SisprenicContext dbContext)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        List<Payment> payments = await dbContext.Payment
            .AsNoTracking()
            .Where(p => p.LoanId == loan.Id
                     && p.PaymentDay >= loan.StartDate
                     && p.PaymentDay <= today)
            .ToListAsync();

        return Calculate(loan, payments, today);
    }

    // Cada ciclo k cubre el rango [StartDate + k meses, StartDate + (k + 1) meses).
    // El ciclo actual es aquel que contiene la fecha de "hoy".
    //
    // - interestPending      = suma del déficit de interés de los ciclos PASADOS.
    // - interestThisPeriod   = déficit de interés del ciclo ACTUAL
    //                          (interés esperado del ciclo - interés ya pagado en el ciclo).
    //   Así, si el cliente ya pagó parcialmente este mes, sólo se sugiere lo que falta.
    internal static LoanSummaryDto Calculate(Loan loan, IReadOnlyCollection<Payment> payments, DateOnly today)
    {
        int currentCycle = MonthsBetween(loan.StartDate, today);

        decimal totalPrincipalPaid = payments.Sum(p => p.Principal);
        decimal principalCurrent = loan.Principal - totalPrincipalPaid;

        Dictionary<int, (decimal PaidInterest, decimal PaidPrincipal)> paymentsByCycle = payments
            .GroupBy(p => MonthsBetween(loan.StartDate, p.PaymentDay))
            .ToDictionary(
                g => g.Key,
                g => (PaidInterest: g.Sum(p => p.Interest), PaidPrincipal: g.Sum(p => p.Principal))
            );

        // Recorrer los ciclos manteniendo el capital vigente al inicio de cada uno.
        // Solo se cuenta el déficit por ciclo: un sobrepago no compensa otro ciclo.
        decimal interestPending = 0m;
        decimal interestThisPeriod = 0m;
        decimal principalAtStart = loan.Principal;

        for (int cycle = 0; cycle <= currentCycle; cycle++)
        {
            decimal expected = principalAtStart * loan.InterestRate;
            decimal paidInterest = 0m;
            decimal paidPrincipal = 0m;

            if (paymentsByCycle.TryGetValue(cycle, out var c))
            {
                paidInterest = c.PaidInterest;
                paidPrincipal = c.PaidPrincipal;
            }

            decimal unpaid = Math.Max(expected - paidInterest, 0m);

            if (cycle < currentCycle)
            {
                interestPending += unpaid;
            }
            else
            {
                interestThisPeriod = unpaid;
            }

            // El abono a capital del ciclo k afecta el interés desde el ciclo k+1.
            principalAtStart -= paidPrincipal;
        }

        DateOnly nextPaymentDate = loan.StartDate.AddMonths(currentCycle + 1);

        return new LoanSummaryDto(
            principalCurrent,
            interestThisPeriod,
            interestPending,
            nextPaymentDate
        );
    }

    // Si end < start (caso raro: préstamo con StartDate futura), devuelve 0
    // para no romper el cálculo de ciclos con un valor negativo.
    private static int MonthsBetween(DateOnly start, DateOnly end)
    {
        int months = ((end.Year - start.Year) * 12) + (end.Month - start.Month);
        if (end.Day < start.Day) months--;
        return months < 0 ? 0 : months;
    }
}
