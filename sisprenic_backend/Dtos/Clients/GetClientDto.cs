using sisprenic_backend.Dtos.Loans;

namespace sisprenic_backend.Dtos.Clients;

public record class GetClientDto(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber,
    List<GetLoanDto> Loans
);

public record class GetAllClientDto(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber
);