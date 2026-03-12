using sisprenic.Entities;

namespace sisprenic_backend.Entities;

public class Loan
{
    public int Id { get; set; }
    public decimal Principal { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateOnly StartDate { get; set; }

    public int ClientId { get; set; }
    public required Client Client { get; set; }
}
