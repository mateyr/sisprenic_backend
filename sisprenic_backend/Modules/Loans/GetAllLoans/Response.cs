using sisprenic_backend.Modules.Loans.Shared;

namespace sisprenic_backend.Modules.Loans.GetAllLoans;

public record GetAllLoansResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    ClientSummaryResponse Client
);
