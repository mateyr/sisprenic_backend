namespace sisprenic_backend.Modules.Clients.GetAllClients;

public record GetAllClientsResponse(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber
);
