namespace Sisprenic.Api.Modules.Loans.Shared;

public record LoanSummaryDto
(
    decimal PrincipalCurrent,
    decimal InterestThisPeriod,
    decimal InterestPending,
    DateOnly NextPaymentDate
)
{
    // Total interest the loan still owes: overdue cycles plus the current one.
    public decimal InterestOutstanding => InterestPending + InterestThisPeriod;
}
