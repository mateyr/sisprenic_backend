using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetAllLoans;

public record GetAllLoansResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    ClientSummaryResponse Client
);
