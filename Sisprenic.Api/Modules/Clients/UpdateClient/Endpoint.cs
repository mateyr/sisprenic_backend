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
        SisprenicContext dbContext)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Client? client = await dbContext.Client.FindAsync(id);
        if (client is null)
            return TypedResults.NotFound();

        if (request.FirstName is not null)       client.FirstName = request.FirstName;
        if (request.SecondName is not null)      client.SecondName = request.SecondName;
        if (request.LastName is not null)        client.LastName = request.LastName;
        if (request.SecondLastName is not null)  client.SecondLastName = request.SecondLastName;
        if (request.Identification is not null)  client.Identification = request.Identification;
        if (request.PhoneNumber is not null)     client.PhoneNumber = request.PhoneNumber;

        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
