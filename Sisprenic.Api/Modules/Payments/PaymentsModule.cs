using Sisprenic.Api.Modules.Payments.CreatePayment;
using Sisprenic.Api.Modules.Payments.DeletePayment;
using Sisprenic.Api.Modules.Payments.GetAllPayments;
using Sisprenic.Api.Modules.Payments.GetPaymentById;

namespace Sisprenic.Api.Modules.Payments;

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
