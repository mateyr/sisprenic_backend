namespace sisprenic_backend.Modules.Clients.UpdateClient;

public record UpdateClientRequest(
    string FirstName,
    string SecondName,
    string LastName,
    string SecondLastName,
    string Identification,
    string PhoneNumber
);
