namespace sisprenic.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public string? SecondName { get; set; }
        public required string LastName { get; set; }
        public string? SecondLastName { get; set; }
        public required string Identification { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
