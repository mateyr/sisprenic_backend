namespace sisprenic_backend.Modules.Clients.GetClientById;

public record GetClientByIdResponse(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber,
    List<ClientLoanResponse> Loans
);

public record ClientLoanResponse(
    int Id,
    decimal Principal,
    decimal InterestRate,
    int TermMonths,
    DateOnly StartDate
);
