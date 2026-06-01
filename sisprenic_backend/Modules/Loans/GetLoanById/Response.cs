using sisprenic_backend.Modules.Loans.Shared;

namespace sisprenic_backend.Modules.Loans.GetLoanById;

public record GetLoanByIdResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    ClientSummaryResponse Client,
    LoanSummaryDto Summary
);
