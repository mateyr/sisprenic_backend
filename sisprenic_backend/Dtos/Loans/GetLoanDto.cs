using sisprenic_backend.Dtos.Clients;

namespace sisprenic_backend.Dtos.Loans;

public record class GetLoanDto(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate
);

public record GetLoanDetailDto(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate,
    ClientSummaryDto Client
);
