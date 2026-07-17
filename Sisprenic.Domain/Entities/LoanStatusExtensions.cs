namespace Sisprenic.Domain.Entities;

public static class LoanStatusExtensions
{
    public static string ToDisplayName(this LoanStatus status) => status switch
    {
        LoanStatus.Active => "Activo",
        LoanStatus.Paid => "Pagado",
        _ => status.ToString()
    };
}
