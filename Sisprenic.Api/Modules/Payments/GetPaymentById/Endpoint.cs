using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Modules.Payments.Shared;

namespace Sisprenic.Api.Modules.Payments.GetPaymentById;

public static class GetPaymentByIdEndpoint
{
    public static void MapGetPaymentById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", Handle).RequireAuthorization(Permissions.Payments.Read);
    }

    private static async Task<IResult> Handle(
        int id,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(cancellationToken);

        return payment is null ? TypedResults.NotFound() : TypedResults.Ok(payment);
    }
}
