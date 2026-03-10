namespace sisprenic_backend.Dtos.Loans;

public record class CreateLoanDto(
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateTime StartDate,
    int ClientId
);
