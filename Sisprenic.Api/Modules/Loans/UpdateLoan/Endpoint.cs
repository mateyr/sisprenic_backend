using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Modules.Loans.UpdateLoan;

public static class UpdateLoanEndpoint
{
    public static void MapUpdateLoan(this RouteGroupBuilder group)
    {
        group.MapPatch("/{id:int}", Handle).RequireAuthorization(Permissions.Loans.Update);
    }

    private static async Task<IResult> Handle(
        int id,
        IValidator<UpdateLoanRequest> validator,
        UpdateLoanRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Loan? loan = await dbContext.Loan.FindAsync([id], cancellationToken);
        if (loan is null) return TypedResults.NotFound();

        bool hasPayments = await dbContext.Payment.AnyAsync(p => p.LoanId == id, cancellationToken);

        if (hasPayments)
        {
            bool tryingToChangeRestrictedFields =
                request.Principal.HasValue ||
                request.InterestRate.HasValue ||
                request.TermMonths.HasValue ||
                request.StartDate.HasValue;

            if (tryingToChangeRestrictedFields)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Loan"] = ["No se puede modificar un préstamo que ya tiene pagos registrados. Solo está permitido cambiar el cliente."]
                });
        }

        if (request.ClientId.HasValue && loan.ClientId != request.ClientId.Value)
        {
            bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == request.ClientId.Value, cancellationToken);
            if (!clientExists)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ClientId"] = [$"No existe un cliente con el id {request.ClientId.Value}."]
                });
        }

        if (request.Principal.HasValue)    loan.Principal    = request.Principal.Value;
        if (request.InterestRate.HasValue) loan.InterestRate = request.InterestRate.Value;
        if (request.TermMonths.HasValue)   loan.TermMonths   = request.TermMonths.Value;
        if (request.StartDate.HasValue)    loan.StartDate    = request.StartDate.Value;
        if (request.ClientId.HasValue)     loan.ClientId     = request.ClientId.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
