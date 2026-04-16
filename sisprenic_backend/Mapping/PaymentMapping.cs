using sisprenic_backend.Dtos.Payments;
using sisprenic_backend.Entities;

namespace sisprenic_backend.Mapping;

public static class PaymentMapping
{
    public static GetPaymentDto ToPaymentDto(this Payment payment)
    {
        return new GetPaymentDto(
            payment.Id,
            payment.Principal,
            payment.Interest,
            payment.PaymentDay,
            payment.Note,
            payment.LoanId
        );
    }
}
