namespace sisprenic_backend.Modules.Loans.Shared;

public record LoanSummaryDto
(
    decimal PrincipalCurrent,
    decimal InterestThisPeriod,
    decimal InterestPending,
    DateOnly NextPaymentDate
);
