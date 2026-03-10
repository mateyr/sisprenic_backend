namespace sisprenic_backend.Dtos.Loans;

public record class UpdateLoanDto(
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateTime StartDate,
    int ClientId
);
