namespace Sisprenic.Api.Modules.Loans.CreateLoan;

public record CreateLoanRequest(
    decimal? Principal,
    decimal? InterestRate,
    int? TermMonths,
    DateOnly? StartDate,
    int? ClientId
);
