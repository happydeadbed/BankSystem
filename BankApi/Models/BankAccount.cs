namespace BankApi.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}