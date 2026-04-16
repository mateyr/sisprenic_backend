namespace sisprenic_backend.Dtos.Payments;

public record class GetPaymentDto(
    int Id,
    decimal Principal,
    decimal Interest,
    DateOnly PaymentDay,
    string? Note,
    int LoanId
);
