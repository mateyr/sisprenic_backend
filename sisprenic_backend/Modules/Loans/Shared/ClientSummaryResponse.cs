namespace sisprenic_backend.Modules.Loans.Shared;

public record ClientSummaryResponse(
    int Id,
    string FirstName,
    string? SecondName,
    string LastName,
    string? SecondLastName,
    string Identification,
    string PhoneNumber
);
