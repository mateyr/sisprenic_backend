using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Modules.Loans.CreateLoan;

public static class CreateLoanEndpoint
{
    public static void MapCreateLoan(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle).RequireAuthorization(Permissions.Loans.Create);
    }

    private static async Task<IResult> Handle(
        IValidator<CreateLoanRequest> validator,
        CreateLoanRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        bool clientExists = await dbContext.Client.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["ClientId"] = [$"No existe un cliente con el id {request.ClientId}."]
                });
        }

        Loan loan = new()
        {
            Principal = request.Principal!.Value,
            InterestRate = request.InterestRate!.Value,
            TermMonths = request.TermMonths!.Value,
            StartDate = request.StartDate!.Value,
            ClientId = request.ClientId!.Value,
            Client = null!
        };

        dbContext.Loan.Add(loan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.Created($"/loans/{loan.Id}", loan);
    }
}
