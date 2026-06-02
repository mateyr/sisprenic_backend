namespace Sisprenic.Api.Modules.Loans.Shared;

public record LoanSummaryDto
(
    decimal PrincipalCurrent,
    decimal InterestThisPeriod,
    decimal InterestPending,
    DateOnly NextPaymentDate
);
