using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Entities
{
    public class Client : ISoftDeletable
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public string? SecondName { get; set; }
        public required string LastName { get; set; }
        public string? SecondLastName { get; set; }
        public required string Identification { get; set; }
        public required string PhoneNumber { get; set; }

        public ICollection<Loan> Loans { get; } = new List<Loan>();

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
