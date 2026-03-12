using sisprenic.Entities;

using sisprenic_backend.Dtos.Clients;

namespace sisprenic_backend.Mapping;

public static class ClientMapping
{
    public static GetClientDto ToClientDto(this Client client)
    {
        return new GetClientDto
        (
            client.Id,
            client.FirstName,
            client.SecondLastName,
            client.LastName,
            client.SecondLastName,
            client.Identification,
            client.PhoneNumber,
            client.Loans.Select(l => l.ToLoanDto()).ToList()
        );
    }
}
