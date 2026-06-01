using sisprenic.Database;

using sisprenic_backend.Entities;

namespace sisprenic_backend.Modules.Payments.DeletePayment;

public static class DeletePaymentEndpoint
{
    public static void MapDeletePayment(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", Handle).RequireAuthorization("payments:delete");
    }

    private static async Task<IResult> Handle(int id, SisprenicContext dbContext)
    {
        Payment? payment = await dbContext.Payment.FindAsync(id);
        if (payment is null) return TypedResults.NotFound();

        dbContext.Payment.Remove(payment);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
