using sisprenic_backend.Dtos.Clients;
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

    public static GetLoanDetailDto ToLoanDetailDto(this Loan loan)
    {
        return new GetLoanDetailDto
        (
            loan.Id,
            loan.Principal,
            loan.InterestRate,
            loan.TermMonths,
            loan.StartDate,
            new ClientSummaryDto(
                loan.Client.Id,
                loan.Client.FirstName,
                loan.Client.SecondLastName,
                loan.Client.LastName,
                loan.Client.SecondLastName,
                loan.Client.Identification,
                loan.Client.PhoneNumber
            )
        );
    }
}
