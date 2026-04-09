using static sisprenic_backend.Endpoints.Payments.PaymentTypedResults;

namespace sisprenic_backend.Endpoints.Payments;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this WebApplication app)
    {
        var route = app.MapGroup("/payments");

        route.MapGet("/", GetAllPayments).RequireAuthorization("payments:read");

        return route;
    }
}
