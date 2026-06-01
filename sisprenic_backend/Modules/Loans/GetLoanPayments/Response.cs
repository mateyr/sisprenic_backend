namespace sisprenic_backend.Modules.Loans.GetLoanPayments;

public record LoanPaymentResponse(
    int Id,
    decimal Principal,
    decimal Interest,
    DateOnly PaymentDay,
    string? Note
);
