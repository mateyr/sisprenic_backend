using static sisprenic_backend.Endpoints.Payments.PaymentTypedResults;

namespace sisprenic_backend.Endpoints.Payments;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this WebApplication app)
    {
        var route = app.MapGroup("/payments");

        route.MapGet("/", GetAllPayments).RequireAuthorization("payments:read");
        route.MapGet("/{id}", GetPayment).RequireAuthorization("payments:read");
        route.MapPost("/", CreatePayment).RequireAuthorization("payments:create");
        route.MapDelete("/{id}", DeletePayment).RequireAuthorization("payments:delete");

        return route;
    }
}
