using Xunit;

using sisprenic.Entities;

using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Endpoints.Loans.Services;
using sisprenic_backend.Entities;

namespace Sisprenic.UnitTests.Loans;

public sealed class LoanSummaryServiceTests
{
    private static readonly DateOnly Anchor = new DateOnly(2026, 1, 1);

    [Fact(DisplayName = "Ciclo 0 pagado íntegro: sin pendientes; ciclo actual sin pagos muestra interés íntegro")]
    public void FullInterest_HaveNoPending_AndFullNextCycleInterest()
    {
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, Anchor);

        Payment[] payments =
        [
            Pay(loan, id: 1, interest: 1_000m, principal: 0m, new DateOnly(2026, 1, 10)),
        ];

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, payments, today: new DateOnly(2026, 2, 1));

        Assert.Equal(0m, s.InterestPending);
        Assert.Equal(1_000m, s.InterestThisPeriod);
        Assert.Equal(10_000m, s.PrincipalCurrent);
    }

    [Fact(DisplayName = "Interés parcial en ciclo 0: pendiente igual al déficit; ciclo siguiente con interés íntegro")]
    public void PartialInterest_AccumulatePending()
    {
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, Anchor);

        Payment[] payments =
        [
            Pay(loan, id: 1, interest: 500m, principal: 0m, new DateOnly(2026, 1, 5)),
        ];

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, payments, today: new DateOnly(2026, 2, 1));

        Assert.Equal(500m, s.InterestPending);
        Assert.Equal(1_000m, s.InterestThisPeriod);
        Assert.Equal(10_000m, s.PrincipalCurrent);
    }

    [Fact(DisplayName = "Abono a capital en ciclo 0 reduce capital actual y esperado de interés del ciclo 1")]
    public void PrincipalPayment_ReduceNextCycleInterest_AndPrincipal()
    {
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, Anchor);

        Payment[] payments =
        [
            Pay(loan, id: 1, interest: 1_000m, principal: 1_000m, new DateOnly(2026, 1, 15)),
        ];

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, payments, today: new DateOnly(2026, 2, 1));

        Assert.Equal(0m, s.InterestPending);
        Assert.Equal(900m, s.InterestThisPeriod);
        Assert.Equal(9_000m, s.PrincipalCurrent);
        Assert.Equal(new DateOnly(2026, 3, 1), s.NextPaymentDate);
    }

    [Fact(DisplayName = "Sin pagos: pendiente suma déficits en ciclos previos e interés íntegro en el ciclo actual")]
    public void NoPayments_AccumulatePending_AndFullCurrentInterest()
    {
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, Anchor);

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, Array.Empty<Payment>(), today: new DateOnly(2026, 3, 1));

        Assert.Equal(2_000m, s.InterestPending);
        Assert.Equal(1_000m, s.InterestThisPeriod);
        Assert.Equal(10_000m, s.PrincipalCurrent);
    }

    [Fact(DisplayName = "Varios pagos en el mismo ciclo deben agruparse (intereses y capitales sumados)")]
    public void MultiplePaymentsInSameCycle_AggregateBeforeCalculation()
    {
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, Anchor);

        Payment[] payments =
        [
            Pay(loan, id: 1, interest: 300m, principal: 0m, new DateOnly(2026, 1, 5)),
            Pay(loan, id: 2, interest: 400m, principal: 0m, new DateOnly(2026, 1, 20)),
            Pay(loan, id: 3, interest: 200m, principal: 250m, new DateOnly(2026, 1, 28)),
        ];

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, payments, today: new DateOnly(2026, 1, 30));

        Assert.Equal(0m, s.InterestPending);
        Assert.Equal(100m, s.InterestThisPeriod);
        Assert.Equal(9_750m, s.PrincipalCurrent);
    }

    [Fact(DisplayName = "StartDate en mitad de mes: pagos antes del día de ancla siguen en ciclo 0")]
    public void MidMonthStart_KeepCycleZeroUntilAnniversary()
    {
        DateOnly start = new DateOnly(2026, 1, 15);
        Loan loan = LoanOf(principal: 10_000m, rate: 0.10m, start);

        Payment[] payments =
        [
            Pay(loan, id: 1, interest: 1_000m, principal: 0m, new DateOnly(2026, 2, 10)),
        ];

        LoanSummaryDto s = LoanSummaryService.Calculate(loan, payments, today: new DateOnly(2026, 2, 10));

        Assert.Equal(0m, s.InterestPending);
        Assert.Equal(0m, s.InterestThisPeriod);
        Assert.Equal(10_000m, s.PrincipalCurrent);
    }

    private static Loan LoanOf(decimal principal, decimal rate, DateOnly startDate)
    {
        Client client = new()
        {
            Id = 1,
            FirstName = "Test",
            SecondName = null,
            LastName = "Cliente",
            SecondLastName = null,
            Identification = "T1",
            PhoneNumber = "000",
        };

        return new Loan
        {
            Id = 42,
            Principal = principal,
            InterestRate = rate,
            TermMonths = 120,
            StartDate = startDate,
            ClientId = client.Id,
            Client = client,
        };
    }

    private static Payment Pay(Loan loan, int id, decimal interest, decimal principal, DateOnly day) =>
        new()
        {
            Id = id,
            Interest = interest,
            Principal = principal,
            PaymentDay = day,
            LoanId = loan.Id,
            Loan = loan,
        };
}
