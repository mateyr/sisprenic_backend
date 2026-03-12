namespace sisprenic_backend.Dtos.Loans;

public record class CreateLoanDto(
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    int ClientId
);
