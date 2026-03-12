namespace sisprenic_backend.Dtos.Loans;

public record class UpdateLoanDto(
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    int ClientId
);
