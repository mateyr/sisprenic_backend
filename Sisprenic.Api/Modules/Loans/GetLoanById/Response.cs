using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetLoanById;

public record GetLoanByIdResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    string Status,
    ClientSummaryResponse Client,
    LoanSummaryDto Summary
);
