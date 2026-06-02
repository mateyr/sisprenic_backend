namespace Sisprenic.Api.Modules.Loans.UpdateLoan;

public record UpdateLoanRequest(
    decimal? Principal,
    decimal? InterestRate,
    int? TermMonths,
    DateOnly? StartDate,
    int? ClientId
);
