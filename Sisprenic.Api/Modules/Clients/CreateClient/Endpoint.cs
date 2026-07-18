using FluentValidation;
using FluentValidation.Results;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;
using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Modules.Clients.CreateClient;

public static class CreateClientEndpoint
{
    public static void MapCreateClient(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle).RequireAuthorization(Permissions.Clients.Create);
    }

    private static async Task<IResult> Handle(
        IValidator<CreateClientRequest> validator,
        CreateClientRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Client client = new()
        {
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            LastName = request.LastName,
            SecondLastName = request.SecondLastName,
            Identification = request.Identification,
            PhoneNumber = request.PhoneNumber
        };

        dbContext.Client.Add(client);
        await dbContext.SaveChangesAsync(cancellationToken);

        CreateClientResponse response = new(
            client.Id,
            client.FirstName,
            client.SecondName,
            client.LastName,
            client.SecondLastName,
            client.Identification,
            client.PhoneNumber);

        return TypedResults.Created($"/clients/{client.Id}", response);
    }
}
