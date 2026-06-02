using Sisprenic.Api.Modules.Loans.Shared;

namespace Sisprenic.Api.Modules.Loans.GetLoanById;

public record GetLoanByIdResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    ClientSummaryResponse Client,
    LoanSummaryDto Summary
);
