using sisprenic_backend.Dtos.Loans;
using sisprenic_backend.Entities;

namespace sisprenic_backend.Mapping;

public static class LoanMapping
{
    public static GetLoanDto ToLoanDto(this Loan loan)
    {
        return new GetLoanDto
        (
            loan.Id,
            loan.Principal,
            loan.InterestRate,
            loan.TermMonths,
            loan.StartDate
        );
    }
}
