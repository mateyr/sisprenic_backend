using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic.Entities;

using sisprenic_backend.Entities;
using sisprenic_backend.Modules.Payments.CreatePayment;

using Xunit;

namespace Sisprenic.UnitTests.Payments;

/// <summary>
/// Prueba <see cref="CreatePaymentHandler.Execute"/> contra un contexto EF in-memory:
/// límites de interés/capital, mensajes cuando sobra importe y fallos declarados.
/// </summary>
public sealed class CreatePaymentServiceTests
{
    private static readonly DateOnly Anchor = new DateOnly(2026, 1, 1);

    [Fact(DisplayName = "Pago dentro de límites: aplica íntegro interés y capital; sin mensaje")]
    public async Task WithinLimits_AppliesInterestAndPrincipal()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 2_000m, Interest: 1_000m, PaymentDay: new DateOnly(2026, 1, 15), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Messages);
        Assert.NotNull(result.Payment);
        Assert.Equal(2_000m, result.Payment!.Principal);
        Assert.Equal(1_000m, result.Payment.Interest);
        Assert.Single(db.Payment);
    }

    [Fact(DisplayName = "Interés por encima del pendiente (con margen): exceso a capital; mensaje de reruteo")]
    public async Task InterestOverflow_RoutesToPrincipal()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 0m, Interest: 1_500m, PaymentDay: new DateOnly(2026, 1, 15), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);

        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "500.00 enviados como interés excedían el interés pendiente y fueron aplicados a capital.",
            messages);

        Assert.DoesNotContain(
            messages,
            m => m.Contains("Se enviaron"));
        Assert.NotNull(result.Payment);
        Assert.Equal(500m, result.Payment!.Principal);
        Assert.Equal(1_000m, result.Payment.Interest);
    }

    [Fact(DisplayName = "Interés enorme: tope interés+capital; informa aplicado vs enviado")]
    public async Task InterestHuge_CapsPrincipal_ReportsPartialApplication()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 0m, Interest: 15_000m, PaymentDay: new DateOnly(2026, 1, 15), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        
        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "Se enviaron 15000.00 pero el sistema solo aplicó 11000.00 para no exceder la deuda total. 4000.00 no fueron registrados.",
            messages);

        Assert.NotNull(result.Messages);
        Assert.NotNull(result.Payment);
        Assert.Equal(10_000m, result.Payment!.Principal);
        Assert.Equal(1_000m, result.Payment.Interest);
    }

    [Fact(DisplayName = "Capital por encima del pendiente: solo aplica capital vigente y avisa")]
    public async Task PrincipalExceedsOutstanding_CapsAndReports()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 12_000m, Interest: 0m, PaymentDay: new DateOnly(2026, 1, 15), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        
        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "Existía interés pendiente; 1000.00 enviados como capital fueron aplicados a interés.",
            messages);

        Assert.Contains(
            "Se enviaron 12000.00 pero el sistema solo aplicó 11000.00 para no exceder la deuda total. 1000.00 no fueron registrados.",
            messages);

        Assert.NotNull(result.Payment);
        Assert.Equal(10_000m, result.Payment!.Principal);
        Assert.Equal(1000m, result.Payment.Interest);
    }

    [Fact(DisplayName = "Mezcla: interés con exceso + capital hasta llenar débito total; sólo mensaje por no aplicado")]
    public async Task Mixed_OverflowFullyAbsorbedByPrincipal()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 10_000m, Interest: 1_500m, PaymentDay: new DateOnly(2026, 1, 15), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        
        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "Se enviaron 11500.00 pero el sistema solo aplicó 11000.00 para no exceder la deuda total. 500.00 no fueron registrados.",
            messages);

        Assert.DoesNotContain(
            messages,
            m => m.Contains("exceso", StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(result.Payment);
        Assert.Equal(10_000m, result.Payment!.Principal);
        Assert.Equal(1_000m, result.Payment.Interest);
    }

    [Fact(DisplayName = "Sin interés pendiente: montos marcados como interés pasan a capital")]
    public async Task NoInterestOutstanding_InterestMarkedGoesToPrincipal_WithMessage()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        DateOnly cycle0Day = new DateOnly(2026, 1, 10);

        await using (SisprenicContext dbPrior = CreateContext(dbName))
        {
            dbPrior.Payment.Add(new Payment
            {
                Interest = 1_000m,
                Principal = 0m,
                PaymentDay = cycle0Day,
                LoanId = loan.Id,
                Loan = null!,
            });
            await dbPrior.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using SisprenicContext db = CreateContext(dbName);
        CreatePaymentRequest dto = new(Principal: 0m, Interest: 500m, PaymentDay: cycle0Day, Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        
        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "No había interés pendiente; 500.00 enviados como interés fueron aplicados a capital.",
            messages); Assert.NotNull(result.Payment);

        Assert.Equal(500m, result.Payment!.Principal);
        Assert.Equal(0m, result.Payment.Interest);
    }

    [Fact(DisplayName = "Préstamo ya saldado: falla sin persistir pago")]
    public async Task LoanAlreadySettled_ReturnsFailure()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        DateOnly payDay = new DateOnly(2026, 1, 15);

        await using (SisprenicContext db0 = CreateContext(dbName))
        {
            db0.Payment.Add(new Payment
            {
                Interest = 1_000m,
                Principal = 10_000m,
                PaymentDay = payDay,
                LoanId = loan.Id,
                Loan = null!,
            });
            await db0.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using SisprenicContext db = CreateContext(dbName);
        CreatePaymentRequest dto = new(Principal: 1m, Interest: 0m, PaymentDay: payDay, Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.False(result.IsSuccess);
        Assert.Equal(["El préstamo ya ha sido pagado en su totalidad."], result.Errors!["loan"]);
        Assert.Equal(1, await db.Payment.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact(DisplayName = "Fecha de pago anterior al inicio del préstamo: falla de validación")]
    public async Task PaymentBeforeStartDate_ReturnsFailure()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(Principal: 100m, Interest: 0m, PaymentDay: Anchor.AddDays(-1), Note: null, LoanId: loan.Id);

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.False(result.IsSuccess);
        Assert.Contains("inicio del préstamo", result.Errors!["paymentDay"][0]);
        Assert.Equal(0, await db.Payment.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact(DisplayName = "Capital enviado con interés pendiente: cubre interés primero y resto a capital")]
    public async Task PrincipalPayment_WithOutstandingInterest_AppliesInterestFirst_ThenPrincipal()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();
        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(
            Principal: 2_000m,
            Interest: 0m,
            PaymentDay: Anchor,
            Note: null,
            LoanId: loan.Id);

        CreatePaymentResult result =
            await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);
        
        string[] messages = ExtractMessages(result);

        Assert.Contains(
            "Existía interés pendiente; 1000.00 enviados como capital fueron aplicados a interés.",
            messages);
        
        Assert.Equal(1_000m, result.Payment!.Interest);
        Assert.Equal(1_000m, result.Payment.Principal);
    }

    [Fact(DisplayName = "Pago genera múltiples mensajes cuando redistribuye y descarta excedente")]
    public async Task PaymentRedistributionAndOverflow_ReturnsMultipleMessages()
    {
        (string dbName, Loan loan) = await SeedFreshLoanAsync();

        await using SisprenicContext db = CreateContext(dbName);

        CreatePaymentRequest dto = new(
            Principal: 12_000m,
            Interest: 0m,
            PaymentDay: Anchor,
            Note: null,
            LoanId: loan.Id);

        CreatePaymentResult result =
            await CreatePaymentHandler.Execute(loan, dto, db);

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Messages);
        Assert.Equal(2, result.Messages!.Count);

        Assert.Contains(
            result.Messages,
            m => m.Code == "interest_applied_from_principal");

        Assert.Contains(
            result.Messages,
            m => m.Code == "amount_unapplied");

        Assert.Equal(10_000m, result.Payment!.Principal);
        Assert.Equal(1_000m, result.Payment.Interest);
    }

    private static string[] ExtractMessages(CreatePaymentResult result) =>
        result.Messages?
            .Select(m => m.Message)
            .ToArray()
        ?? [];

    private static SisprenicContext CreateContext(string databaseName) =>
        new(new DbContextOptionsBuilder<SisprenicContext>()
            .UseInMemoryDatabase(databaseName)
            .Options);

    private static async Task<(string DbName, Loan Loan)> SeedFreshLoanAsync()
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
            StartDate = Anchor,
            Client = client,
        };

        await using SisprenicContext db = CreateContext(dbName);
        db.Loan.Add(loan);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (dbName, loan);
    }
}
