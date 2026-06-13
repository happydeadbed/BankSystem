namespace BankApi.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public string Status { get; set; } = "Active";
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

    }
}
