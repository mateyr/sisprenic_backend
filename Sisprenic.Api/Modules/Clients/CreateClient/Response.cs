namespace Sisprenic.Api.Modules.Clients.CreateClient;

public record CreateClientResponse(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber
);
