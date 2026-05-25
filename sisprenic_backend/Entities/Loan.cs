using sisprenic.Entities;

namespace sisprenic_backend.Entities;

public class Loan : ISoftDeletable
{
    public int Id { get; set; }
    public decimal Principal { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateOnly StartDate { get; set; }

    public int ClientId { get; set; }
    public required Client Client { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
