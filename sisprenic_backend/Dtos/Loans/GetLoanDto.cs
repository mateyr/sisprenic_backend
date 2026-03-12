namespace sisprenic_backend.Dtos.Loans;

public record class GetLoanDto(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate
);
