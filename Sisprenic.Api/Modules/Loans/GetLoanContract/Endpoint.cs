using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

using Sisprenic.Reports.Abstractions;
using Sisprenic.Reports.Reports.LoanContract;

namespace Sisprenic.Api.Modules.Loans.GetLoanContract;

public static class GetLoanContractEndpoint
{
    public static void MapGetLoanContract(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}/contract", Handle);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        IReportRenderer reportRenderer,
        CancellationToken cancellationToken)
    {
        Loan? loan = await dbContext.Loan
            .Include(l => l.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (loan is null)
        {
            return TypedResults.NotFound();
        }

        LoanContractViewModel viewModel = BuildViewModel(loan);
        LoanContractReport report = new(viewModel);

        byte[] pdf = await reportRenderer.RenderAsync(report, cancellationToken);

        return Results.File(pdf, "application/pdf");
    }

    private static LoanContractViewModel BuildViewModel(Loan loan)
    {
        DateOnly endDate = loan.StartDate.AddMonths(loan.TermMonths);

        return new LoanContractViewModel
        {
            ContractNumber = $"PR-{loan.Id:D6}",
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Lender = new LenderInfo
            {
                Name = "SISPRENIC"
            },
            Borrower = new BorrowerInfo
            {
                FullName = BuildFullName(loan.Client),
                Identification = loan.Client.Identification,
                PhoneNumber = loan.Client.PhoneNumber
            },
            Terms = new LoanTermsInfo
            {
                Currency = "C$",
                Principal = loan.Principal,
                MonthlyInterestRate = loan.InterestRate,
                TermMonths = loan.TermMonths,
                StartDate = loan.StartDate,
                EndDate = endDate,
                EstimatedMonthlyInterest = loan.Principal * loan.InterestRate
            }
        };
    }

    private static string BuildFullName(Client client)
    {
        string?[] parts =
        [
            client.FirstName,
            client.SecondName,
            client.LastName,
            client.SecondLastName
        ];

        return string.Join(' ', parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
