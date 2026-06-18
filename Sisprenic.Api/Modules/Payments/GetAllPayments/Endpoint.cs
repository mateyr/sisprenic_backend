using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Modules.Payments.Shared;

namespace Sisprenic.Api.Modules.Payments.GetAllPayments;

public static class GetAllPaymentsEndpoint
{
    public static void MapGetAllPayments(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle).RequireAuthorization(Permissions.Payments.Read);
    }

    private static async Task<IResult> Handle(
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
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
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(payments);
    }
}
