using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Modules.Payments.Shared;

namespace sisprenic_backend.Modules.Payments.GetAllPayments;

public static class GetAllPaymentsEndpoint
{
    public static void MapGetAllPayments(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle).RequireAuthorization("payments:read");
    }

    private static async Task<IResult> Handle(SisprenicContext dbContext)
    {
        List<PaymentResponse> payments = await dbContext.Payment
            .AsNoTracking()
            .Select(p => new PaymentResponse(
                p.Id,
                p.Principal,
                p.Interest,
                p.PaymentDay,
                p.Note,
                p.LoanId))
            .ToListAsync();

        return TypedResults.Ok(payments);
    }
}
