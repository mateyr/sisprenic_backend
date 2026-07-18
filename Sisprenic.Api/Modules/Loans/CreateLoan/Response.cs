namespace Sisprenic.Api.Modules.Loans.CreateLoan;

public record CreateLoanResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    int ClientId,
    string Status
);
