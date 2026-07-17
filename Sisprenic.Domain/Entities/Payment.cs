namespace Sisprenic.Domain.Entities;

public class Payment : ISoftDeletable
{
    public int Id { get; set; }
    public decimal Interest { get; set; }
    public decimal Principal { get; set; }
    public DateOnly PaymentDay { get; set; }
    public string? Note { get; set; }

    public int LoanId { get; set; }
    public required Loan Loan { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}
