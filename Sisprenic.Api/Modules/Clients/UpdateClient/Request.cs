using Sisprenic.Api.Common;

namespace Sisprenic.Api.Modules.Clients.UpdateClient;

public record UpdateClientRequest(
    Optional<string> FirstName,
    Optional<string> SecondName,
    Optional<string> LastName,
    Optional<string> SecondLastName,
    Optional<string> Identification,
    Optional<string> PhoneNumber
);
