namespace sisprenic_backend.Entities;

public class Payment
{
    public int Id { get; set; }
    public decimal Interest { get; set; }
    public decimal Principal { get; set; }
    public DateOnly PaymentDay { get; set; }
    public string? Note { get; set; }

    public int LoanId { get; set; }
    public required Loan Loan { get; set; }
}
