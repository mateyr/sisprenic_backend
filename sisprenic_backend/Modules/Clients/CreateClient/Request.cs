namespace sisprenic_backend.Modules.Clients.CreateClient;

public record CreateClientRequest(
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber
);
