using sisprenic_backend.Modules.Payments.CreatePayment;
using sisprenic_backend.Modules.Payments.DeletePayment;
using sisprenic_backend.Modules.Payments.GetAllPayments;
using sisprenic_backend.Modules.Payments.GetPaymentById;

namespace sisprenic_backend.Modules.Payments;

public static class PaymentsModule
{
    public static void MapPaymentsModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/payments").WithTags("Payments");

        group.MapGetAllPayments();
        group.MapGetPaymentById();
        group.MapCreatePayment();
        group.MapDeletePayment();
    }
}
