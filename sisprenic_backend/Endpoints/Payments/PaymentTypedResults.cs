using Microsoft.EntityFrameworkCore;

using sisprenic.Database;

using sisprenic_backend.Entities;

namespace sisprenic_backend.Endpoints.Payments;

public static class PaymentTypedResults
{
    public static async Task<IResult> GetAllPayments(SisprenicContext dbContext)
    {
        List<Payment> payments = await dbContext.Payment.AsNoTracking().ToListAsync();
        return TypedResults.Ok(payments);
    }
}
