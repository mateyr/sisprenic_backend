namespace sisprenic_backend.Modules.Payments.Shared;

public record PaymentResponse(
    int Id,
    decimal Principal,
    decimal Interest,
    DateOnly PaymentDay,
    string? Note,
    int LoanId
);
