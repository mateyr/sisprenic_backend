using static sisprenic_backend.Endpoints.Loans.LoanTypedResults;

namespace sisprenic_backend.Endpoints.Loans;

public static class LoanEndpoints
{
    public static RouteGroupBuilder MapLoanEndpoints(this WebApplication app)
    {
        var route = app.MapGroup("/loans");

        route.MapGet("/", GetAllLoans);
        route.MapGet("/{id}", GetLoan);
        route.MapGet("/{id}/payments", GetLoanPayments);
        route.MapPost("/", CreateLoan);
        route.MapPut("/{id}", UpdateLoan);
        route.MapDelete("/{id}", DeleteLoan);

        return route;
    }
}
