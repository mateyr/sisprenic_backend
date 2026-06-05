namespace Sisprenic.Reports.Reports.LoanContract;

public sealed class LoanContractViewModel
{
    public required string ContractNumber { get; init; }
    public required DateOnly IssueDate { get; init; }

    public required LenderInfo Lender { get; init; }
    public required BorrowerInfo Borrower { get; init; }
    public required LoanTermsInfo Terms { get; init; }
}

// Data of the entity granting the loan (the company).
public sealed class LenderInfo
{
    public required string Name { get; init; }
    public string? LegalId { get; init; }
    public string? Address { get; init; }
}

// Borrower (client) data.
public sealed class BorrowerInfo
{
    public required string FullName { get; init; }
    public required string Identification { get; init; }
    public required string PhoneNumber { get; init; }
}

// Loan financial terms.
public sealed class LoanTermsInfo
{
    public required string Currency { get; init; }
    public required decimal Principal { get; init; }

    // Monthly interest rate expressed as a fraction (0.10 = 10%).
    public required decimal MonthlyInterestRate { get; init; }

    public required int TermMonths { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }

    // Estimated interest for the first period (Principal * monthly rate).
    public required decimal EstimatedMonthlyInterest { get; init; }
}
