using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Common;
using Sisprenic.Api.Database;

using Sisprenic.Domain.Entities;
using Sisprenic.Api.Modules.Payments.DeletePayment;

using Xunit;

namespace Sisprenic.UnitTests.Payments;

/// <summary>
/// Prueba <see cref="DeletePaymentHandler.Execute"/> contra un contexto EF in-memory:
/// el recálculo del estado del préstamo al eliminar un pago (revertir a Activo o no alterar
/// un préstamo Activo) y la eliminación cuando no hay préstamo asociado.
/// </summary>
public sealed class DeletePaymentServiceTests
{
    // El handler usa BusinessClock.Today() internamente; anclamos el préstamo y los pagos a
    // esa misma fecha para que todo caiga en el ciclo 0 y el cálculo sea determinista.
    private static readonly DateOnly Today = BusinessClock.Today();

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact(DisplayName = "Borrar el pago que saldó el préstamo: revierte a Activo")]
    public async Task DeletingSettlingPayment_RevertsToActive()
    {
        // Interés esperado del ciclo 0 = 10000 * 0.10 = 1000; este pago cubre interés + capital.
        (string dbName, int loanId, List<int> paymentIds) =
            await SeedAsync(LoanStatus.Paid, [(Interest: 1_000m, Principal: 10_000m)]);

        await using SisprenicContext db = CreateContext(dbName);
        Payment payment = (await db.Payment.FindAsync([paymentIds[0]], Ct))!;
        Loan loan = (await db.Loan.FindAsync([loanId], Ct))!;

        await DeletePaymentHandler.Execute(loan, payment, db, Ct);

        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Equal(0, await db.Payment.CountAsync(Ct));
    }

    [Fact(DisplayName = "Préstamo saldado por dos pagos: borrar uno revierte a Activo")]
    public async Task DeletingOneOfTwoSettlingPayments_RevertsToActive()
    {
        (string dbName, int loanId, List<int> paymentIds) =
            await SeedAsync(LoanStatus.Paid,
            [
                (Interest: 1_000m, Principal: 0m),
                (Interest: 0m, Principal: 10_000m),
            ]);

        await using SisprenicContext db = CreateContext(dbName);
        Payment principalPayment = (await db.Payment.FindAsync([paymentIds[1]], Ct))!;
        Loan loan = (await db.Loan.FindAsync([loanId], Ct))!;

        await DeletePaymentHandler.Execute(loan, principalPayment, db, Ct);

        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Equal(1, await db.Payment.CountAsync(Ct));
    }

    [Fact(DisplayName = "Préstamo Activo: borrar un pago parcial lo mantiene Activo")]
    public async Task DeletingPartialPayment_KeepsActive()
    {
        (string dbName, int loanId, List<int> paymentIds) =
            await SeedAsync(LoanStatus.Active,
            [
                (Interest: 500m, Principal: 0m),
                (Interest: 200m, Principal: 0m),
            ]);

        await using SisprenicContext db = CreateContext(dbName);
        Payment payment = (await db.Payment.FindAsync([paymentIds[0]], Ct))!;
        Loan loan = (await db.Loan.FindAsync([loanId], Ct))!;

        await DeletePaymentHandler.Execute(loan, payment, db, Ct);

        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Equal(1, await db.Payment.CountAsync(Ct));
    }

    [Fact(DisplayName = "Sin préstamo asociado: elimina el pago sin tocar estado")]
    public async Task NoLoan_RemovesPaymentOnly()
    {
        (string dbName, _, List<int> paymentIds) =
            await SeedAsync(LoanStatus.Active, [(Interest: 500m, Principal: 0m)]);

        await using SisprenicContext db = CreateContext(dbName);
        Payment payment = (await db.Payment.FindAsync([paymentIds[0]], Ct))!;

        await DeletePaymentHandler.Execute(loan: null, payment, db, Ct);

        Assert.Equal(0, await db.Payment.CountAsync(Ct));
    }

    private static SisprenicContext CreateContext(string databaseName) =>
        new(new DbContextOptionsBuilder<SisprenicContext>()
            .UseInMemoryDatabase(databaseName)
            .Options);

    private static async Task<(string DbName, int LoanId, List<int> PaymentIds)> SeedAsync(
        LoanStatus status,
        IReadOnlyList<(decimal Interest, decimal Principal)> payments)
    {
        string dbName = Guid.NewGuid().ToString();

        Client client = new()
        {
            FirstName = "Rodian",
            SecondName = null,
            LastName = "Matey",
            SecondLastName = null,
            Identification = "T1",
            PhoneNumber = "88888888",
        };

        Loan loan = new()
        {
            Principal = 10_000m,
            InterestRate = 0.10m,
            TermMonths = 3,
            StartDate = Today,
            Status = status,
            Client = client,
        };

        await using SisprenicContext db = CreateContext(dbName);
        db.Loan.Add(loan);
        await db.SaveChangesAsync(Ct);

        List<int> paymentIds = [];
        foreach ((decimal interest, decimal principal) in payments)
        {
            Payment payment = new()
            {
                Interest = interest,
                Principal = principal,
                PaymentDay = Today,
                LoanId = loan.Id,
                Loan = null!,
            };

            db.Payment.Add(payment);
            await db.SaveChangesAsync(Ct);
            paymentIds.Add(payment.Id);
        }

        return (dbName, loan.Id, paymentIds);
    }
}
