using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Modules.Payments.Shared;

namespace sisprenic_backend.Modules.Payments.GetPaymentById;

public static class GetPaymentByIdEndpoint
{
    public static void MapGetPaymentById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", Handle).RequireAuthorization("payments:read");
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        PaymentResponse? payment = await dbContext.Payment
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PaymentResponse(
                p.Id,
                p.Principal,
                p.Interest,
                p.PaymentDay,
                p.Note,
                p.LoanId))
            .FirstOrDefaultAsync();

        return payment is null ? TypedResults.NotFound() : TypedResults.Ok(payment);
    }
}
