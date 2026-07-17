using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;

using Sisprenic.Api.Common;
using Sisprenic.Domain.Entities;
using Sisprenic.Api.Modules.Loans.Shared;
using Sisprenic.Api.Modules.Payments.Shared;

namespace Sisprenic.Api.Modules.Payments.CreatePayment;

public static class CreatePaymentEndpoint
{
    public static void MapCreatePayment(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle).RequireAuthorization(Permissions.Payments.Create);
    }

    private static async Task<IResult> Handle(
        IValidator<CreatePaymentRequest> validator,
        CreatePaymentRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        Loan? loan = await LoanLockService.LoadForUpdateAsync(dbContext, request.LoanId!.Value, cancellationToken);

        if (loan is null)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["loanId"] = [$"No existe un préstamo con el id {request.LoanId}."]
                });
        }

        CreatePaymentResult result = await CreatePaymentHandler.Execute(loan, request, dbContext, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.ValidationProblem(result.Errors!);
        }

        await transaction.CommitAsync(cancellationToken);

        Payment created = result.Payment!;

        PaymentResponse data = new(
            created.Id,
            created.Principal,
            created.Interest,
            created.PaymentDay,
            created.Note,
            created.LoanId);

        ApiResponse<PaymentResponse> body = new(
            Data: data,
            Messages: result.Messages);

        return TypedResults.Created($"/payments/{created.Id}", body);
    }
}
