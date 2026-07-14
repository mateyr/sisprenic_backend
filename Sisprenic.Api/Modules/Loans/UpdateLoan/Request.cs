using Sisprenic.Api.Common;

namespace Sisprenic.Api.Modules.Loans.UpdateLoan;

public record UpdateLoanRequest(
    Optional<decimal> Principal,
    Optional<decimal> InterestRate,
    Optional<int> TermMonths,
    Optional<DateOnly> StartDate,
    Optional<int> ClientId
);
