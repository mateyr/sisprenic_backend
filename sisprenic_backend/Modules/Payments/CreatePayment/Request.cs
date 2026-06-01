namespace sisprenic_backend.Modules.Payments.CreatePayment;

public record CreatePaymentRequest(
    decimal Principal,
    decimal Interest,
    DateOnly PaymentDay,
    string? Note,
    int? LoanId
);
