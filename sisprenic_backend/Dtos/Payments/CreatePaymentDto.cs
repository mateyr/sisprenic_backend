namespace sisprenic_backend.Dtos.Payments;

public record class CreatePaymentDto(
    decimal Principal,
    decimal Interest,
    DateOnly PaymentDay,
    string? Note,
    int LoanId
);
