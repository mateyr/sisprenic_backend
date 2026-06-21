using FluentValidation;
using FluentValidation.Results;

using Sisprenic.Api.Authorization;
using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Clients.UpdateClient;

public static class UpdateClientEndpoint
{
    public static void MapUpdateClient(this RouteGroupBuilder group)
    {
        group.MapPatch("/{id}", Handle).RequireAuthorization(Permissions.Clients.Update);
    }

    private static async Task<IResult> Handle(
        int id,
        IValidator<UpdateClientRequest> validator,
        UpdateClientRequest request,
        SisprenicContext dbContext,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Client? client = await dbContext.Client.FindAsync([id], cancellationToken);
        if (client is null)
            return TypedResults.NotFound();

        if (request.FirstName.HasValue)       client.FirstName = request.FirstName.Value!;
        if (request.SecondName.HasValue)      client.SecondName = request.SecondName.Value;
        if (request.LastName.HasValue)        client.LastName = request.LastName.Value!;
        if (request.SecondLastName.HasValue)  client.SecondLastName = request.SecondLastName.Value;
        if (request.Identification.HasValue)  client.Identification = request.Identification.Value!;
        if (request.PhoneNumber.HasValue)     client.PhoneNumber = request.PhoneNumber.Value!;

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
