namespace sisprenic_backend.Dtos.Clients;

public record class UpdateClientDto(
    string FirstName,
    string SecondName,
    string LastName,
    string SecondLastName,
    string Identification,
    string PhoneNumber
);
